using CDOverhaul.NetworkAssets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class ChangelogUI : OverhaulUIVer2
    {
        [ActionReference(nameof(OnGoBackButtonClicked))]
        [ObjectReference("BackButton")]
        private readonly Button m_GoBackButton;

        [ActionReference(nameof(OnAboutModClicked))]
        [ObjectReference("AboutModButton")]
        private readonly Button m_AboutModButton;

        [ActionReference(nameof(OnOldVersionsButtonClicked))]
        [ObjectReference("OldVersionsButton")]
        private readonly Button m_OldVersionButton;

        [ObjectReference("OldVersionsPanel")]
        private readonly GameObject m_OldVersionsPanel;

        [ActionReference(nameof(OnCloseOldVersionsButtonClicked))]
        [ObjectReference("CloseOldVersionsPanelButton")]
        private readonly Button m_CloseOldVersionsPanelButton;

        private PrefabAndContainer m_ChangelogEntriesContainer;

        [ObjectReference("ChangelogText")]
        private readonly Text m_ChangelogText;
        [ObjectReference("VersionTitle")]
        private readonly Text m_ChangelogHeader;

        private readonly List<Texture> m_LoadedImages = new List<Texture>();

        public int ViewingChangelogIndex = -1;

        public override void Show()
        {
            base.Show();
            ViewChangelog(0);
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);
        }

        public override void Hide()
        {
            base.Hide();
            DestroyLoadedPictures();
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(GameModeManager.IsOnTitleScreen());
        }

        public void ViewChangelog(int index)
        {
            if (m_ChangelogEntriesContainer == null)
                m_ChangelogEntriesContainer = new PrefabAndContainer(MyModdedObject, 4, 5);

            DestroyLoadedPictures();
            OnCloseOldVersionsButtonClicked();
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
            GetController<AboutOverhaulMenu>().Show();
        }

        public void OnOldVersionsButtonClicked()
        {
            m_OldVersionsPanel.gameObject.SetActive(true);

            m_ChangelogEntriesContainer.ClearContainer();
            if (Changelogs.AllChangelogs.IsNullOrEmpty())
                return;

            int index = 0;
            foreach (Changelogs.PatchInfo info in Changelogs.AllChangelogs)
            {
                int index2 = index;

                ModdedObject m = m_ChangelogEntriesContainer.CreateNew();
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