using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIFeaturesMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElement("FeatureStateDisplay", false)]
        private readonly ModdedObject m_featureStatePrefab;

        [UIElement("Content")]
        private readonly Transform m_container;

        [UIElementAction(nameof(OnResetButtonClicked))]
        [UIElement("ResetButton")]
        private readonly Button m_resetButton;

        public override bool hideTitleScreen => true;

        private bool m_hasToRestart;

        protected override void OnInitialized()
        {
            populate();
        }

        public override void Hide()
        {
            base.Hide();

            if (m_hasToRestart)
            {
                m_hasToRestart = false;
                ModFeatures.Save();
                ModUIConstants.ShowRestartRequiredScreen(true);
            }
        }

        private void populate()
        {
            if (m_container.childCount != 0)
                TransformUtils.DestroyAllChildren(m_container);

            System.Collections.Generic.Dictionary<ModFeatures.FeatureType, bool> d = ModFeatures.GetData()?.FeatureStates;
            if (d == null)
                return;

            int position = 1;
            foreach (System.Collections.Generic.KeyValuePair<ModFeatures.FeatureType, bool> feature in d)
            {
                ModdedObject moddedObject = Instantiate(m_featureStatePrefab, m_container);
                moddedObject.gameObject.SetActive(true);
                moddedObject.GetObject<Text>(0).text = StringUtils.AddSpacesToCamelCasedString(feature.Key.ToString());
                moddedObject.GetObject<GameObject>(1).SetActive(position % 2 == 0);

                Toggle toggle = moddedObject.GetComponent<Toggle>();
                toggle.isOn = feature.Value;
                toggle.onValueChanged.AddListener(delegate (bool value)
                {
                    d[feature.Key] = value;
                    m_hasToRestart = true;
                });

                position++;
            }
        }

        public void OnResetButtonClicked()
        {
            if (ModFeatures.Default())
            {
                m_hasToRestart = true;
                populate();
            }
        }
    }
}
