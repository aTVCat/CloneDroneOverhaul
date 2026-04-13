using OverhaulMod.Combat;
using OverhaulMod.Engine;
using OverhaulMod.UI;
using OverhaulMod.Utils;
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
        public const string EDITOR_STARTED_EVENT = "PersonalizationEditorStarted";

        public const string OBJECT_EDITED_EVENT = "PersonalizationEditorObjectEdited";

        public const string PRESET_PREVIEW_CHANGED_EVENT = "PersonalizationEditorPresetPreviewChanged";

        public const GameMode GAME_MODE_VALUE = (GameMode)2500;

        [ModSetting(ModSettingsConstants.CUSTOMIZATION_EDITOR_AMBIANCE, true)]
        public static bool PlayAmbiance;

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

        public string EditorID
        {
            get => ModUserInfo.localPlayerSteamID.ToString();
        }

        public bool CanVerifyItems => ExclusivePerkManager.Instance.IsLocalUserAbleToVerifyItems();

        public PersonalizationController PreviewingPersonalizationController
        {
            get;
            set;
        }

        public PersonalizationItemInfo EditingItemInfo
        {
            get;
            set;
        }

        public PersonalizationEditorObjectBehaviour EditingRoot
        {
            get;
            set;
        }

        public string EditingItemFolder
        {
            get => EditingItemInfo.FolderPath;
        }

        private bool _viewingOriginalModel;
        public bool ViewingOriginalModel
        {
            get => _viewingOriginalModel;
            set
            {
                _viewingOriginalModel = value;
                RefreshGreatswordPreview();
                if (PreviewingPersonalizationController) PreviewingPersonalizationController.RefreshWeaponSkinsNextFrame();
            }
        }

        private WeaponVariant2 _previewPresetKey;
        public WeaponVariant2 PreviewPresetKey
        {
            get => _previewPresetKey;
            set
            {
                _previewPresetKey = value;
                RefreshGreatswordPreview();
            }
        }

        public static bool IsInEditorMode() => GameModeManager.Is(GAME_MODE_VALUE);

        public void StartEditorGameMode(bool noTransition = false)
        {
            if (noTransition || !TransitionManager.OverhaulSceneTransitions)
            {
                _ = base.StartCoroutine(startEditorGameModeCoroutine(false));
                return;
            }
            TransitionManager.Instance.DoInGameTransition(startEditorGameModeCoroutine(true));
        }

        private IEnumerator startEditorGameModeCoroutine(bool useTransitionManager)
        {
            AudioManager.Instance.FadeOutMusic(1f);

            if (PlayAmbiance) ModAudioManager.Instance.PlayCustomizationEditorAmbiance();

            if (useTransitionManager) yield return new WaitForSecondsRealtime(1f);
            else yield return null;

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
            GameFlowManager.Instance._gameMode = GAME_MODE_VALUE;

            EditingItemInfo = null;
            EditingRoot = null;
            PreviewPresetKey = WeaponVariant2.Normal;
            ViewingOriginalModel = false;

            _isInScreenshotMode = false;
            _isInPlaytestMode = false;

            GarbageManager.Instance.DestroyAllGarbage();
            LevelManager.Instance.CleanUpLevelThisFrame();
            CacheManager.Instance.CreateOrClearInstance();
            GameFlowManager.Instance.HideTitleScreen(false);
            ArenaCameraManager.Instance.HideTitleScreenCamera();
            ArenaCameraManager.Instance.TurnOffArenaCamera();

            PersonalizationEditorTemplateManager.Instance.LoadTemplates();

            SingleplayerServerStarter.Instance.StartServerThenCall(delegate
            {
                StartCoroutine(spawnLevelAndFinishInitializationCoroutine(useTransitionManager));
            });
            yield break;
        }

        private IEnumerator spawnLevelAndFinishInitializationCoroutine(bool useTransitionManager)
        {
            yield return null;

            LevelEditorLevelData levelEditorLevelData;
            string levelFilePath = Path.Combine(ModCore.DataFolder, "levels/personalizationEditorLevel.json");
            if (File.Exists(levelFilePath))
            {
                try
                {
                    levelEditorLevelData = ModJsonUtils.DeserializeStream<LevelEditorLevelData>(levelFilePath);
                }
                catch
                {
                    levelEditorLevelData = null;
                }
            }
            else
            {
                levelEditorLevelData = null;
            }

            LevelManager.Instance._currentLevelHidesTheArena = levelEditorLevelData != null;
            if (levelEditorLevelData != null) yield return StartCoroutine(LevelEditorDataManager.Instance.DeserializeInto(new GameObject("Personalization Editor Room Level").transform, levelEditorLevelData, true));
            else
            {
                ArenaLiftManager.Instance.SetToArena();
            }
            GlobalEventManager.Instance.Dispatch(GlobalEvents.LevelSpawned);

            ModUIConstants.ShowPersonalizationEditorUI();

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
            yield break;
        }

        public void EditItem(PersonalizationItemInfo personalizationItemInfo)
        {
            EditingItemInfo = personalizationItemInfo;
            SpawnRootObject();

            if (personalizationItemInfo != null)
            {
                UIPersonalizationEditor editorUi = UIPersonalizationEditor.instance;
                editorUi.Inspector.Populate();
                UIElementPersonalizationEditorUtilitiesPanel utils = editorUi.Utilities;
                utils.SetAvailablePresets(GetPresetsForEditingWeaponSkin());

                UIPersonalizationEditor.instance.ShowNotification("Success", $"Loaded {personalizationItemInfo.Name}!", UIElementPersonalizationEditorNotification.SuccessColor);
            }
        }

        public PersonalizationItemSaveResult SaveItem(bool ignoreDevPanel = false)
        {
            if (EditingItemInfo == null) return new PersonalizationItemSaveResult("Editing item info is NULL");
            if (!EditingRoot) return new PersonalizationItemSaveResult("Editing item is NULL");

            UIPersonalizationEditor.instance.Inspector.ApplyValues(ignoreDevPanel);
            SerializeRoot();

            return PersonalizationEditorDataManager.Instance.SaveItem(EditingItemInfo);
        }

        public void SerializeRoot()
        {
            EditingItemInfo.RootObject = EditingRoot.Serialize();
        }

        public void SerializeRootAndRespawnBot()
        {
            SerializeRoot();
            SpawnBot(true);
        }

        public void SpawnBot(bool spawnEditingItem)
        {
            _ = base.StartCoroutine(spawnBotCoroutine(spawnEditingItem));
        }

        private IEnumerator spawnBotCoroutine(bool spawnEditingItem)
        {
            if (_bot) BoltNetwork.Destroy(_bot.gameObject);

            GameObject spawnPoint = new GameObject("Temporary Player Spawn Point");
            spawnPoint.transform.position = Vector3.zero;

            UIElementPersonalizationEditorUtilitiesPanel utilities = UIPersonalizationEditor.instance.Utilities;
            Color favColor = utilities.GetFavoriteColor();
            CharacterModel model = MultiplayerCharacterCustomizationManager.Instance.GetCharacterModel(utilities.GetCharacterModelIndex());
            CloneSpawningData cloneSpawningData = new CloneSpawningData(spawnPoint.transform, true, false, favColor, model);
            CloneSpawner cloneSpawner = GameFlowManager.Instance._cloneSpawner;
            cloneSpawner.UseSkinInSingleplayer = false;

            FirstPersonMover bot = cloneSpawner.SpawnClone(cloneSpawningData);
            Destroy(spawnPoint);

            bot._upgradeCollection._upgradeLevels = new Dictionary<UpgradeType, int>();
            bot._upgradeCollection.AddUpgradeIfMissing(UpgradeType.SwordUnlock, 1);
            bot._upgradeCollection.AddUpgradeIfMissing(UpgradeType.BowUnlock, 1);
            bot._upgradeCollection.AddUpgradeIfMissing(UpgradeType.Hammer, 3);
            bot._upgradeCollection.AddUpgradeIfMissing(UpgradeType.SpearUnlock, 1);
            bot._upgradeCollection.AddUpgradeIfMissing(UpgradeType.EnergyCapacity, 2);
            bot._upgradeCollection.AddUpgradeIfMissing(UpgradeType.Dash, 1);
            bot._upgradeCollection.AddUpgradeIfMissing(ModUpgradesManager.SCYTHE_UNLOCK_UPGRADE, 1);
            bot.transform.eulerAngles = Vector3.up * 90f;
            if (bot._playerCamera) bot._playerCamera.gameObject.SetActive(false);

            EnergySource energySource = bot.GetEnergySource();
            energySource.HasInfiniteEnergy = true;

            _bot = bot;
            _greatSwordPreviewController = bot.gameObject.AddComponent<GreatSwordPreviewController>();

            if (spawnEditingItem)
            {
                while (!bot.GetComponent<PersonalizationController>() || !bot.GetComponent<PersonalizationController>().HasInitialized())
                    yield return null;

                bot.SetEquippedWeaponType(EditingItemInfo.Weapon, false);
                SpawnRootObject();
            }
            yield break;
        }

        public FirstPersonMover GetBot() => _bot;

        public void SpawnRootObject()
        {
            PersonalizationController personalizationController = PreviewingPersonalizationController;
            if (!personalizationController) return;

            personalizationController.DestroyAllItems();

            PersonalizationItemInfo info = EditingItemInfo;
            if (info == null) return;

            PersonalizationEditorObjectInfo rootInfo = info.RootObject;
            if (rootInfo == null) return;

            EditingRoot = personalizationController.SpawnItem(EditingItemInfo);
            PersonalizationEditorObjectManager.Instance.SetCurrentRootNextUniqueIndex();
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

                PersonalizationController personalizationController = firstPersonMover.GetComponent<PersonalizationController>();
                if (personalizationController) personalizationController.RefreshArrowSpawnPoint();

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
                firstPersonMover.SetEquippedWeaponType(EditingItemInfo.Weapon, false);
                firstPersonMover.GetComponent<BoltEntity>().ReleaseControl();
                firstPersonMover.transform.position = Vector3.zero;
                firstPersonMover.transform.eulerAngles = Vector3.up * 90f;
                firstPersonMover.stopAirCleavingIfActive();
            }

            yield break;
        }

        public bool IsInPlaytestMode() => _isInPlaytestMode;

        public void EnterScreenshotMode()
        {
            if (_isInScreenshotMode) return;

            PersonalizationItemSaveResult saveResult = SaveItem();
            if (EditingItemInfo != null && saveResult.HasFailed())
            {
                UIPersonalizationEditor.instance.ShowSaveErrorMessage(saveResult.Error);
                return;
            }

            _isInScreenshotMode = true;

            _editingItemBeforeScreenshoting = EditingItemInfo;
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

            PersonalizationEditorCamera cameraController = stage.GetCameraController();
            cameraController.gameObject.SetActive(true);

            _ = ModUIConstants.ShowPersonalizationEditorPlaytestHUD();
            ModUIConstants.ShowPersonalizationEditorScreenshotControls();
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

            EditItem(_editingItemBeforeScreenshoting);

            PersonalizationEditorCamera cameraController = stage.GetCameraController();
            cameraController.gameObject.SetActive(false);

            ModUIConstants.HidePersonalizationEditorPlaytestHUD();
            ModUIConstants.HidePersonalizationEditorScreenshotControls();
        }

        public bool IsInScreenshotMode() => _isInScreenshotMode;

        public void RefreshGreatswordPreview()
        {
            if (_greatSwordPreviewController) _greatSwordPreviewController.SetPreviewActivate(ViewingOriginalModel && (PreviewPresetKey == WeaponVariant2.NormalMultiplayer || PreviewPresetKey == WeaponVariant2.OnFireMultiplayer));
        }

        public List<Dropdown.OptionData> GetPresetsForEditingWeaponSkin(bool includeNone = false)
        {
            WeaponType weaponType = EditingItemInfo.Weapon;

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
            ModUIUtils.MessagePopupOK("Welcome to customization editor!", "Here you can make weapon skins. Accessories and pets are coming soon." +
                "\n\n<color=#FFFFFF><size=14>HOW TO MAKE STUFF?</size></color>" +
                "\nTo open or create a project, click on 'File' at the top left and click on 'Open'." +
                "\n\n<color=#FFFFFF><size=14>HOW TO UPLOAD STUFF?</size></color>" +
                "\nTo upload your project, click on 'Verify' at the top left and click on 'Upload item'." +
                "\n\nOnce the item's uploaded, you'll have to wait until it's verified in the next customization assets update." +
                "\n\nBefore uploading, you might want to make a preview image of your item, click on 'Screenshot' at the top, pick cool angle and click 'Take screenshot'." +
                "\n(Moderators can redo the preview image to align with others)" +
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
                    Color a = ModParseUtils.TryParseColor(oldAndNewColors[0], Color.white);
                    Color b = ModParseUtils.TryParseColor(oldAndNewColors[1], Color.white);
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