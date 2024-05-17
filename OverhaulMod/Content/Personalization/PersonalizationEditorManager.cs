using OverhaulMod.UI;
using OverhaulMod.Utils;
using Steamworks;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorManager : Singleton<PersonalizationEditorManager>
    {
        public const string OBJECT_EDITED_EVENT = "PersonalizationEditorObjectEdited";

        private PersonalizationController m_editingPersonalizationController;
        public PersonalizationController editingPersonalizationController
        {
            get
            {
                if (!m_editingPersonalizationController)
                    m_editingPersonalizationController = CharacterTracker.Instance?.GetPlayer()?.GetComponent<PersonalizationController>();

                return m_editingPersonalizationController;
            }
        }

        public PersonalizationItemInfo editingItemInfo
        {
            get;
            set;
        }
        public PersonalizationEditorObjectBehaviour editingRoot { get; set; }

        public string editingFolder
        {
            get;
            set;
        }

        private string m_editorId;
        public string editorId
        {
            get
            {
                if (!SteamManager.Instance || !SteamManager.Instance.Initialized)
                    return null;

                if (m_editorId == null)
                {
                    m_editorId = SteamUser.GetSteamID().ToString();
                }
                return m_editorId;
            }
        }

        public bool canVerifyItems
        {
            get
            {
                return true;
            }
        }

        public bool canEditNonOwnItems
        {
            get
            {
                return true;
            }
        }

        public static bool IsInEditor()
        {
            return GameFlowManager.Instance._gameMode == (GameMode)2500;
        }

        public void StartEditorGameMode()
        {
            GameFlowManager.Instance._gameMode = (GameMode)2500;

            LevelManager.Instance.CleanUpLevelThisFrame();
            GameFlowManager.Instance.HideTitleScreen(false);

            GameUIRoot.Instance.LoadingScreen.Show();

            GameDataManager.Instance.SaveHighScoreDataWithoutModifyingIt();
            CacheManager.Instance.CreateOrClearInstance();
            GarbageManager.Instance.DestroyAllGarbage();

            SingleplayerServerStarter.Instance.StartServerThenCall(delegate
            {
                UIPersonalizationEditor editorUi = ModUIConstants.ShowPersonalizationEditorUI();

                GameUIRoot.Instance.LoadingScreen.Hide();
                GlobalEventManager.Instance.Dispatch(GlobalEvents.LevelSpawned);

                GameObject cameraObject = Instantiate(PlayerCameraManager.Instance.DefaultGameCameraPrefab.gameObject);
                cameraObject.tag = "MainCamera";
                cameraObject.transform.position = new Vector3(-2.5f, 3f, 3f);
                cameraObject.transform.eulerAngles = new Vector3(5f, 120f, 0f);
                PersonalizationEditorCamera personalizationEditorCamera = cameraObject.AddComponent<PersonalizationEditorCamera>();
                personalizationEditorCamera.ToolBarTransform = editorUi.ToolBarTransform;
                /*personalizationEditorCamera.LeftPanelTransform = editorUi.LeftPanelTransform;*/

                LevelEditorLevelData levelEditorLevelData = null;
                try
                {
                    levelEditorLevelData = ModJsonUtils.DeserializeStream<LevelEditorLevelData>(ModCore.dataFolder + "levels/personalizationEditorLevel.json");
                }
                catch
                {
                    LevelManager.Instance._currentLevelHidesTheArena = false;
                }

                _ = base.StartCoroutine(spawnLevelCoroutine(levelEditorLevelData));
            });
        }

        public void EditItem(PersonalizationItemInfo personalizationItemInfo, string folder)
        {
            editingItemInfo = personalizationItemInfo;
            editingFolder = folder;
            UIPersonalizationEditor.instance.Inspector.Populate(personalizationItemInfo);
            SpawnRootObject();
        }

        public bool SaveItem(out string error)
        {
            if (editingItemInfo == null)
            {
                error = "Editing item info is NULL";
                return false;
            }

            if (!editingRoot)
            {
                error = "Editing item is NULL";
                return false;
            }

            string folder = editingFolder;
            if (folder.IsNullOrEmpty())
            {
                error = "Could not find folder";
                return false;
            }

            if (!Directory.Exists(folder))
                _ = Directory.CreateDirectory(folder);

            UIPersonalizationEditor.instance.Inspector.ApplyValues();
            SerializeRoot();
            try
            {
                ModJsonUtils.WriteStream(folder + PersonalizationManager.ITEM_INFO_FILE, editingItemInfo);
            }
            catch (Exception exc)
            {
                error = exc.ToString();
                return false;
            }
            error = null;
            return true;
        }

        public void SerializeRoot()
        {
            editingItemInfo.RootObject = editingRoot.Serialize();
        }

        public void SpawnBot()
        {
            _ = base.StartCoroutine(spawnBotCoroutine());
        }

        private IEnumerator spawnBotCoroutine()
        {
            PersonalizationController personalizationController = editingPersonalizationController;
            if (personalizationController)
            {
                Destroy(personalizationController.gameObject);
            }

            GameObject spawnPoint = new GameObject();

            FirstPersonMover bot = GameFlowManager.Instance.SpawnPlayer(spawnPoint.transform, true, true);
            bot.transform.eulerAngles = Vector3.up * 90f;
            if (bot._playerCamera)
                bot._playerCamera.gameObject.SetActive(false);

            DelegateScheduler.Instance.Schedule(delegate
            {
                bot._upgradeCollection.AddUpgradeIfMissing(UpgradeType.BowUnlock, 1);
                bot._upgradeCollection.AddUpgradeIfMissing(UpgradeType.Hammer, 3);
                bot._upgradeCollection.AddUpgradeIfMissing(UpgradeType.SpearUnlock, 1);
                bot._upgradeCollection.AddUpgradeIfMissing(UpgradeType.ShieldSize, 1);
                bot.RefreshUpgrades();
            }, 0.5f);

            Destroy(spawnPoint);
            yield break;
        }

        private IEnumerator spawnLevelCoroutine(LevelEditorLevelData levelEditorLevelData)
        {
            if (levelEditorLevelData != null)
            {
                GameObject level = new GameObject();
                LevelManager.Instance._currentLevelHidesTheArena = true;
                yield return base.StartCoroutine(LevelEditorDataManager.Instance.DeserializeInto(level.transform, levelEditorLevelData));
            }
            else
            {
                LevelManager.Instance._currentLevelHidesTheArena = false;
            }
            GlobalEventManager.Instance.Dispatch(GlobalEvents.LevelSpawned);
            SpawnBot();
            yield break;
        }

        public void SpawnRootObject()
        {
            PersonalizationEditorObjectInfo rootInfo = editingItemInfo?.RootObject;
            if (rootInfo == null)
                return;

            PersonalizationController personalizationController = editingPersonalizationController;
            if (!personalizationController)
                return;

            personalizationController.DestroyAllItems();
            editingRoot = personalizationController.SpawnItem(editingItemInfo);
            PersonalizationEditorObjectManager.Instance.SetCurrentRootNextUniqueIndex(rootInfo.NextUniqueIndex);
        }
    }
}
