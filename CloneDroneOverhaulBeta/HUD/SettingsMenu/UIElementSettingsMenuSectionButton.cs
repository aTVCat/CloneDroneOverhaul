using CDOverhaul.NetworkAssets;
using InjectionClasses;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UIElementSettingsMenuSectionButton : OverhaulBehaviour
    {
        [UIElementReference("Name")]
        private readonly Text m_NameLabel;

        [UIElementReference("Description")]
        private readonly Text m_DescriptionLabel;

        [UIElementDefaultVisibilityState(false)]
        [UIElementReference("Icon")]
        private readonly RawImage m_Icon;

        [UIElementReference("Outline")]
        private readonly Graphic m_Outline;

        [UIElementComponents(new System.Type[] { typeof(UIComponentGraphicColorUpdater) })]
        [UIElementReference("Icon")]
        private readonly UIComponentGraphicColorUpdater m_IconColors;

        private UISettingsMenu m_SettingsMenu;

        public string categoryId
        {
            get;
            set;
        }

        public string sectionId
        {
            get;
            set;
        }

        public override void Awake()
        {
            m_SettingsMenu = OverhaulUIManager.reference?.GetUI<UISettingsMenu>(UIConstants.UI_SETTINGS_MENU);
            if (!m_SettingsMenu)
            {
                OverhaulDebug.Error("SectionButton - m_SettingsMenu is NULL! Manager existence: " + OverhaulUIManager.reference, EDebugType.UI);
                base.enabled = false;
                return;
            }

            UIController.AssignValues(this);
        }

        public override void Start()
        {
            m_NameLabel.text = sectionId;
            m_DescriptionLabel.text = "This is a section without description support :(";

            m_IconColors.colors = new Color[]
            {
                Color.white,
                Color.clear
            };
            m_IconColors.colorIndex = 1;
            m_IconColors.SetGraphic(m_Icon);

            base.gameObject.AddComponent<OverhaulUISelectionOutline>().SetGraphic(m_Outline);
            Button button = base.GetComponent<Button>();
            button.AddOnClickListener(OnClicked);

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
            m_Icon.color = Color.clear;
            m_IconColors.colorIndex = 1;

            string path = OverhaulMod.Core.ModDirectory + "Assets/Settings/Section_" + sectionId + ".png";
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
                m_Icon.gameObject.SetActive(true);
                m_IconColors.colorIndex = 0;
            };
            OverhaulNetworkAssetsController.DownloadTexture("file://" + path, overhaulDownloadInfo);
        }

        public void OnClicked()
        {
            m_SettingsMenu.selectedCategoryId = categoryId;
            m_SettingsMenu.selectedSectionId = sectionId;
            m_SettingsMenu.Populate();
        }
    }
}
