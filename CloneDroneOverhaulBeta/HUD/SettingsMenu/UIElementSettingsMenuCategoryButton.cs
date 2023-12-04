using CDOverhaul.Gameplay.Combat;
using CDOverhaul.NetworkAssets;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UIElementSettingsMenuCategoryButton : OverhaulBehaviour
    {
        [UIElementReference("CategoryLabel")]
        private readonly Text m_Label;

        [UIElementReference("SelectedIndicator")]
        private readonly GameObject m_SelectedIndicator;

        [UIElementReference("SelectedIndicator")]
        private readonly Graphic m_SelectedIndicatorGraphic;

        [UIElementReference("Icon")]
        private readonly RawImage m_Icon;

        private UISettingsMenu m_SettingsMenu;

        private bool m_Selected;
        public bool selected
        {
            get
            {
                return m_Selected;
            }
            set
            {
                m_Selected = value;
                m_SelectedIndicator.SetActive(value);

                if (!value)
                    return;

                UISettingsMenu settingsMenu = m_SettingsMenu;
                if (!settingsMenu)
                    return;

                UIElementSettingsMenuCategoryButton categoryButton = settingsMenu.recentSelectedCategoryButton;
                if (categoryButton && categoryButton != this)
                {
                    categoryButton.selected = false;
                }
                m_SelectedIndicatorGraphic.color = OverhaulCombatState.GetUIThemeColor(UISettingsMenu.DefaultBarColor);
            }
        }

        public string categoryId
        {
            get;
            set;
        }

        public override void Awake()
        {
            m_SettingsMenu = OverhaulUIManager.reference?.GetUI<UISettingsMenu>(UIConstants.UI_SETTINGS_MENU);
            if (!m_SettingsMenu)
            {
                OverhaulDebug.Error("CategoryButton - m_SettingsMenu is NULL! Manager existence: " + OverhaulUIManager.reference, EDebugType.UI);
                base.enabled = false;
                return;
            }

            Button button = base.GetComponent<Button>();
            button.AddOnClickListener(OnClicked);

            UIController.AssignVariables(this);
        }

        public override void Start()
        {
            selected = m_SettingsMenu.selectedCategoryId == categoryId;
            m_Label.text = categoryId;
            loadIcon();
        }

        protected override void OnDisposed()
        {
            if (m_Icon && m_Icon.texture)
                Destroy(m_Icon.texture);

            OverhaulDisposable.AssignNullToAllVars(this);
        }

        private void loadIcon()
        {
            string path = OverhaulMod.Core.ModDirectory + "Assets/Settings/" + categoryId + ".png";
            if (!File.Exists(path))
            {
                path = OverhaulMod.Core.ModDirectory + "Assets/Settings/Unknown.png";
            }

            OverhaulDownloadInfo overhaulDownloadInfo = new OverhaulDownloadInfo();
            overhaulDownloadInfo.DoneAction = delegate
            {
                if (!m_Icon || overhaulDownloadInfo.Error)
                    return;

                Texture texture = overhaulDownloadInfo.DownloadedTexture;
                texture.filterMode = FilterMode.Point;
                m_Icon.texture = texture;
            };
            OverhaulNetworkAssetsController.DownloadTexture("file://" + path, overhaulDownloadInfo);
        }

        public void OnClicked()
        {
            if (!base.enabled || selected)
                return;

            selected = true;
            m_SettingsMenu.selectedCategoryId = categoryId;
            m_SettingsMenu.selectedSectionId = string.Empty;
            m_SettingsMenu.recentSelectedCategoryButton = this;
            m_SettingsMenu.Populate();
        }
    }
}
