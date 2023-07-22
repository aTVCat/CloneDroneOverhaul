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

        private const string ITEM_BACKGROUND = "Item Background";

        [OverhaulSettingWithNotification(1)]
        [OverhaulSetting("Game interface.Vanilla changes.\"Piksieli Prst\" font", true, false, "This font makes Overhaul's UI less differ from game UI")]
        public static bool PixelsSimpleFont;

        private static Font s_OgUIFont;
        private static Font s_OgSubtitlesFont;
        private static float s_OgFontScale = -1f;

        private static Font s_OpenSansRegularFont;
        private static Font s_OpenSansExtraBoldFont;
        private static Sprite s_CanvasDark;
        private static Sprite s_CanvasBright;
        private static Sprite s_Checkmark;

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

            if (s_OgUIFont == null) s_OgUIFont = LocalizationManager.Instance.SupportedLanguages[0].UIFont;
            if (s_OgSubtitlesFont == null) s_OgSubtitlesFont = LocalizationManager.Instance.SupportedLanguages[0].SubtitlesFont;
            if (s_OgFontScale == -1f) s_OgFontScale = LocalizationManager.Instance.SupportedLanguages[0].UIFontScale;
            SetEnglishFont(PixelsSimpleFont);

            UpdateSprites(GameUIRoot.Instance.transform);

            UpdateSprites(GameUIRoot.Instance.LevelEditorUI.FilesUI.DisplayPrefab.transform);
            UpdateSprites(GameUIRoot.Instance.LevelEditorUI.FilesUI.FolderPrefab.transform);
            UpdateSprites(GameUIRoot.Instance.LevelEditorUI.LibraryUI.DisplayPrefab.transform);
            UpdateSprites(GameUIRoot.Instance.LevelEditorUI.LibraryUI.FolderPrefab.transform);

            UpdateSprites(GameUIRoot.Instance.LevelEditorUI.ChallengeConfigUI.ChallengeListItemDisplayPrefab.transform);
            UpdateSprites(GameUIRoot.Instance.LevelEditorUI.ChallengeConfigUI.LevelEditorChallengeLevelDisplayPrefab.transform);

            UpdateSprites(GameUIRoot.Instance.LevelEditorUI.SectionConfigPanel.SectionListItemPrefab.transform);
            UpdateSprites(GameUIRoot.Instance.LevelEditorUI.HistoryPanel.ItemViewPrefab.transform);

            LevelEditorInspector levelEditorInspector = GameUIRoot.Instance.LevelEditorUI.InspectorTransform.GetComponent<LevelEditorInspector>();
            UpdateSprites(levelEditorInspector.CustomFieldContainer);
            UpdateSprites(levelEditorInspector.CustomInspectorDescriptionBoxPrefab.transform);
            UpdateSprites(levelEditorInspector.CustomInspectorPropertyGroupPrefab.transform);

            CustomInspectorPropertyGroup propertyGroup = levelEditorInspector.CustomInspectorPropertyGroupPrefab;
            UpdateSprites(propertyGroup.CustomFieldCheckBoxPrefab.transform);
            UpdateSprites(propertyGroup.CustomFieldColorPrefab.transform);
            UpdateSprites(propertyGroup.CustomFieldEditorPrefab.transform);
            UpdateSprites(propertyGroup.CustomFieldEditorStringInputPrefab.transform);
            UpdateSprites(propertyGroup.CustomFieldEditorDropdownPrefab.transform);
            UpdateSprites(propertyGroup.CustomFieldStringToggleListPrefab.transform);
            UpdateSprites(propertyGroup.CustomFieldStringToggleListPrefab.StringToggleListWindowPrefab.transform);
            UpdateSprites(propertyGroup.CustomFieldStringToggleListPrefab.StringToggleListWindowPrefab.ListItemPrefab.transform);
            UpdateSprites(propertyGroup.CustomInspectorLevelSelectorFieldPrefab.transform);
            UpdateSprites(propertyGroup.MethodButtonPrefab.transform);
            UpdateSprites(propertyGroup.CustomInspectorMethodCalledFromAnimationDropdownPrefab.transform);

            SuccessfullyPatched = true;
        }

        public static void UpdateSprites(Transform transform)
        {
            if (!s_CanvasDark)
                s_CanvasDark = OverhaulAssetsController.GetAsset<Sprite>("CanvasDark-SQ2-16x16"/*"CanvasDark-Small2-16x16"*/, OverhaulAssetPart.Part1);
            if (!s_CanvasBright)
                s_CanvasBright = OverhaulAssetsController.GetAsset<Sprite>("CanvasBright-SQ-16x16"/*"CanvasBright-Small-16x16"*/, OverhaulAssetPart.Part1);
            if (!s_Checkmark)
                s_Checkmark = OverhaulAssetsController.GetAsset<Sprite>("CheckmarkSmall", OverhaulAssetPart.Part1);
            if (!s_OpenSansRegularFont)
                s_OpenSansRegularFont = OverhaulAssetsController.GetAsset<Font>("OpenSans-Regular", OverhaulAssetPart.Fonts);
            if (!s_OpenSansExtraBoldFont)
                s_OpenSansExtraBoldFont = OverhaulAssetsController.GetAsset<Font>("OpenSans-ExtraBold", OverhaulAssetPart.Fonts);

            foreach (Image image in transform.GetComponentsInChildren<Image>(true))
            {
                if (image && image.sprite)
                {
                    string name = image.sprite.name;
                    if (name.Equals(UI_SPRITE) || name.Equals(KNOB))
                    {
                        image.sprite = s_CanvasDark;
                    }
                    else if (name.Equals(CHECKMARK))
                    {
                        image.sprite = s_Checkmark;
                        image.color = Color.black;
                    }
                    else if (name.Equals(BACKGROUND) || name.Equals(INPUT_FIELD_BACKGROUND))
                    {
                        image.sprite = s_CanvasBright;
                    }
                    else if (image.name.Equals(ITEM_BACKGROUND))
                    {
                        RectTransform rectTransform = image.rectTransform;
                        if (rectTransform.anchorMax == Vector2.one)
                            rectTransform.anchorMax = new Vector2(0.985f, 1f);
                        if (rectTransform.anchorMin == Vector2.zero)
                            rectTransform.anchorMin = new Vector2(0.015f, 0f);
                    }
                }
            }

            foreach(Mask mask in transform.GetComponentsInChildren<Mask>(true))
            {
                if (mask)
                {
                    GameObject gameObject = mask.gameObject;
                    Image image = mask.GetComponent<Image>();
                    if (image)
                    {
                        Object.Destroy(mask);
                        image.color = Color.clear;
                        gameObject.AddComponent<RectMask2D>();
                    }
                }
            }

            foreach (Text text in transform.GetComponentsInChildren<Text>(true))
            {
                if (text && text.font)
                {
                    if (!text.GetComponent<LocalizedTextField>())
                    {
                        if(text.font.name == "Arial")
                        {
                            text.font = s_OpenSansRegularFont;
                            text.fontSize -= 2;
                            text.fontStyle = FontStyle.Normal;
                        }
                        else if (text.font.name == "KhmerUIb")
                        {
                            text.font = s_OpenSansExtraBoldFont;
                            text.fontSize -= 2;
                        }
                    }
                }
            }
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

            if (OverhaulVersion.IsUpdate2)
            {
                LocalizationManager.Instance.SetCurrentLanguage(LocalizationManager.Instance.GetCurrentLanguageCode());
            }
        }

        public static void ChangeButtonAction(Transform transform, string buttonObjectName, UnityAction action)
        {
            if (!transform || string.IsNullOrEmpty(buttonObjectName) || action == null)
                return;

            Transform transform1 = transform.FindChildRecurisve(buttonObjectName);
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
    }
}
