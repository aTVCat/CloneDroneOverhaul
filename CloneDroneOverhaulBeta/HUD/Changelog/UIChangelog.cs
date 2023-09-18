using CDOverhaul.NetworkAssets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class UIChangelog : UIController
    {
        [UIElementActionReference(nameof(OnGoBackButtonClicked))]
        [UIElementReference("BackButton")]
        private readonly Button m_GoBackButton;

        [UIElementActionReference(nameof(OnAboutModClicked))]
        [UIElementReference("AboutModButton")]
        private readonly Button m_AboutModButton;

        [UIElementActionReference(nameof(OnOldVersionsButtonClicked))]
        [UIElementReference("OldVersionsButton")]
        private readonly Button m_OldVersionButton;

        [UIElementReference("OldVersionsPanel")]
        private readonly GameObject m_OldVersionsPanel;

        [UIElementActionReference(nameof(OnCloseOldVersionsButtonClicked))]
        [UIElementReference("CloseOldVersionsPanelButton")]
        private readonly Button m_CloseOldVersionsPanelButton;

        [PrefabContainer(4, 5)]
        private readonly PrefabContainer m_ChangelogEntriesContainer;

        [PrefabContainer(7, 8)]
        private readonly PrefabContainer m_ImagesContainer;

        [UIElementReference("ChangelogText")]
        private readonly Text m_ChangelogText;
        [UIElementReference("VersionTitle")]
        private readonly Text m_ChangelogHeader;

        private readonly List<Texture> m_LoadedImages = new List<Texture>();

        public int ViewingChangelogIndex = -1;

        protected override bool HideTitleScreen() => true;
        protected override bool WaitForEscapeKeyToHide() => true;

        public override void Show()
        {
            base.Show();
            ViewChangelog(0);
        }

        public override void Hide()
        {
            base.Hide();
            DestroyLoadedPictures();
        }

        public void ViewChangelog(int index)
        {
            DestroyLoadedPictures();
            OnCloseOldVersionsButtonClicked();
            m_ImagesContainer.Clear();
            m_ChangelogText.text = string.Empty;

            List<Changelogs.PatchInfo> list = Changelogs.AllChangelogs;
            if (!base.gameObject.activeInHierarchy || list.IsNullOrEmpty())
                return;

            ViewingChangelogIndex = Mathf.Clamp(index, 0, list.Count - 1);
            Changelogs.PatchInfo info = list[index];
            string langID = LocalizationManager.Instance.GetCurrentLanguageCode();
            if (!File.Exists(info.DirectoryPath + "Info" + langID + ".txt"))
            {
                langID = "en";
            }

            m_ChangelogHeader.text = "Version " + info.TargetModVersion;
            m_ChangelogText.text = OverhaulCore.ReadText(info.DirectoryPath + "Info" + langID + ".txt");
            if (!info.Images.IsNullOrEmpty())
            {
                foreach (string art in info.Images)
                {
                    ModdedObject moddedObject = m_ImagesContainer.InstantiateEntry();
                    moddedObject.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        if (moddedObject != null)
                        {
                            RawImage i = moddedObject.GetComponent<RawImage>();
                            UIConstants.ShowImageViewer((Texture2D)i.texture);
                        }
                    });
                    _ = base.StartCoroutine(loadPicture(moddedObject.GetComponent<RawImage>(), info.DirectoryPath + art));
                }
            }
        }

        public void DestroyLoadedPictures()
        {
            if (m_LoadedImages.IsNullOrEmpty())
                return;

            foreach (Texture2D texture in m_LoadedImages)
            {
                if (texture)
                    Destroy(texture);
            }
            m_LoadedImages.Clear();
        }

        public void OnGoBackButtonClicked()
        {
            Hide();
        }

        public void OnAboutModClicked()
        {
            Hide();
            UIConstants.ShowAboutMod();
        }

        public void OnOldVersionsButtonClicked()
        {
            m_OldVersionsPanel.gameObject.SetActive(true);

            m_ChangelogEntriesContainer.Clear();
            if (Changelogs.AllChangelogs.IsNullOrEmpty())
                return;

            int index = 0;
            foreach (Changelogs.PatchInfo info in Changelogs.AllChangelogs)
            {
                int index2 = index;

                ModdedObject m = m_ChangelogEntriesContainer.InstantiateEntry();
                m.GetComponent<Button>().onClick.AddListener(delegate
                {
                    ViewChangelog(index2);
                    m_OldVersionsPanel.gameObject.SetActive(false);
                });
                m.GetObject<Text>(0).text = "Version " + info.TargetModVersion;
                m.GetObject<Transform>(1).gameObject.SetActive(index == 0);

                index++;
            }
        }

        public void OnCloseOldVersionsButtonClicked()
        {
            m_OldVersionsPanel.gameObject.SetActive(false);
        }

        private IEnumerator loadPicture(RawImage attachTo, string path)
        {
            if (!File.Exists(path))
            {
                attachTo.gameObject.SetActive(false);
                yield break;
            }

            OverhaulDownloadInfo handler = new OverhaulDownloadInfo();
            handler.DoneAction = delegate
            {
                if (attachTo)
                {
                    if (handler == null || handler.Error)
                    {
                        attachTo.gameObject.SetActive(false);
                        return;
                    }
                    attachTo.texture = handler.DownloadedTexture;
                    m_LoadedImages.Add(handler.DownloadedTexture);
                }
            };
            OverhaulNetworkAssetsController.DownloadTexture("file://" + path, handler);
            yield break;
        }
    }
}