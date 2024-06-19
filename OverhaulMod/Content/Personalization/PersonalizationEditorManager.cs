using ICSharpCode.SharpZipLib.Zip;
using OverhaulMod.Engine;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorManager : Singleton<PersonalizationEditorManager>
    {
        public const string ITEM_INFO_FILE = "itemInfo.json";

        public const string OBJECT_EDITED_EVENT = "PersonalizationEditorObjectEdited";

        public const string PRESET_PREVIEW_CHANGED_EVENT = "PersonalizationEditorPresetPreviewChanged";

        private PersonalizationController m_currentPersonalizationController;
        public PersonalizationController currentPersonalizationController
        {
            get
            {
                if (!m_currentPersonalizationController)
                    m_currentPersonalizationController = CharacterTracker.Instance?.GetPlayer()?.GetComponent<PersonalizationController>();

                return m_currentPersonalizationController;
            }
        }

        public PersonalizationItemInfo currentEditingItemInfo
        {
            get;
            set;
        }

        public PersonalizationEditorObjectBehaviour currentEditingRoot
        {
            get;
            set;
        }

        public string currentEditingItemFolder
        {
            get
            {
                return currentEditingItemInfo.FolderPath;
            }
        }

        public PersonalizationEditorObjectShowConditions previewPresetKey
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
                return ModUserInfo.isDeveloper;
            }
        }

        public bool canEditNonOwnItems
        {
            get
            {
                return ModUserInfo.isDeveloper;
            }
        }

        public static bool IsInEditor()
        {
            return GameFlowManager.Instance._gameMode == (GameMode)2500;
        }

        public void StartEditorGameMode()
        {
            if (!TransitionManager.OverhaulNonSceneTransitions)
            {
                _ = base.StartCoroutine(startEditorGameModeCoroutine(false));
                return;
            }
            TransitionManager.Instance.DoNonSceneTransition(startEditorGameModeCoroutine(true));
        }

        private IEnumerator startEditorGameModeCoroutine(bool useTransitionManager)
        {
            if (useTransitionManager)
                yield return new WaitForSecondsRealtime(0.25f);

            m_currentPersonalizationController = null;
            currentEditingItemInfo = null;
            currentEditingRoot = null;
            previewPresetKey = PersonalizationEditorObjectShowConditions.IsNormal;

            GameFlowManager.Instance._gameMode = (GameMode)2500;

            LevelManager.Instance.CleanUpLevelThisFrame();
            GameFlowManager.Instance.HideTitleScreen(false);

            GameDataManager.Instance.SaveHighScoreDataWithoutModifyingIt();
            CacheManager.Instance.CreateOrClearInstance();
            GarbageManager.Instance.DestroyAllGarbage();

            SingleplayerServerStarter.Instance.StartServerThenCall(delegate
            {
                UIPersonalizationEditor editorUi = ModUIConstants.ShowPersonalizationEditorUI();

                GameObject cameraObject = Instantiate(PlayerCameraManager.Instance.DefaultGameCameraPrefab.gameObject);
                cameraObject.tag = "MainCamera";
                cameraObject.transform.position = new Vector3(-2.5f, 3f, 3f);
                cameraObject.transform.eulerAngles = new Vector3(5f, 120f, 0f);
                PersonalizationEditorCamera personalizationEditorCamera = cameraObject.AddComponent<PersonalizationEditorCamera>();
                personalizationEditorCamera.ToolBarTransform = editorUi.ToolBarTransform;

                LevelEditorLevelData levelEditorLevelData = null;
                try
                {
                    levelEditorLevelData = ModJsonUtils.DeserializeStream<LevelEditorLevelData>(Path.Combine(ModCore.dataFolder, "levels/personalizationEditorLevel.json"));
                }
                catch
                {
                }

                _ = base.StartCoroutine(spawnLevelCoroutine(useTransitionManager, levelEditorLevelData));
            });
            yield break;
        }

        public List<Dropdown.OptionData> GetConditionOptions()
        {
            if (ModAdvancedCache.TryGet("DropdownShowConditionOptions", out List<Dropdown.OptionData> list))
                return list;

            list = new List<Dropdown.OptionData>
            {
                new DropdownShowConditionOptionData(PersonalizationEditorObjectShowConditions.IsNormal),
                new DropdownShowConditionOptionData(PersonalizationEditorObjectShowConditions.IsOnFire),
                new DropdownShowConditionOptionData(PersonalizationEditorObjectShowConditions.IsNormalMultiplayer),
                new DropdownShowConditionOptionData(PersonalizationEditorObjectShowConditions.IsOnFireMultiplayer)
            };
            ModAdvancedCache.Add("DropdownShowConditionOptions", list);
            return list;
        }

        public bool CreateItem(string name, bool usePersistentFolder, out PersonalizationItemInfo personalizationItem)
        {
            personalizationItem = null;
            if (name.IsNullOrEmpty())
                return false;

            string rootDirectory = usePersistentFolder ? ModCore.customizationPersistentFolder : ModCore.customizationFolder;
            string directoryName = name.Replace(" ", string.Empty);
            string directoryPath = $"{Path.Combine(rootDirectory, directoryName)}/";
            string filesDirectoryPath = Path.Combine(directoryPath, "files/");

            if (Directory.Exists(directoryPath))
                return false;
            else
                _ = Directory.CreateDirectory(directoryPath);

            if (!Directory.Exists(filesDirectoryPath))
                _ = Directory.CreateDirectory(filesDirectoryPath);

            personalizationItem = new PersonalizationItemInfo()
            {
                Name = name,
                Description = "No description provided.",
                IsVerified = false,
                Category = PersonalizationCategory.WeaponSkins,
                EditorID = PersonalizationEditorManager.Instance.editorId,
                ItemID = Guid.NewGuid().ToString(),
                FolderPath = directoryPath,
                RootFolderPath = rootDirectory,
                RootFolderName = usePersistentFolder ? ModCore.CUSTOMIZATION_PERSISTENT_FOLDER_NAME : ModCore.CUSTOMIZATION_FOLDER_NAME,
                IsPersistentAsset = usePersistentFolder
            };
            personalizationItem.FixValues();
            personalizationItem.SetAuthor(SteamFriends.GetPersonaName());
            PersonalizationManager.Instance.itemList.Items.Add(personalizationItem);

            ModJsonUtils.WriteStream(directoryPath + PersonalizationEditorManager.ITEM_INFO_FILE, personalizationItem);
            return true;
        }

        public void EditItem(PersonalizationItemInfo personalizationItemInfo, string folder)
        {
            currentEditingItemInfo = personalizationItemInfo;
            UIPersonalizationEditor.instance.Inspector.Populate(personalizationItemInfo);
            SpawnRootObject();
        }

        public bool SaveItem(out string error)
        {
            if (currentEditingItemInfo == null)
            {
                error = "Editing item info is NULL";
                return false;
            }

            if (!currentEditingRoot)
            {
                error = "Editing item is NULL";
                return false;
            }

            string folder = currentEditingItemFolder;
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
                ModJsonUtils.WriteStream(Path.Combine(folder, ITEM_INFO_FILE), currentEditingItemInfo);
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
            currentEditingItemInfo.RootObject = currentEditingRoot.Serialize();
        }

        public void SpawnBot()
        {
            _ = base.StartCoroutine(spawnBotCoroutine());
        }

        private IEnumerator spawnBotCoroutine()
        {
            PersonalizationController personalizationController = currentPersonalizationController;
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

                BoltEntity boltEntity = bot.GetComponent<BoltEntity>();
                if (boltEntity)
                {
                    bot._hasEverHadLocalControl = false;
                    bot._hasLocalControl = false;
                    boltEntity.ReleaseControl();
                }
            }, 0.5f);

            Destroy(spawnPoint);
            yield break;
        }

        private IEnumerator spawnLevelCoroutine(bool useTransitionManager, LevelEditorLevelData levelEditorLevelData)
        {
            if (levelEditorLevelData != null)
            {
                GameObject level = new GameObject();
                LevelManager.Instance._currentLevelHidesTheArena = true;
                _ = LevelEditorDataManager.Instance.DeserializeInto(level.transform, levelEditorLevelData).MoveNext();
            }
            else
            {
                LevelManager.Instance._currentLevelHidesTheArena = false;
            }
            ArenaLiftManager.Instance.SetToArena();
            GlobalEventManager.Instance.Dispatch(GlobalEvents.LevelSpawned);
            SpawnBot();

            if (useTransitionManager)
            {
                yield return new WaitForSecondsRealtime(1f);
                TransitionManager.Instance.EndTransition();
            }
            WelcomeMessage();

            yield break;
        }

        public void SpawnRootObject()
        {
            PersonalizationEditorObjectInfo rootInfo = currentEditingItemInfo?.RootObject;
            if (rootInfo == null)
                return;

            PersonalizationController personalizationController = currentPersonalizationController;
            if (!personalizationController)
                return;

            personalizationController.DestroyAllItems();
            currentEditingRoot = personalizationController.SpawnItem(currentEditingItemInfo);
            PersonalizationEditorObjectManager.Instance.SetCurrentRootNextUniqueIndex(rootInfo.NextUniqueIndex);
        }

        public void ExportItem(PersonalizationItemInfo personalizationItemInfo, out string destination, string overrideDirectoryPath = null)
        {
            string fn = $"PersonalizationItem_{personalizationItemInfo.ItemID.ToString().Replace("-", string.Empty)}.zip";
            string folder = overrideDirectoryPath.IsNullOrEmpty() ? ModDataManager.Instance.savesFolder : overrideDirectoryPath;
            destination = Path.Combine(folder, fn);

            if (File.Exists(destination))
                File.Delete(destination);

            FastZip fastZip = new FastZip();
            fastZip.CreateZip(destination, personalizationItemInfo.FolderPath, true, null);
        }

        public void WelcomeMessage()
        {
            UIPersonalizationEditor.instance.Dropdown.Hide();
            ModUIUtils.MessagePopupOK("Welcome to customization editor!", "Here you can make weapon skins, accessories and pets." +
                "\n\n<color=#FFFFFF><size=14>HOW TO MAKE STUFF?</size></color>" +
                "\nTo open or create a project, click on 'File' at the top left and click on 'Open'." +
                "\n\n<color=#FFFFFF><size=14>HOW TO UPLOAD STUFF?</size></color>" +
                "\nTo upload your project, click on 'File' at the top left and click on 'Upload'." +
                "\nOnce you upload an item, you'll have to wait until it's verified and when it is, customization assets will get an update." +
                "\n\n<color=#FFCB23>This editor is still in development, so you can experience some issues while editing!</color>", 400f, true);
        }

        public void TutorialVideo()
        {
            UIPersonalizationEditor.instance.Dropdown.Hide();
            Application.OpenURL("https://google.com"); // todo: make tutorial video and add it here
        }

        public List<ColorPairFloat> GetColorPairsFromString(string dataString)
        {
            if (dataString.IsNullOrEmpty())
                return null;

            string[] split = dataString.Split('|');
            if (split.IsNullOrEmpty())
                return null;

            List<ColorPairFloat> list = new List<ColorPairFloat>();
            foreach (string oldAndNewColorsString in split)
            {
                if (oldAndNewColorsString.IsNullOrEmpty())
                    continue;

                string[] oldAndNewColors = oldAndNewColorsString.Split('-');
                if (oldAndNewColors.Length == 2)
                {
                    Color a = ModParseUtils.TryParseToColor(oldAndNewColors[0], Color.white);
                    Color b = ModParseUtils.TryParseToColor(oldAndNewColors[1], Color.white);
                    list.Add(new ColorPairFloat(a, b));
                }
            }
            return list;
        }

        public string GetStringFromColorPairs(List<ColorPairFloat> colorPairs)
        {
            if (colorPairs.IsNullOrEmpty())
                return null;

            int index = 0;

            StringBuilder stringBuilder = new StringBuilder();
            foreach (ColorPairFloat cp in colorPairs)
            {
                string colorA = ColorUtility.ToHtmlStringRGBA(cp.ColorA);
                string colorB = ColorUtility.ToHtmlStringRGBA(cp.ColorB);
                string colorsString = $"{colorA}-{colorB}".Replace("#", string.Empty);
                _ = stringBuilder.Append(colorsString);
                if (index + 1 != colorPairs.Count)
                    _ = stringBuilder.Append('|');

                index++;
            }
            return stringBuilder.ToString();
        }
    }
}
