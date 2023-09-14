using CDOverhaul.NetworkAssets;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Vanilla
{
    public class TitleScreenUIImprovements : OverhaulBehaviour
    {
        private ModdedObject m_ModdedObject;

        public OverhauledMessagePanel MessagePanel;

        public override void Start()
        {
            m_ModdedObject = base.GetComponent<ModdedObject>();
            MessagePanel = m_ModdedObject.GetObject<Transform>(0).gameObject.AddComponent<OverhauledMessagePanel>();
            MessagePanel.Initialize(this);
        }

        private void Update()
        {
            MessagePanel.gameObject.SetActive(OverhauledMessagePanel.Show && GameUIRoot.Instance.TitleScreenUI.TitleScreenMessagePanel.activeInHierarchy);
        }

        public class OverhauledMessagePanel : OverhaulBehaviour
        {
            public static bool Show = true;

            public TitleScreenUIImprovements UIImprovements;
            public TitleScreenMessagePanel MessagePanel;

            private ModdedObject m_ModdedObject;
            private Text m_Header;
            private Text m_Description;

            private Button m_CloseButton;

            private Image m_Image;
            private Button m_ImageButton;
            private RectTransform m_MainHolderTransform;

            private Button m_LinkButton;
            private Text m_LinkButtonText;

            private Texture m_DownloadedTexture;
            private string m_Link;

            public void Initialize(TitleScreenUIImprovements ui)
            {
                UIImprovements = ui;

                GlobalEventManager.Instance.AddEventListener(GlobalEvents.PlayfabTitleDataDownloaded, new Action(onGetTitleScreenMessageData));

                m_ModdedObject = base.GetComponent<ModdedObject>();
                m_Header = m_ModdedObject.GetObject<Text>(1);
                m_Description = m_ModdedObject.GetObject<Text>(2);
                m_Image = m_ModdedObject.GetObject<Image>(0);
                m_Image.enabled = false;
                m_ImageButton = m_Image.GetComponent<Button>();
                m_ImageButton.onClick.AddListener(delegate
                {
                    UIConstants.ShowImageViewer((Texture2D)m_DownloadedTexture);
                });
                m_ImageButton.enabled = false;
                m_CloseButton = m_ModdedObject.GetObject<Button>(4);
                m_CloseButton.onClick.AddListener(HideForThisSession);
                m_LinkButton = m_ModdedObject.GetObject<Button>(3);
                m_LinkButton.onClick.AddListener(OpenLink);
                m_LinkButtonText = m_ModdedObject.GetObject<Text>(5);
                m_MainHolderTransform = m_Image.rectTransform;

                ResetPanel();
            }

            public override void Start()
            {
                MessagePanel = GameUIRoot.Instance.TitleScreenUI.TitleScreenMessagePanel.GetComponent<TitleScreenMessagePanel>();
            }

            protected override void OnDisposed()
            {
                GlobalEventManager.Instance.RemoveEventListener(GlobalEvents.PlayfabTitleDataDownloaded, new Action(onGetTitleScreenMessageData));
                if (m_DownloadedTexture)
                {
                    Destroy(m_DownloadedTexture);
                }
            }

            private void onGetTitleScreenMessageData()
            {
                PopulateTitleScreenMessage();
            }

            public void ResetPanel()
            {
                m_Image.enabled = false;
                m_ImageButton.enabled = false;
                m_Header.text = string.Empty;
                m_Description.text = string.Empty;
                m_LinkButton.gameObject.SetActive(false);
                m_LinkButtonText.text = string.Empty;
            }

            public void PopulateTitleScreenMessage()
            {
                ResetPanel();

                TitleScreenMessageData titleScreenMessageData = PlayfabLevelDownloadManager.Instance.GetTitleScreenMessageData();
                if (titleScreenMessageData == null)
                    return;

                m_Header.text = titleScreenMessageData.Title;
                m_Description.text = titleScreenMessageData.Body;
                m_LinkButton.gameObject.SetActive(true);
                m_LinkButtonText.text = titleScreenMessageData.ButtonText;
                m_Link = titleScreenMessageData.ButtonURL;
                RefreshTitleScreenMessageImage();
            }

            public void RefreshTitleScreenMessageImage()
            {
                TitleScreenMessageData titleScreenMessageData = PlayfabLevelDownloadManager.Instance.GetTitleScreenMessageData();
                if (titleScreenMessageData == null)
                    return;

                OverhaulDownloadInfo overhaulDownloadInfo = new OverhaulDownloadInfo();
                overhaulDownloadInfo.DoneAction = delegate
                {
                    if (overhaulDownloadInfo.Error)
                        return;

                    if (!overhaulDownloadInfo.DownloadedTexture)
                        return;

                    Sprite sprite = (overhaulDownloadInfo.DownloadedTexture as Texture2D).ToSprite();
                    float num = overhaulDownloadInfo.DownloadedTexture.height / (float)overhaulDownloadInfo.DownloadedTexture.width;
                    m_MainHolderTransform.sizeDelta = new Vector2(m_MainHolderTransform.sizeDelta.x, m_MainHolderTransform.rect.width * num);
                    m_Image.sprite = sprite;
                    m_Image.enabled = true;
                    m_ImageButton.enabled = true;
                    m_DownloadedTexture = overhaulDownloadInfo.DownloadedTexture;
                };
                OverhaulNetworkAssetsController.DownloadTexture(titleScreenMessageData.ImageURL, overhaulDownloadInfo);
            }

            public void HideForThisSession()
            {
                Show = false;
            }

            public void OpenLink()
            {
                Application.OpenURL(m_Link);
            }
        }
    }
}
