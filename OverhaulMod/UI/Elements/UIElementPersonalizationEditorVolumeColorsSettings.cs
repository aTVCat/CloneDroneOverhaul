using OverhaulMod.Content.Personalization;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationEditorVolumeColorsSettings : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button _closeButton;

        [UIElement("ReplaceColorDisplay", false)]
        private readonly ModdedObject _replaceColorDisplay;

        [UIElement("Content")]
        private readonly Transform _container;

        [UIElementAction(nameof(OnCopyColorsButtonClicked))]
        [UIElement("CopyColorsButton")]
        private readonly Button _copyColorsButton;

        [UIElementAction(nameof(OnPasteColorsButtonClicked))]
        [UIElement("PasteColorsButton")]
        private readonly Button _pasteColorsButton;

        private VolumeSettingsPreset _volumeSettingsPreset;

        private List<ColorPairFloat> _colorPairs;

        private Dictionary<string, FavoriteColorSettings> _favoriteColorSettings;

        public Action<string> OnColorChanged;

        protected override void OnInitialized()
        {
            _colorPairs = new List<ColorPairFloat>();
            _pasteColorsButton.interactable = false;
        }

        public void Populate(VolumeSettingsPreset volumeSettingsPreset)
        {
            _volumeSettingsPreset = volumeSettingsPreset;
            _colorPairs = PersonalizationEditorManager.Instance.GetColorPairsFromString(volumeSettingsPreset.ColorReplacements);
            _favoriteColorSettings = volumeSettingsPreset.ReplaceWithFavoriteColors;
            populate(_colorPairs, _favoriteColorSettings);
        }

        private void populate(List<ColorPairFloat> list, Dictionary<string, FavoriteColorSettings> replaceWithFavoriteColors)
        {
            if (_container.childCount != 0)
                TransformUtils.DestroyAllChildren(_container);

            if (!list.IsNullOrEmpty())
            {
                int index = 0;
                foreach (ColorPairFloat cp in list)
                {
                    int i = index;
                    ModdedObject colorPairDisplay = Instantiate(_replaceColorDisplay, _container);
                    colorPairDisplay.gameObject.SetActive(true);
                    UIElementPersonalizationEditorColorPairDisplay editorColorPairDisplay = colorPairDisplay.gameObject.AddComponent<UIElementPersonalizationEditorColorPairDisplay>();
                    editorColorPairDisplay.InitializeElement();
                    editorColorPairDisplay.returnNewPair = false;
                    editorColorPairDisplay.colorPair = cp;
                    editorColorPairDisplay.favoriteColorSettings = replaceWithFavoriteColors;
                    editorColorPairDisplay.colorPickerTransform = UIPersonalizationEditor.Instance.transform;
                    editorColorPairDisplay.OnValueChanged.AddListener(onColorChangedCallback);
                    editorColorPairDisplay.OnFavoriteColorSettingsChanged.AddListener(delegate
                    {
                        GlobalEventManager.Instance.Dispatch(PersonalizationEditorManager.OBJECT_EDITED_EVENT);
                    });

                    index++;
                }
            }
        }

        private void onColorChangedCallback(ColorPairFloat colorPairFloat)
        {
            OnColorChanged?.Invoke(PersonalizationEditorManager.Instance.GetStringFromColorPairs(_colorPairs));
        }

        public void OnCopyColorsButtonClicked()
        {
            if (_colorPairs == null || _favoriteColorSettings == null)
                return;

            _pasteColorsButton.interactable = true;
            PersonalizationEditorClipboard.Instance.CopyColorSettings(_colorPairs, _favoriteColorSettings);
        }

        public void OnPasteColorsButtonClicked()
        {
            List<ColorPairFloat> originalColors = _colorPairs;
            Dictionary<string, FavoriteColorSettings> originalFavoriteColors = _favoriteColorSettings;

            if (originalColors == null || originalFavoriteColors == null)
                return;

            _pasteColorsButton.interactable = false;

            PersonalizationEditorClipboard.Instance.PasteColorSettings(out List<ColorPairFloat> colorPairs, out Dictionary<string, FavoriteColorSettings> favoriteColors);

            foreach (ColorPairFloat colorPairFloatA in originalColors)
            {
                foreach (ColorPairFloat colorPairFloatB in colorPairs)
                {
                    if (colorPairFloatA.ColorA == colorPairFloatB.ColorA)
                        colorPairFloatA.ColorB = colorPairFloatB.ColorB;
                }
            }

            originalFavoriteColors.Clear();
            foreach (KeyValuePair<string, FavoriteColorSettings> kv in favoriteColors)
                originalFavoriteColors.Add(kv.Key, kv.Value.Clone());

            onColorChangedCallback(null);
            populate(originalColors, originalFavoriteColors);
        }
    }
}
