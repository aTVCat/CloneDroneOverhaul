using ICSharpCode.SharpZipLib.Zip;
using OverhaulMod.Combat;
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
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorManager : Singleton<PersonalizationEditorManager>
    {
        public const string ITEM_INFO_FILE = "itemInfo.json";

        public const string ITEM_META_DATA_FILE = "metaData.json";

        public const string EDITOR_STARTED_EVENT = "PersonalizationEditorStarted";

        public const string OBJECT_EDITED_EVENT = "PersonalizationEditorObjectEdited";

        public const string PRESET_PREVIEW_CHANGED_EVENT = "PersonalizationEditorPresetPreviewChanged";

        public const GameMode GAME_MODE_VALUE = (GameMode)2500;

        [ModSetting(ModSettingsConstants.CUSTOMIZATION_EDITOR_AMBIANCE, true)]
        public static bool EditorAmbiance;

        public readonly GameData GameData = new GameData();

        private bool _hasConfiguredGameData;

        private bool _isInPlaytestMode;

        private bool _isInScreenshotMode;

        private Color _ambientColorBeforeScreenshotMode;

        private AmbientMode _ambientModeBeforeScreenshotMode;

        private GreatSwordPreviewController _greatSwordPreviewController;

        private FirstPersonMover _bot;

        private PersonalizationEditorCamera _camera;

        private PersonalizationItemInfo _editingItemBeforeScreenshoting;

        public PersonalizationController currentPersonalizationController
        {
            get;
            set;
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

        private bool _originalModelsEnabled;
        public bool originalModelsEnabled
        {
            get
            {
                return _originalModelsEnabled;
            }
            set
            {
                _originalModelsEnabled = value;
                RefreshGreatswordPreview();
            }
        }

        private WeaponVariant2 _previewPresetKey;
        public WeaponVariant2 previewPresetKey
        {
            get
            {
                return _previewPresetKey;
            }
            set
            {
                _previewPresetKey = value;
                RefreshGreatswordPreview();
            }
        }

        private string _editorId;
        public string editorId
        {
            get
            {
                if (!SteamManager.Instance || !SteamManager.Instance.Initialized)
                    return null;

                if (_editorId == null)
                {
                    _editorId = SteamUser.GetSteamID().ToString();
                }
                return _editorId;
            }
        }

        public bool canVerifyItems => ExclusivePerkManager.Instance.IsLocalUserAbleToVerifyItems();

        public void StartEditorGameMode(bool noTransition = false)
        {
            if (noTransition || !TransitionManager.OverhaulNonSceneTransitions)
            {
                _ = base.StartCoroutine(startEditorGameModeCoroutine(false));
                return;
            }
            TransitionManager.Instance.DoNonSceneTransition(startEditorGameModeCoroutine(true));
        }

        private IEnumerator startEditorGameModeCoroutine(bool useTransitionManager)
        {
            AudioManager.Instance.FadeOutMusic(1f);

            if (EditorAmbiance)
                ModAudioManager.Instance.PlayCustomizationEditorAmbiance();

            if (useTransitionManager)
                yield return new WaitForSecondsRealtime(0.25f);

            yield return null;

            if (!_hasConfiguredGameData)
            {
                GameData gameData = GameData;
                gameData.HumanFacts = HumanFactsManager.Instance.GetRandomFactSet();
                gameData.PlayerUpgrades = new Dictionary<UpgradeType, int>()
                {
                    { UpgradeType.SwordUnlock, 1 },
                    { UpgradeType.BowUnlock, 1 },
                    { UpgradeType.Hammer, 3 },
                    { UpgradeType.SpearUnlock, 1 },
                    { UpgradeType.Dash, 1 },
                    { UpgradeType.EnergyCapacity, 2 },
                    { UpgradeType.EnergyRecharge, 2 },
                };
                _hasConfiguredGameData = true;
            }

            currentEditingItemInfo = null;
            currentEditingRoot = null;
            previewPresetKey = WeaponVariant2.Normal;
            originalModelsEnabled = false;

            _isInScreenshotMode = false;
            _isInPlaytestMode = false;

            GameFlowManager.Instance._gameMode = GAME_MODE_VALUE;

            LevelManager.Instance.CleanUpLevelThisFrame();
            GameFlowManager.Instance.HideTitleScreen(false);

            GameDataManager.Instance.SaveHighScoreDataWithoutModifyingIt();
            CacheManager.Instance.CreateOrClearInstance();
            GarbageManager.Instance.DestroyAllGarbage();

            PersonalizationEditorTemplateManager.Instance.LoadTemplates();

            SingleplayerServerStarter.Instance.StartServerThenCall(delegate
            {
                UIPersonalizationEditor editorUi = ModUIConstants.ShowPersonalizationEditorUI();

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

        public bool CreateItem(string directoryName, string name, string uniqueId, bool usePersistentFolder, PersonalizationItemInfo templateSource, out PersonalizationItemInfo personalizationItem)
        {
            string rootDirectory = usePersistentFolder ? ModCore.customizationPersistentFolder : ModCore.customizationFolder;
            string directoryPath = Path.Combine(rootDirectory, directoryName);
            string filesDirectoryPath = Path.Combine(directoryPath, "files");

            personalizationItem = null;
            if (Directory.Exists(directoryPath))
                return false;
            else
                _ = Directory.CreateDirectory(directoryPath);

            if (!Directory.Exists(filesDirectoryPath))
                _ = Directory.CreateDirectory(filesDirectoryPath);

            bool useTemplate = true;
            if (templateSource != null)
            {
                try
                {
                    personalizationItem = ModJsonUtils.Deserialize<PersonalizationItemInfo>(ModJsonUtils.Serialize(templateSource));

                    personalizationItem.Name = name;
                    personalizationItem.Description = "No description provided.";
                    personalizationItem.IsVerified = false;
                    personalizationItem.EditorID = Instance.editorId;
                    personalizationItem.ItemID = uniqueId;
                    personalizationItem.FolderPath = directoryPath;
                    personalizationItem.RootFolderPath = rootDirectory;
                    personalizationItem.RootFolderName = usePersistentFolder ? ModCore.CUSTOMIZATION_PERSISTENT_FOLDER_NAME : ModCore.CUSTOMIZATION_FOLDER_NAME;
                    personalizationItem.IsPersistentAsset = usePersistentFolder;
                    personalizationItem.MetaData = new PersonalizationItemMetaData()
                    {
                        CustomizationSystemVersion = PersonalizationItemMetaData.CurrentCustomizationSystemVersion,
                    };
                }
                catch
                {
                    useTemplate = false;
                }
            }
            else
                useTemplate = false;

            if (!useTemplate)
            {
                personalizationItem = new PersonalizationItemInfo()
                {
                    Name = name,
                    Description = "No description provided.",
                    IsVerified = false,
                    Category = PersonalizationCategory.WeaponSkins,
                    EditorID = Instance.editorId,
                    ItemID = uniqueId,
                    FolderPath = directoryPath,
                    RootFolderPath = rootDirectory,
                    RootFolderName = usePersistentFolder ? ModCore.CUSTOMIZATION_PERSISTENT_FOLDER_NAME : ModCore.CUSTOMIZATION_FOLDER_NAME,
                    IsPersistentAsset = usePersistentFolder,
                    MetaData = new PersonalizationItemMetaData()
                    {
                        CustomizationSystemVersion = PersonalizationItemMetaData.CurrentCustomizationSystemVersion,
                    }
                };
            }

            personalizationItem.FixValues();
            personalizationItem.SetAuthor(SteamFriends.GetPersonaName());
            PersonalizationManager.Instance.itemList.Items.Add(personalizationItem);

            ModJsonUtils.WriteStream(Path.Combine(directoryPath, ITEM_INFO_FILE), personalizationItem);
            ModJsonUtils.WriteStream(Path.Combine(directoryPath, ITEM_META_DATA_FILE), personalizationItem.MetaData);
            return true;
        }

        public void EditItem(PersonalizationItemInfo personalizationItemInfo)
        {
            currentEditingItemInfo = personalizationItemInfo;

            if (personalizationItemInfo != null)
            {
                UIElementPersonalizationEditorUtilitiesPanel utils = UIPersonalizationEditor.instance.Utilities;
                utils.Show();
                utils.SetConditionOptions(GetConditionOptionsDependingOnEditingWeapon());

                UIPersonalizationEditor.instance.Inspector.Populate(personalizationItemInfo);
                SpawnRootObject();

                UIPersonalizationEditor.instance.ShowNotification("Success", $"Loaded the item ({personalizationItemInfo.Name})", UIElementPersonalizationEditorNotification.SuccessColor);
            }
            else
            {
                PersonalizationController personalizationController = currentPersonalizationController;
                if (personalizationController) personalizationController.DestroyAllItems();
            }
        }

        public bool SaveItem(out string error, bool ignoreDevPanel = false)
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

            PersonalizationItemMetaData personalizationItemMetaData = currentEditingItemInfo.MetaData;
            if (personalizationItemMetaData == null)
            {
                personalizationItemMetaData = new PersonalizationItemMetaData
                {
                    CustomizationSystemVersion = PersonalizationItemMetaData.CurrentCustomizationSystemVersion
                };
            }

            UIPersonalizationEditor.instance.Inspector.ApplyValues(ignoreDevPanel);
            SerializeRoot();
            try
            {
                ModJsonUtils.WriteStream(Path.Combine(folder, ITEM_INFO_FILE), currentEditingItemInfo);
                ModJsonUtils.WriteStream(Path.Combine(folder, ITEM_META_DATA_FILE), personalizationItemMetaData);
            }
            catch (Exception exc)
            {
                error = exc.ToString();
                return false;
            }
            error = null;
            return true;
        }

        public void ImportItem(string path, string itemFolderName, out string error, bool editItem = false)
        {
            error = null;

            string folderPath = Path.Combine(ModCore.customizationFolder, itemFolderName);
            _ = Directory.CreateDirectory(folderPath);

            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(path, folderPath, null);

            PersonalizationItemList itemList = PersonalizationManager.Instance.itemList;
            PersonalizationItemInfo info;
            try
            {
                info = itemList.LoadItemInfo(folderPath);
            }
            catch (Exception exc)
            {
                error = exc.ToString();
                return;
            }

            /*
            foreach (PersonalizationEditorObjectInfo child in info.RootObject.Children)
            {
                if (child.Path == "Volume")
                {
                    if (child.PropertyValues.TryGetValue(nameof(PersonalizationEditorObjectVolume.volumeSettingPresets), out object obj) && obj is Dictionary<WeaponVariant2, VolumeSettingsPreset> dictionary && !dictionary.IsNullOrEmpty())
                    {
                        foreach (VolumeSettingsPreset value in dictionary.Values)
                        {
                            string voxFilePath = value.VoxFilePath;
                            if (!voxFilePath.IsNullOrEmpty() && !voxFilePath.StartsWith(itemFolderName))
                            {
                                string sub = voxFilePath.Substring(voxFilePath.IndexOf(Path.DirectorySeparatorChar) + 1);
                                voxFilePath = $"{itemFolderName}{Path.DirectorySeparatorChar}{sub}";
                                value.VoxFilePath = voxFilePath;
                            }
                        }
                    }
                }
                else if (child.Path == "CvmModel")
                {
                    if (child.PropertyValues.TryGetValue(nameof(PersonalizationEditorObjectCVMModel.presets), out object obj) && obj is Dictionary<WeaponVariant2, CVMModelPreset> dictionary && !dictionary.IsNullOrEmpty())
                    {
                        foreach (CVMModelPreset value in dictionary.Values)
                        {
                            string cvmFilePath = value.CvmFilePath;
                            if (!cvmFilePath.IsNullOrEmpty() && !cvmFilePath.StartsWith(itemFolderName))
                            {
                                string sub = cvmFilePath.Substring(cvmFilePath.IndexOf(Path.DirectorySeparatorChar) + 1);
                                cvmFilePath = $"{itemFolderName}{Path.DirectorySeparatorChar}{sub}";
                                value.CvmFilePath = cvmFilePath;
                            }
                        }
                    }
                }
            }*/

            Action finalAction = delegate
            {
                itemList.Items.Add(info);

                if (editItem)
                {
                    UIPersonalizationEditor.instance.ShowEverything();
                    EditItem(info);
                }
            };

            PersonalizationItemInfo existingItem = itemList.GetItem(info.ItemID);
            if (existingItem != null)
            {
                ModUIUtils.MessagePopup(true, "An item with the same ID has been already imported!", "Do you want to replace the old version with the new one?", 150f, MessageMenu.ButtonLayout.EnableDisableButtons, "Ok", "Yes", "No", null, delegate
                {
                    if (Path.GetFullPath(existingItem.FolderPath) == Path.GetFullPath(info.FolderPath))
                    {
                        ModUIUtils.MessagePopupOK("Both items have been in the same folder", "Just a notification");
                    }
                    else
                    {
                        Directory.Delete(existingItem.FolderPath, true);
                    }
                    _ = itemList.Items.Remove(existingItem);
                    finalAction();
                });
            }
            else
            {
                finalAction();
            }
        }

        public void ExportItem(PersonalizationItemInfo personalizationItemInfo, out string destination, string overrideDirectoryPath = null, string overrideFn = null)
        {
            string fn = overrideFn.IsNullOrEmpty() ? $"{Path.GetFileName(personalizationItemInfo.FolderPath)}.zip" : overrideFn;
            string folder = overrideDirectoryPath.IsNullOrEmpty() ? ModDataManager.savesFolder : overrideDirectoryPath;
            destination = Path.Combine(folder, fn);

            if (File.Exists(destination))
                File.Delete(destination);

            FastZip fastZip = new FastZip();
            fastZip.CreateZip(destination, personalizationItemInfo.FolderPath, true, null);
        }

        public void SerializeRoot()
        {
            currentEditingItemInfo.RootObject = currentEditingRoot.Serialize();
        }

        public void SerializeRotAndRespawnBot()
        {
            SerializeRoot();
            BoltNetwork.Destroy(_bot.gameObject);
            SpawnBot(true);
        }

        public void SpawnBot(bool spawnEditingItem)
        {
            _ = base.StartCoroutine(spawnBotCoroutine(spawnEditingItem));
        }

        private IEnumerator spawnBotCoroutine(bool spawnEditingItem)
        {
            PersonalizationController personalizationController = currentPersonalizationController;
            if (personalizationController)
            {
                Destroy(personalizationController.gameObject);
            }

            GameObject spawnPoint = new GameObject();
            spawnPoint.transform.position = Vector3.zero;

            CloneSpawningData cloneSpawningData = new CloneSpawningData(spawnPoint.transform, true, false, UIPersonalizationEditor.instance.Utilities.GetFavoriteColor(), null);

            CloneSpawner cloneSpawner = GameFlowManager.Instance._cloneSpawner;
            cloneSpawner.UseSkinInSingleplayer = false;

            FirstPersonMover bot = cloneSpawner.SpawnClone(cloneSpawningData);
            bot._upgradeCollection._upgradeLevels = new Dictionary<UpgradeType, int>();
            bot._upgradeCollection.AddUpgradeIfMissing(UpgradeType.SwordUnlock, 1);
            bot._upgradeCollection.AddUpgradeIfMissing(UpgradeType.BowUnlock, 1);
            bot._upgradeCollection.AddUpgradeIfMissing(UpgradeType.Hammer, 3);
            bot._upgradeCollection.AddUpgradeIfMissing(UpgradeType.SpearUnlock, 1);
            bot._upgradeCollection.AddUpgradeIfMissing(UpgradeType.EnergyCapacity, 2);
            bot._upgradeCollection.AddUpgradeIfMissing(UpgradeType.Dash, 1);
            bot._upgradeCollection.AddUpgradeIfMissing(ModUpgradesManager.SCYTHE_UNLOCK_UPGRADE, 1);
            bot.transform.eulerAngles = Vector3.up * 90f;
            if (bot._playerCamera)
                bot._playerCamera.gameObject.SetActive(false);

            _bot = bot;
            _greatSwordPreviewController = bot.gameObject.AddComponent<GreatSwordPreviewController>();

            DelegateScheduler.Instance.Schedule(delegate
            {
                BoltEntity boltEntity = bot.GetComponent<BoltEntity>();
                if (boltEntity)
                {
                    bot._hasEverHadLocalControl = false;
                    bot._hasLocalControl = false;
                    boltEntity.ReleaseControl();
                }

                if (spawnEditingItem)
                {
                    bot.SetEquippedWeaponType(currentEditingItemInfo.Weapon, false);
                    SpawnRootObject();
                }

            }, 0.2f);

            Destroy(spawnPoint);
            yield break;
        }

        public FirstPersonMover GetBot()
        {
            return _bot;
        }

        private IEnumerator spawnLevelCoroutine(bool useTransitionManager, LevelEditorLevelData levelEditorLevelData)
        {
            yield return null;

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
            SpawnBot(false);
            GlobalEventManager.Instance.Dispatch(EDITOR_STARTED_EVENT);

            GameObject cameraObject = Instantiate(PlayerCameraManager.Instance.DefaultGameCameraPrefab.gameObject);
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(-2.5f, 3f, 3f);
            cameraObject.transform.eulerAngles = new Vector3(5f, 120f, 0f);
            _camera = cameraObject.AddComponent<PersonalizationEditorCamera>();

            if (useTransitionManager)
            {
                yield return new WaitForSecondsRealtime(1f);
                TransitionManager.Instance.EndTransition();
            }
            WelcomeMessage();

            ArenaCameraManager.Instance.HideTitleScreenCamera();
            ArenaCameraManager.Instance.TurnOffArenaCamera();

            yield break;
        }

        public void SpawnRootObject()
        {
            PersonalizationItemInfo info = currentEditingItemInfo;
            if (info == null)
                return;

            PersonalizationEditorObjectInfo rootInfo = info.RootObject;
            if (rootInfo == null)
            {
                rootInfo = new PersonalizationEditorObjectInfo()
                {
                    Name = "Root",
                    Path = "Empty",
                    IsRoot = true,
                    Children = new List<PersonalizationEditorObjectInfo>(),
                    PropertyValues = new Dictionary<string, object>()
                };
                currentEditingItemInfo.RootObject = rootInfo;
            }

            PersonalizationController personalizationController = currentPersonalizationController;
            if (!personalizationController)
                return;

            personalizationController.DestroyAllItems();

            currentEditingRoot = personalizationController.SpawnItem(currentEditingItemInfo);
            PersonalizationEditorObjectManager.Instance.SetCurrentRootNextUniqueIndex(rootInfo.NextUniqueIndex);
        }

        public void EnterPlaytestMode()
        {
            if (_isInPlaytestMode) return;

            _isInPlaytestMode = true;

            FirstPersonMover firstPersonMover = _bot;
            if (firstPersonMover)
            {
                firstPersonMover.GetComponent<BoltEntity>().TakeControl();

                firstPersonMover.SetCameraHolderEnabled(true);
                firstPersonMover.SetPlayerCameraEnabled(true);
                firstPersonMover.SetCameraAnimatorEnabled(true);

                _camera.gameObject.SetActive(false);

                UIPersonalizationEditor.instance.Hide();
                _ = ModUIConstants.ShowPersonalizationEditorPlaytestHUD();
            }
        }

        public void ExitPlaytestMode()
        {
            if (!_isInPlaytestMode) return;

            _isInPlaytestMode = false;

            FirstPersonMover firstPersonMover = _bot;
            if (firstPersonMover)
            {
                firstPersonMover.SetCameraAnimatorEnabled(false);
                firstPersonMover.SetPlayerCameraEnabled(true);
                firstPersonMover.SetCameraHolderEnabled(true);
                firstPersonMover.ResetInputKeys();
                firstPersonMover.InstantlySetTorsoTiltX(0f);
                firstPersonMover.SetIsJumpingBools(false);
                firstPersonMover.SetIsMovingBools(false);

                PersonalizationEditorCamera camera = _camera;
                camera.transform.position = new Vector3(-2.5f, 3f, 3f);
                camera.transform.eulerAngles = new Vector3(5f, 120f, 0f);
                camera.gameObject.SetActive(true);

                UIPersonalizationEditor.instance.Show();
                ModUIConstants.HidePersonalizationEditorPlaytestHUD();

                _ = base.StartCoroutine(exitPlaytestModeCoroutine(firstPersonMover));
            }
        }

        private IEnumerator exitPlaytestModeCoroutine(FirstPersonMover firstPersonMover)
        {
            ModTime modTime = ModTime.Instance;

            int ffc = modTime.GetFixedFrameCount();
            while (modTime.GetFixedFrameCount() < ffc + 2)
                yield return null;

            if (firstPersonMover)
            {
                firstPersonMover.SetEquippedWeaponType(currentEditingItemInfo.Weapon, false);
                firstPersonMover.GetComponent<BoltEntity>().ReleaseControl();
                firstPersonMover.transform.position = Vector3.zero;
                firstPersonMover.transform.eulerAngles = Vector3.up * 90f;
                firstPersonMover.stopAirCleavingIfActive();
            }

            yield break;
        }

        public bool IsInPlaytestMode()
        {
            return _isInPlaytestMode;
        }

        public void EnterScreenshotMode()
        {
            if (_isInScreenshotMode) return;

            if (currentEditingItemInfo != null && !SaveItem(out string error))
            {
                UIPersonalizationEditor.instance.ShowSaveErrorMessage(error);
                return;
            }

            _isInScreenshotMode = true;

            _editingItemBeforeScreenshoting = currentEditingItemInfo;
            EditItem(null);

            _ambientColorBeforeScreenshotMode = RenderSettings.ambientLight;
            _ambientModeBeforeScreenshotMode = RenderSettings.ambientMode;
            RenderSettings.ambientLight = Color.white * 0.8f;
            RenderSettings.ambientMode = AmbientMode.Flat;

            _camera.gameObject.SetActive(false);

            UIPersonalizationEditor.instance.Hide();

            PersonalizationEditorScreenshotManager stage = PersonalizationEditorScreenshotManager.Instance;
            stage.ShowStage();
            stage.ShowOverlay();
            stage.SpawnItemInHolder(_editingItemBeforeScreenshoting);
            stage.AdjustCameraPositionForCurrentItem();

            ModUIConstants.ShowPersonalizationEditorScreenshotControls();

            PersonalizationEditorCamera cameraController = stage.GetCameraController();
            cameraController.gameObject.SetActive(true);

            _ = ModUIConstants.ShowPersonalizationEditorPlaytestHUD();
        }

        public void ExitScreenshotMode()
        {
            if (!_isInScreenshotMode) return;
            _isInScreenshotMode = false;

            RenderSettings.ambientLight = _ambientColorBeforeScreenshotMode;
            RenderSettings.ambientMode = _ambientModeBeforeScreenshotMode;

            _camera.gameObject.SetActive(true);

            UIPersonalizationEditor.instance.Show();

            PersonalizationEditorScreenshotManager stage = PersonalizationEditorScreenshotManager.Instance;
            stage.DestroyStageItem();
            stage.HideStage();
            stage.HideOverlay();

            ModUIConstants.HidePersonalizationEditorScreenshotControls();

            EditItem(_editingItemBeforeScreenshoting);

            PersonalizationEditorCamera cameraController = stage.GetCameraController();
            cameraController.gameObject.SetActive(false);

            ModUIConstants.HidePersonalizationEditorPlaytestHUD();
        }

        public bool IsInScreenshotMode()
        {
            return _isInScreenshotMode;
        }

        public void RefreshGreatswordPreview()
        {
            if (_greatSwordPreviewController)
                _greatSwordPreviewController.SetPreviewActivate(originalModelsEnabled && (previewPresetKey == WeaponVariant2.NormalMultiplayer || previewPresetKey == WeaponVariant2.OnFireMultiplayer));
        }

        public List<Dropdown.OptionData> GetConditionOptions()
        {
            if (ModAdvancedCache.TryGet("DropdownShowConditionOptions", out List<Dropdown.OptionData> list))
                return list;

            list = new List<Dropdown.OptionData>
            {
                new DropdownWeaponVariantOptionData(WeaponVariant2.Normal),
                new DropdownWeaponVariantOptionData(WeaponVariant2.OnFire),
                new DropdownWeaponVariantOptionData(WeaponVariant2.NormalMultiplayer),
                new DropdownWeaponVariantOptionData(WeaponVariant2.OnFireMultiplayer)
            };
            ModAdvancedCache.Add("DropdownShowConditionOptions", list);
            return list;
        }

        public List<Dropdown.OptionData> GetConditionOptionsDependingOnEditingWeapon(bool includeNone = false)
        {
            WeaponType weaponType = currentEditingItemInfo.Weapon;

            List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
            if (includeNone)
                list.Add(new DropdownWeaponVariantOptionData(WeaponVariant2.None));

            list.Add(new DropdownWeaponVariantOptionData(WeaponVariant2.Normal));

            if (weaponType == WeaponType.Sword)
            {
                list.Add(new DropdownWeaponVariantOptionData(WeaponVariant2.OnFire));
                list.Add(new DropdownWeaponVariantOptionData(WeaponVariant2.NormalMultiplayer));
                list.Add(new DropdownWeaponVariantOptionData(WeaponVariant2.OnFireMultiplayer));
            }
            else if (weaponType == WeaponType.Hammer || weaponType == WeaponType.Spear || weaponType == ModWeaponsManager.SCYTHE_TYPE)
            {
                list.Add(new DropdownWeaponVariantOptionData(WeaponVariant2.OnFire));
            }

            return list;
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
                "\n\n<color=#FFCB23>This editor is still in development, so you can experience issues while editing!</color>", 400f, true);
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

        public static bool IsInEditor()
        {
            return GameModeManager.Is(GAME_MODE_VALUE);
        }
    }
}
