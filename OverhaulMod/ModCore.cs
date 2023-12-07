using ModLibrary;
using ModLibrary.YieldInstructions;
using OverhaulMod.Patches.Replacements;
using OverhaulMod.Utils;
using OverhaulMod.Visuals;
using System;
using System.Collections;
using UnityEngine;

namespace OverhaulMod
{
    [MainModClass]
    public class ModCore : Mod
    {
        public static event Action GameInitialized;

        public static ModCore instance { get; private set; }

        private static string s_folder;
        public static string folder
        {
            get
            {
                ModCore modCore = instance;
                if (modCore == null)
                {
                    return null;
                }

                if (s_folder == null)
                {
                    s_folder = modCore.ModInfo.FolderPath;
                }
                return s_folder;
            }
        }

        private static string s_savesFolder;
        public static string savesFolder
        {
            get
            {
                if (s_savesFolder == null)
                {
                    s_savesFolder = folder + "saves/";
                }
                ModIOUtils.CreateFolderIfNotCreated(s_savesFolder);
                return s_savesFolder;
            }
        }

        private static string s_assetsFolder;
        public static string assetsFolder
        {
            get
            {
                if (s_assetsFolder == null)
                {
                    s_assetsFolder = folder + "assets/";
                }
                return s_assetsFolder;
            }
        }

        private bool m_hasAddedListeners;

        public override void OnModEnabled()
        {
            instance = this;

            addEventListeners();
        }

        public override void OnModLoaded()
        {
            instance = this;

            ModLoader.Load();
            ModReplacement.Load();

            addEventListeners();
        }

        public override void OnModDeactivated()
        {
            instance = null;
        }

        public override UnityEngine.Object OnResourcesLoad(string path)
        {
            ModLevelEditorManager manager = ModLevelEditorManager.Instance;
            if (manager)
            {
                if (manager.HasTexture(path))
                    return manager.GetTexture(path);

                if (manager.HasTransform(path))
                    return manager.GetTransform(path);
            }
            return null;
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover)
        {
            _ = ModActionUtils.RunCoroutine(waitUntilCharacterModelInitialization(firstPersonMover));
        }

        public override void OnUpgradesRefreshed(FirstPersonMover owner, UpgradeCollection upgrades)
        {
            owner.RefreshModWeaponModels();
        }

        private IEnumerator waitUntilCharacterModelInitialization(FirstPersonMover firstPersonMover)
        {
            yield return new WaitForCharacterModelInitialization(firstPersonMover);
            yield return new WaitUntil(() => !firstPersonMover || firstPersonMover.gameObject.activeInHierarchy);
            yield return new WaitForEndOfFrame();
            if (firstPersonMover && firstPersonMover.IsAlive())
            {
                ModWeaponsManager.Instance.AddWeaponsToRobot(firstPersonMover);
            }
            yield return new WaitForEndOfFrame();
            if (firstPersonMover && firstPersonMover.IsAlive())
            {
                firstPersonMover.RefreshModWeaponModels();

                if (firstPersonMover.IsAttachedAndAlive())
                {
                    _ = firstPersonMover.gameObject.AddComponent<RobotWeaponBag>();
                }
            }
            yield break;
        }

        private void addEventListeners()
        {
            if (m_hasAddedListeners)
            {
                return;
            }
            m_hasAddedListeners = true;

            GlobalEventManager.Instance.AddEventListenerOnce(GlobalEvents.GameInitializtionCompleted, onGameInitialized);
        }

        private void onGameInitialized()
        {
            Debug.LogWarning("GAME INITIALIZED");
            GameInitialized?.Invoke();
        }
    }
}
