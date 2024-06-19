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
        private readonly Button m_closeButton;

        [UIElement("ReplaceColorDisplay", false)]
        private readonly ModdedObject m_replaceColorDisplay;

        [UIElement("Content")]
        private readonly Transform m_container;

        private List<ColorPairFloat> m_colorPairs;

        public Action<string> onColorChanged
        {
            get;
            set;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            m_colorPairs = new List<ColorPairFloat>();
        }

        public void Populate(string colorsString)
        {
            List<ColorPairFloat> list = PersonalizationEditorManager.Instance.GetColorPairsFromString(colorsString);
            m_colorPairs = list;

            if (m_container.childCount != 0)
                TransformUtils.DestroyAllChildren(m_container);

            if (!list.IsNullOrEmpty())
            {
                int index = 0;
                foreach (ColorPairFloat cp in list)
                {
                    int i = index;
                    ModdedObject colorPairDisplay = Instantiate(m_replaceColorDisplay, m_container);
                    colorPairDisplay.gameObject.SetActive(true);
                    UIElementPersonalizationEditorColorPairDisplay editorColorPairDisplay = colorPairDisplay.gameObject.AddComponent<UIElementPersonalizationEditorColorPairDisplay>();
                    editorColorPairDisplay.InitializeElement();
                    editorColorPairDisplay.returnNewPair = false;
                    editorColorPairDisplay.colorPair = cp;
                    editorColorPairDisplay.colorPickerTransform = UIPersonalizationEditor.instance.transform;
                    editorColorPairDisplay.onValueChanged.AddListener(onColorChangedCallback);

                    index++;
                }
            }
        }

        private void onColorChangedCallback(ColorPairFloat colorPairFloat)
        {
            onColorChanged?.Invoke(PersonalizationEditorManager.Instance.GetStringFromColorPairs(m_colorPairs));
        }
    }
}
