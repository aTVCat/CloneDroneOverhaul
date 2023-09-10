using Bolt;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CDOverhaul.Patches
{
    public class BaseFixes : ReplacementBase
    {
        private const string UI_SPRITE = "UISprite";
        private const string KNOB = "Knob";
        private const string CHECKMARK = "Checkmark";
        private const string BACKGROUND = "Background";
        private const string INPUT_FIELD_BACKGROUND = "InputFieldBackground";
        private const string EXIT_BUTTON = "ExitButton";

        private const string ITEM_BACKGROUND = "Item Background";

        private const string ARIAL = "Arial";
        private const string KHMERUIB = "KhmerUIb";

        [OverhaulSettingWithNotification(1)]
        [OverhaulSettingAttribute_Old("Game interface.Vanilla changes.\"Piksieli Prst\" font", true, false, "This font makes Overhaul's UI less differ from game UI")]
        public static bool PixelsSimpleFont;

        private static Font s_OgUIFont;
        private static Font s_OgSubtitlesFont;
        private static float s_OgFontScale = -1f;

        private static Font s_OpenSansRegularFont;
        private static Font s_OpenSansExtraBoldFont;
        private static Sprite s_CanvasDark;
        private static Sprite s_CanvasDarkNoGrayOutline;
        private static Sprite s_CanvasBright;
        private static Sprite s_CanvasBrightNoGrayOutline;
        private static Sprite s_Checkmark;
        private static Sprite s_ExitIcon;

        public override void Replace()
        {
            base.Replace();

            // Fix stripes on environment
            DirectionalLightManager.Instance.DirectionalLight.shadowNormalBias = 1.1f;
            DirectionalLightManager.Instance.DirectionalLight.shadowBias = 1f;

            // This may reduce RAM usage & improve performance a bit
            UnityEngine.Physics.reuseCollisionCallbacks = true;

            GameUIRoot.Instance.EmoteSelectionUI.GetComponent<Image>().enabled = false;
            ProjectileManager.Instance.ArrowPool.Prefab.GetComponent<Projectile>().VelocityMagnitude = 75f;

            EmoteManager.Instance.PitchLimits.Max = 1f;
            MultiplayerCharacterCustomizationManager.Instance.CharacterModels[17].UnlockedByAchievementID = string.Empty;

            SkyBoxManager.Instance.LevelConfigurableSkyboxes[8].SetColor("_Tint", new Color(0.6f, 0.73f, 2f, 1f));

            AttackManager manager = AttackManager.Instance;
            if (manager)
            {
                manager.HitColor = new Color(4f, 0.65f, 0.35f, 0.2f);
                manager.BodyOnFireColor = new Color(1f, 0.42f, 0.22f, 0.1f);
            }

            AudioConfiguration audioConfiguration = AudioSettings.GetConfiguration();
            if (!OverhaulVersion.IsTestMode && audioConfiguration.numVirtualVoices != 512 && GameModeManager.IsOnTitleScreen())
            {
                audioConfiguration.numVirtualVoices = 512;
                audioConfiguration.numRealVoices = 32;
                audioConfiguration.dspBufferSize = 1024;
                _ = AudioSettings.Reset(audioConfiguration);
            }

            if (!s_OgUIFont)
                s_OgUIFont = LocalizationManager.Instance.SupportedLanguages[0].UIFont;
            if (!s_OgSubtitlesFont)
                s_OgSubtitlesFont = LocalizationManager.Instance.SupportedLanguages[0].SubtitlesFont;
            if (s_OgFontScale == -1f)
                s_OgFontScale = LocalizationManager.Instance.SupportedLanguages[0].UIFontScale;
            SetEnglishFont(PixelsSimpleFont);

            UpdateSprites(GameUIRoot.Instance.transform, false, -2, (FontStyle)(-1), true);
            UpdateSprites(GameUIRoot.Instance.LevelEditorUI.FilesUI.DisplayPrefab.transform);
            UpdateSprites(GameUIRoot.Instance.LevelEditorUI.FilesUI.FolderPrefab.transform);
            UpdateSprites(GameUIRoot.Instance.LevelEditorUI.LibraryUI.DisplayPrefab.transform);
            UpdateSprites(GameUIRoot.Instance.LevelEditorUI.LibraryUI.FolderPrefab.transform);

            UpdateSprites(GameUIRoot.Instance.TwitchEnemySettingsMenu.TwitchEnemyDisplayPrefab.transform);
            Transform ps4ControllerLabel = GameUIRoot.Instance.SettingsMenu.transform.FindChildRecursive("Label_PS4");
            if (ps4ControllerLabel)
            {
                ps4ControllerLabel.gameObject.SetActive(false);
            }

            Transform spectatingLabel = GameUIRoot.Instance.CurrentlySpectatingUI.transform.FindChildRecursive("SpectatingWord");
            if (spectatingLabel)
            {
                spectatingLabel.gameObject.SetActive(false);
            }
            Transform currentUsernameLabel = GameUIRoot.Instance.CurrentlySpectatingUI.transform.FindChildRecursive("CurrentUsername");
            if (currentUsernameLabel)
            {
                currentUsernameLabel.gameObject.SetActive(false);
            }

            UpdateSprites(GameUIRoot.Instance.LevelEditorUI.ChallengeConfigUI.ChallengeListItemDisplayPrefab.transform, false, -2, (FontStyle)(-1), false, true);
            UpdateSprites(GameUIRoot.Instance.LevelEditorUI.ChallengeConfigUI.LevelEditorChallengeLevelDisplayPrefab.transform, false, -2, (FontStyle)(-1), false, true);
            UpdateSprites(GameUIRoot.Instance.LevelEditorUI.LevelTagSelector.LevelTagCategoryListPrefab.transform, false, -2, (FontStyle)(-1), false, true);
            UpdateSprites(GameUIRoot.Instance.LevelEditorUI.LevelTagSelector.LevelTagCategoryListPrefab.TagButtonPrefab.transform, false, -2, (FontStyle)(-1), false, true);

            UpdateSprites(GameUIRoot.Instance.LevelEditorUI.SectionConfigPanel.SectionListItemPrefab.transform, false, -2, (FontStyle)(-1), false, true);
            UpdateSprites(GameUIRoot.Instance.LevelEditorUI.HistoryPanel.ItemViewPrefab.transform, false, -2, (FontStyle)(-1), false, true);

            LevelEditorInspector levelEditorInspector = GameUIRoot.Instance.LevelEditorUI.InspectorTransform.GetComponent<LevelEditorInspector>();
            UpdateSprites(levelEditorInspector.CustomFieldContainer, false, -2, (FontStyle)(-1), false, true);
            UpdateSprites(levelEditorInspector.CustomInspectorDescriptionBoxPrefab.transform, false, -2, (FontStyle)(-1), false, true);
            UpdateSprites(levelEditorInspector.CustomInspectorPropertyGroupPrefab.transform, false, -2, (FontStyle)(-1), false, true);

            CustomInspectorPropertyGroup propertyGroup = levelEditorInspector.CustomInspectorPropertyGroupPrefab;
            UpdateSprites(propertyGroup.CustomFieldCheckBoxPrefab.transform, false, -2, (FontStyle)(-1), false, true);
            UpdateSprites(propertyGroup.CustomFieldColorPrefab.transform, false, -2, (FontStyle)(-1), false, true);
            UpdateSprites(propertyGroup.CustomFieldEditorPrefab.transform, false, -2, (FontStyle)(-1), false, true);
            UpdateSprites(propertyGroup.CustomFieldEditorStringInputPrefab.transform, false, -2, (FontStyle)(-1), false, true);
            UpdateSprites(propertyGroup.CustomFieldEditorDropdownPrefab.transform, false, -2, (FontStyle)(-1), false, true);
            UpdateSprites(propertyGroup.CustomFieldStringToggleListPrefab.transform, false, -2, (FontStyle)(-1), false, true);
            UpdateSprites(propertyGroup.CustomFieldStringToggleListPrefab.StringToggleListWindowPrefab.transform, false, -2, (FontStyle)(-1), false, true);
            UpdateSprites(propertyGroup.CustomFieldStringToggleListPrefab.StringToggleListWindowPrefab.ListItemPrefab.transform, false, -2, (FontStyle)(-1), false, true);
            UpdateSprites(propertyGroup.CustomInspectorLevelSelectorFieldPrefab.transform, false, -2, (FontStyle)(-1), false, true);
            UpdateSprites(propertyGroup.MethodButtonPrefab.transform, false, -2, (FontStyle)(-1), false, true);
            UpdateSprites(propertyGroup.CustomInspectorMethodCalledFromAnimationDropdownPrefab.transform, false, -2, (FontStyle)(-1), false, true);

            UpdateSprites(TwitchEnemySpawnManager.Instance.TwitchEnemyNameTagPool.Prefab, false, -3);
            UpdateSprites(PlayerAllyManager.Instance.StoryAllyNameTagPool.Prefab, false, -3);

            UpdateSprites(PrefabDatabase.Find(BoltPrefabs.BattleRoyaleTransportBot).transform, false, 0);

            _ = StaticCoroutineRunner.StartStaticCoroutine(updateLangFontCoroutine());
            SuccessfullyPatched = true;
        }

        public static void UpdateSprites(Transform transform, bool forceRegularFont = false, int offsetFontSize = -2, FontStyle forceFontStyle = (FontStyle)(-1), bool checkLevelEditor = false, bool forceNoGrayOutline = false)
        {
            if (!s_CanvasDark)
                s_CanvasDark = OverhaulAssetLoader.GetAsset<Sprite>("CanvasDark-SQ2-16x16"/*"CanvasDark-Small2-16x16"*/, OverhaulAssetPart.Part1);
            if (!s_CanvasBright)
                s_CanvasBright = OverhaulAssetLoader.GetAsset<Sprite>("CanvasBright-SQ-16x16"/*"CanvasBright-Small-16x16"*/, OverhaulAssetPart.Part1);
            if (!s_CanvasBrightNoGrayOutline)
                s_CanvasBrightNoGrayOutline = s_CanvasBright = OverhaulAssetLoader.GetAsset<Sprite>("CanvasBright-SQ-NGO", OverhaulAssetPart.Part1);
            if (!s_CanvasDarkNoGrayOutline)
                s_CanvasDarkNoGrayOutline = OverhaulAssetLoader.GetAsset<Sprite>("CanvasDark-SQ-NGO", OverhaulAssetPart.Part1);
            if (!s_Checkmark)
                s_Checkmark = OverhaulAssetLoader.GetAsset<Sprite>("CheckmarkSmall", OverhaulAssetPart.Part1);
            if (!s_ExitIcon)
                s_ExitIcon = OverhaulAssetLoader.GetAsset<Sprite>("Close-Colored-16x16", OverhaulAssetPart.Part1);
            if (!s_OpenSansRegularFont)
                s_OpenSansRegularFont = OverhaulAssetLoader.GetAsset<Font>("OpenSans-Regular", OverhaulAssetPart.Fonts);
            if (!s_OpenSansExtraBoldFont)
                s_OpenSansExtraBoldFont = OverhaulAssetLoader.GetAsset<Font>("OpenSans-ExtraBold", OverhaulAssetPart.Fonts);


            foreach (Image image in transform.GetComponentsInChildren<Image>(true))
            {
                if (image && image.sprite)
                {
                    string name = image.sprite.name;
                    if (name.Equals(UI_SPRITE) || name.Equals(KNOB))
                    {
                        image.sprite = forceNoGrayOutline || (checkLevelEditor && CheckParentNameRecursive(image.transform, "LevelEditorUI"))
                            ? s_CanvasDarkNoGrayOutline
                            : s_CanvasDark;
                    }
                    else if (name.Equals(CHECKMARK))
                    {
                        image.sprite = s_Checkmark;
                        image.color = Color.black;
                    }
                    else if (name.Equals(BACKGROUND) || name.Equals(INPUT_FIELD_BACKGROUND))
                    {
                        image.sprite = forceNoGrayOutline || (checkLevelEditor && CheckParentNameRecursive(image.transform, "LevelEditorUI"))
                            ? s_CanvasBrightNoGrayOutline
                            : s_CanvasBright;
                    }
                    else if (image.name.Equals(ITEM_BACKGROUND))
                    {
                        RectTransform rectTransform = image.rectTransform;
                        if (rectTransform.anchorMax == Vector2.one)
                            rectTransform.anchorMax = new Vector2(0.985f, 1f);
                        if (rectTransform.anchorMin == Vector2.zero)
                            rectTransform.anchorMin = new Vector2(0.015f, 0f);
                    }/*
                    else if (image.sprite.texture && image.sprite.texture.name.Equals(EXIT_BUTTON))
                    {
                        image.sprite = s_ExitIcon;
                    }*/
                }
            }

            foreach (Mask mask in transform.GetComponentsInChildren<Mask>(true))
            {
                if (mask)
                {
                    GameObject gameObject = mask.gameObject;
                    Image image = mask.GetComponent<Image>();
                    if (image)
                    {
                        Object.Destroy(mask);
                        image.color = Color.clear;
                        _ = gameObject.AddComponent<RectMask2D>();
                    }
                }
            }

            foreach (Text text in transform.GetComponentsInChildren<Text>(true))
            {
                if (text && text.font)
                {
                    if (!text.GetComponent<LocalizedTextField>())
                    {
                        if (text.font.name == ARIAL || forceRegularFont)
                        {
                            text.font = s_OpenSansRegularFont;
                            text.fontSize += offsetFontSize;
                            text.fontStyle = FontStyle.Normal;
                        }
                        else if (text.font.name == KHMERUIB)
                        {
                            text.font = s_OpenSansExtraBoldFont;
                            text.fontSize += offsetFontSize;
                            text.resizeTextMinSize += offsetFontSize;
                        }

                        if (forceFontStyle != (FontStyle)(-1))
                        {
                            text.fontStyle = forceFontStyle;
                        }
                    }
                }
            }
        }

        public static bool CheckParentNameRecursive(Transform transform, string name)
        {
            return transform && (transform.name == name || CheckParentNameRecursive(transform.parent, name));
        }

        public static void SetEnglishFont(bool piksielyPrst)
        {
            if (!LocalizationManager.Instance || LocalizationManager.Instance.SupportedLanguages.IsNullOrEmpty())
                return;

            if (!piksielyPrst)
            {
                _ = LocalizationManager.Instance.SupportedLanguages[0].UIFont != s_OgUIFont;
                LocalizationManager.Instance.SupportedLanguages[0].UIFont = s_OgUIFont;
                LocalizationManager.Instance.SupportedLanguages[0].SubtitlesFont = s_OgSubtitlesFont;
                LocalizationManager.Instance.SupportedLanguages[0].UIFontScale = s_OgFontScale;
            }
            else
            {
                _ = LocalizationManager.Instance.SupportedLanguages[0].UIFont != LocalizationManager.Instance.SupportedLanguages[7].UIFont;
                LocalizationManager.Instance.SupportedLanguages[0].UIFont = LocalizationManager.Instance.SupportedLanguages[7].UIFont;
                LocalizationManager.Instance.SupportedLanguages[0].SubtitlesFont = LocalizationManager.Instance.SupportedLanguages[7].SubtitlesFont;
                LocalizationManager.Instance.SupportedLanguages[0].UIFontScale = 0.675f;
            }
        }

        public static void ChangeButtonAction(Transform transform, string buttonObjectName, UnityAction action)
        {
            if (!transform || string.IsNullOrEmpty(buttonObjectName) || action == null)
                return;

            Transform transform1 = transform.FindChildRecursive(buttonObjectName);
            if (!transform1)
            {
                throw new System.Exception("Could not find " + buttonObjectName + " button under " + transform.name + " object");
            }

            Button button = transform1.GetComponent<Button>();
            if (!button)
            {
                throw new System.Exception("Object " + buttonObjectName + " is not a button!");
            }

            button.RemoveAllOnClickListeners();
            button.AddOnClickListener(action);
        }

        public static IEnumerator WaitThenMirrorEmote_Patched(EmoteDefinition emotePlayed)
        {
            yield return new WaitForSeconds(1f);
            FirstPersonMover player = CharacterTracker.Instance.GetPlayerAlly() as FirstPersonMover;
            if (player && !player.IsWeaponDamageActive())
            {
                EmoteManager.Instance.TriggerEmoteForPlayer(player, emotePlayed);
            }
            yield break;
        }

        private static IEnumerator updateLangFontCoroutine()
        {
            yield return new WaitUntil(() => SettingsManager.Instance.IsInitialized());
            LocalizationManager.Instance.SetCurrentLanguage(SettingsManager.Instance.GetCurrentLanguageID());
            yield break;
        }
    }
}
