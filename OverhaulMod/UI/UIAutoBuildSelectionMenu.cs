using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIAutoBuildSelectionMenu : OverhaulUIBehaviour
    {
        [UIElement("Shading")]
        private readonly Image m_shading;

        [UIElement("SelectBuildLabelPrefab", false)]
        private readonly Text m_selectBuildLabelPrefab;

        [UIElement("BuildDisplayPrefab", false)]
        private readonly ModdedObject m_buildDisplayPrefab;

        [UIElement("Container")]
        private readonly Transform m_buildDisplayContainer;

        public override bool closeOnEscapeButtonPress => false;

        protected override void OnInitialized()
        {
            PopulateBuilds();

        }

        public override void Show()
        {
            base.Show();

        }

        public void PopulateBuilds()
        {
            if (m_buildDisplayContainer.childCount != 0)
                TransformUtils.DestroyAllChildren(m_buildDisplayContainer);

            Text label = Instantiate(m_selectBuildLabelPrefab, m_buildDisplayContainer);
            label.gameObject.SetActive(true);

            UpgradeManager upgradeManager = UpgradeManager.Instance;
            AutoBuildManager autoBuildManager = AutoBuildManager.Instance;
            foreach (var build in autoBuildManager.buildList.Builds)
            {
                ModdedObject moddedObject = Instantiate(m_buildDisplayPrefab, m_buildDisplayContainer);
                moddedObject.gameObject.SetActive(true);

                Text buildNameText = moddedObject.GetObject<Text>(0);
                buildNameText.text = AutoBuildManager.GetBuildDisplayName(build.Name);

                Image upgradeIconPrefab = moddedObject.GetObject<Image>(1);
                upgradeIconPrefab.gameObject.SetActive(true);
                Transform upgradeIconContainer = moddedObject.GetObject<Transform>(2);
                foreach (var upgrade in build.Upgrades)
                {
                    Sprite sprite = null;

                    UpgradeDescription upgradeDescription = upgradeManager.GetUpgrade(upgrade.UpgradeType, upgrade.Level);
                    if (upgradeDescription && upgradeDescription.Icon)
                    {
                        sprite = upgradeDescription.Icon;
                    }
                    else
                    {
                        sprite = ModResources.Sprite(AssetBundleConstants.UI, "NA-HQ-128x128");
                    }

                    Image upgradeIcon = Instantiate(upgradeIconPrefab, upgradeIconContainer);
                    upgradeIcon.sprite = sprite;
                }
                upgradeIconPrefab.gameObject.SetActive(false);
            }
        }
    }
}
