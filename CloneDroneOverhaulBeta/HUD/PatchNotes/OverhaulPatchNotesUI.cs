using CDOverhaul.NetworkAssets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class OverhaulPatchNotesUI : OverhaulUI
    {
        private Button m_OkButton;
        private Button m_ModBotButton;
        private Button m_GitHubButton;
        private Button m_OldChangelogsButton;

        private Transform m_OldChangelogsPanel;
        private OverhaulUI.PrefabAndContainer m_ChangelogEntriesContainer;
        private Button m_CloseOldChangelogsButton;

        private OverhaulUI.PrefabAndContainer m_ArtContainer;
        private Text m_Changelog;
        private List<Texture> m_LoadedArt = new List<Texture>();

        public int ViewingChangelogIndex = -1;

        public override void Initialize()
        {
            m_OkButton = MyModdedObject.GetObject<Button>(0);
            m_OkButton.onClick.AddListener(OnOKClicked);
            m_GitHubButton = MyModdedObject.GetObject<Button>(1);
            m_GitHubButton.onClick.AddListener(OnGitHubClicked);
            m_ModBotButton = MyModdedObject.GetObject<Button>(2);
            m_ModBotButton.onClick.AddListener(OnModBotClicked);
            m_ArtContainer = new PrefabAndContainer(MyModdedObject, 3, 4);
            m_Changelog = MyModdedObject.GetObject<Text>(5);
            m_OldChangelogsButton = MyModdedObject.GetObject<Button>(10);
            m_OldChangelogsButton.onClick.AddListener(OnOldChangelogsClicked);
            m_CloseOldChangelogsButton = MyModdedObject.GetObject<Button>(9);
            m_CloseOldChangelogsButton.onClick.AddListener(OnCloseOldChangelogsClicked);
            m_OldChangelogsPanel = MyModdedObject.GetObject<Transform>(6);
            m_OldChangelogsPanel.gameObject.SetActive(false);
            m_ChangelogEntriesContainer = new PrefabAndContainer(MyModdedObject, 7, 8);
            Hide();
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();

            m_OkButton = null;
            m_ModBotButton = null;
            m_GitHubButton = null;
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
            ViewChangelog(0);
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);
        }

        public void Hide()
        {
            OverhaulUIDescriptionTooltip.SetActive(false);
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(true);
            base.gameObject.SetActive(false);
            DestroyLoadedPictures();
        }

        public void DestroyLoadedPictures()
        {
            if (!m_LoadedArt.IsNullOrEmpty())
            {
                int i = 0;
                do
                {
                    if (m_LoadedArt[i] != null)
                    {
                        Destroy(m_LoadedArt[i]);
                    }
                    i++;
                } while (i < m_LoadedArt.Count);
                m_LoadedArt.Clear();
            }
        }

        public void OnOKClicked()
        {
            Hide();
        }

        public void OnGitHubClicked()
        {
            Application.OpenURL("https://github.com/aTVCat/CloneDroneOverhaul/releases");
        }

        public void OnModBotClicked()
        {
            Application.OpenURL("https://modbot.org/modPreview.html?modID=rAnDomPaTcHeS1");
        }

        public void OnOldChangelogsClicked()
        {
            m_OldChangelogsPanel.gameObject.SetActive(true);

            m_ChangelogEntriesContainer.ClearContainer();
            if (OverhaulPatchNotes.AllChangelogs.IsNullOrEmpty())
            {
                return;
            }

            int index = 0;
            foreach (OverhaulPatchNotes.PatchInfo info in OverhaulPatchNotes.AllChangelogs)
            {
                int index2 = index;

                ModdedObject m = m_ChangelogEntriesContainer.CreateNew();
                m.GetComponent<Button>().onClick.AddListener(delegate
                {
                    ViewChangelog(index2);
                    m_OldChangelogsPanel.gameObject.SetActive(false);
                });
                m.GetObject<Text>(0).text = "Version " + info.TargetModVersion;
                m.GetObject<Transform>(1).gameObject.SetActive(ViewingChangelogIndex == index);

                index++;
            }
        }

        public void OnCloseOldChangelogsClicked()
        {
            m_OldChangelogsPanel.gameObject.SetActive(false);
        }

        public void ViewChangelog(int index)
        {
            DestroyLoadedPictures();
            m_ArtContainer.ClearContainer();
            m_Changelog.text = string.Empty;
            if (!base.gameObject.activeInHierarchy || OverhaulPatchNotes.AllChangelogs.IsNullOrEmpty())
            {
                ViewingChangelogIndex = -1;
                return;
            }
            ViewingChangelogIndex = index;

            OverhaulPatchNotes.PatchInfo info = OverhaulPatchNotes.AllChangelogs[index];
            m_Changelog.text = info.InformationString;

            if (!info.Art.IsNullOrEmpty())
            {
                foreach (string art in info.Art)
                {
                    ModdedObject m = m_ArtContainer.CreateNew();
                    m.GetComponent<Button>().onClick.AddListener(delegate
                    {
                        if (m != null)
                        {
                            RawImage i = m.GetComponent<RawImage>();
                            if (i != null && i.texture != null)
                            {
                                OverhaulUIImageViewer.SetActive(true, i.texture);
                            }
                        }
                    });
                    StaticCoroutineRunner.StartStaticCoroutine(loadPicture(m.GetComponent<RawImage>(), info.DirectoryPath + art));
                }
            }

            OverhaulUIDescriptionTooltip.SetActive(true, "Patch notes: Version " + info.TargetModVersion, "All about latest builds!");
        }

        private IEnumerator loadPicture(RawImage attachTo, string path)
        {
            yield return null;
            if (attachTo == null)
            {
                yield break;
            }

            if (!File.Exists(path))
            {
                attachTo.gameObject.SetActive(false);
                yield break;
            }

            OverhaulNetworkDownloadHandler handler = new OverhaulNetworkDownloadHandler();
            handler.DoneAction = delegate
            {
                if (attachTo != null)
                {
                    if (handler == null || handler.Error)
                    {
                        attachTo.gameObject.SetActive(false);
                        return;
                    }
                    attachTo.texture = handler.DownloadedTexture;
                    m_LoadedArt.Add(handler.DownloadedTexture);
                }
            };
            OverhaulNetworkController.DownloadTexture("file://" + path, handler);
            yield break;
        }
    }
}