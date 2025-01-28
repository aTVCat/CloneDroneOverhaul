using OverhaulMod.Content;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIAddonsDownloadEditor : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElement("AddonDisplay", false)]
        private readonly ModdedObject m_addonDisplay;

        [UIElement("Content")]
        private readonly Transform m_container;

        [UIElementAction(nameof(OnSaveButtonClicked))]
        [UIElement("SaveButton")]
        private readonly Button m_saveButton;

        [UIElementAction(nameof(OnNewAddonButtonClicked))]
        [UIElement("NewAddonButton")]
        private readonly Button m_newAddonButton;

        [UIElement("NeedsSaveIcon", false)]
        private readonly GameObject m_needsSaveIcon;

        private List<UIElementAddonEditorDownloadDisplay> m_instantiatedEntries;

        protected override void OnInitialized()
        {
            m_instantiatedEntries = new List<UIElementAddonEditorDownloadDisplay>();
        }

        public override void Show()
        {
            base.Show();
            populate();
        }

        private void populate()
        {
            m_instantiatedEntries.Clear();
            if (m_container.childCount != 0)
                TransformUtils.DestroyAllChildren(m_container);

            AddonDownloadListInfo downloads = AddonManager.Instance.GetDownloadsFromDisk();
            foreach (AddonDownloadInfo download in downloads.Addons)
            {
                ModdedObject moddedObject = Instantiate(m_addonDisplay, m_container);
                moddedObject.gameObject.SetActive(true);
                UIElementAddonEditorDownloadDisplay addonEditorDownloadDisplay = moddedObject.gameObject.AddComponent<UIElementAddonEditorDownloadDisplay>();
                addonEditorDownloadDisplay.Initialize(download, downloads);
                m_instantiatedEntries.Add(addonEditorDownloadDisplay);
            }
        }

        public void RemoveEntry(UIElementAddonEditorDownloadDisplay downloadDisplay)
        {
            m_instantiatedEntries.Remove(downloadDisplay);
        }

        public void OnSaveButtonClicked()
        {
            foreach (UIElementAddonEditorDownloadDisplay entry in m_instantiatedEntries)
            {
                entry.UpdateAddonDownloadInfo();
            }
            AddonManager.Instance.SaveDownloadsToDisk();
        }

        public void OnNewAddonButtonClicked()
        {
            m_newAddonButton.interactable = false;
            DelegateScheduler.Instance.Schedule(delegate
            {
                if (m_newAddonButton)
                    m_newAddonButton.interactable = true;
            }, 1f);

            AddonDownloadInfo addonDownloadInfo = new AddonDownloadInfo();
            AddonDownloadListInfo downloads = AddonManager.Instance.GetDownloadsFromDisk();
            downloads.Addons.Add(addonDownloadInfo);
            ModdedObject moddedObject = Instantiate(m_addonDisplay, m_container);
            moddedObject.gameObject.SetActive(true);
            UIElementAddonEditorDownloadDisplay addonEditorDownloadDisplay = moddedObject.gameObject.AddComponent<UIElementAddonEditorDownloadDisplay>();
            addonEditorDownloadDisplay.Initialize(addonDownloadInfo, downloads);
            m_instantiatedEntries.Add(addonEditorDownloadDisplay);
        }
    }
}
