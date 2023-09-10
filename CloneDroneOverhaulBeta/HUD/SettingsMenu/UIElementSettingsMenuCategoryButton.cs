using CDOverhaul.NetworkAssets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UIElementSettingsMenuCategoryButton : OverhaulBehaviour
    {
        [UIElementReference("CategoryLabel")]
        private Text m_Label;

        [UIElementReference("SelectedIndicator")]
        private GameObject m_SelectedIndicator;

        [UIElementReference("Icon")]
        private RawImage m_Icon;

        private UIOverhaulSettingsMenu m_SettingsMenu;

        private string m_Text;
        public string text
        {
            get
            {
                return m_Text;
            }
            set
            {
                m_Text = value;
                m_Label.text = value;
            }
        }

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

                UIOverhaulSettingsMenu settingsMenu = m_SettingsMenu;
                if (!settingsMenu)
                    return;

                UIElementSettingsMenuCategoryButton categoryButton = settingsMenu.recentSelectedCategoryButton;
                if (categoryButton && categoryButton != this)
                {
                    categoryButton.selected = false;
                }
            }
        }

        public string categoryId
        {
            get;
            set;
        }

        public override void Awake()
        {
            m_SettingsMenu = OverhaulUIManager.reference?.GetUI<UIOverhaulSettingsMenu>(UIConstants.UI_SETTINGS_MENU);
            if (!m_SettingsMenu)
            {
                OverhaulDebug.Error("CategoryButton - m_SettingsMenu is NULL! Manager existence: " + OverhaulUIManager.reference, EDebugType.UI);
                base.enabled = false;
                return;
            }

            Button button = base.GetComponent<Button>();
            button.AddOnClickListener(OnClicked);

            UIController.AssignValues(this);
        }

        public override void Start()
        {
            selected = m_SettingsMenu.selectedCategoryId == categoryId;
            text = categoryId;
            attachIcon();
        }

        protected override void OnDisposed()
        {
            if (m_Icon && m_Icon.texture)
                Destroy(m_Icon.texture);
        }

        private void attachIcon()
        {
            string path = OverhaulMod.Core.ModDirectory + "Assets/Settings/" + categoryId + ".png";
            if (!File.Exists(path))
            {
                path = OverhaulMod.Core.ModDirectory + "Assets/Settings/UnknownCategory.png";
            }

            OverhaulDownloadInfo overhaulDownloadInfo = new OverhaulDownloadInfo();
            overhaulDownloadInfo.DoneAction = delegate
            {
                if (overhaulDownloadInfo.Error)
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

            m_SettingsMenu.selectedCategoryId = categoryId;
            selected = true;
            m_SettingsMenu.recentSelectedCategoryButton = this;
        }
    }
}
