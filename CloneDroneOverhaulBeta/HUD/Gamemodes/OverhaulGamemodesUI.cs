using CDOverhaul.NetworkAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class OverhaulGamemodesUI : OverhaulUI
    {
        public ChapterSelectionUI OverhaulChapterSelectionUI;
        public LastBotStandingPlayUI OverhaulLastBotStandingPlayUI;
        public ChallengesUI OverhaulChallengesUI;
        public DuelPlayUI OverhaulDuelPlayUI;

        public OverhaulGamemodesUIFullscreenWindow FullscreenWindow;

        private RawImage m_Background;
        private RawImage m_LastSpawnedBackground;
        private Transform m_BackgroundContainer;

        public override void Initialize()
        {
            base.gameObject.SetActive(false);

            m_Background = MyModdedObject.GetObject<RawImage>(0);
            m_Background.gameObject.SetActive(false);
            m_BackgroundContainer = MyModdedObject.GetObject<Transform>(6);
            FullscreenWindow = MyModdedObject.GetObject<Transform>(1).gameObject.AddComponent<OverhaulGamemodesUIFullscreenWindow>();
            FullscreenWindow.Initialize();
            FullscreenWindow.GamemodesUI = this;

            OverhaulChapterSelectionUI = MyModdedObject.GetObject<Transform>(2).gameObject.AddComponent<ChapterSelectionUI>().Initialize<ChapterSelectionUI>(this);
            OverhaulLastBotStandingPlayUI = MyModdedObject.GetObject<Transform>(3).gameObject.AddComponent<LastBotStandingPlayUI>().Initialize<LastBotStandingPlayUI>(this);
            OverhaulChallengesUI = MyModdedObject.GetObject<Transform>(5).gameObject.AddComponent<ChallengesUI>().Initialize<ChallengesUI>(this);
            OverhaulDuelPlayUI = MyModdedObject.GetObject<Transform>(7).gameObject.AddComponent<DuelPlayUI>().Initialize<DuelPlayUI>(this);

            GameModeCardData[] datas = GameUIRoot.Instance.TitleScreenUI.SingleplayerModeSelectScreen.GameModeData;
            datas[0].ClickedCallback = new UnityEngine.Events.UnityEvent();
            datas[0].ClickedCallback.AddListener(delegate
            {
                ShowWithUI(0);
            });
            datas[1].ClickedCallback = new UnityEngine.Events.UnityEvent();
            datas[1].ClickedCallback.AddListener(delegate
            {
                OverhaulTransitionController.DoTransitionWithAction(delegate
                {
                    GameUIRoot.Instance.TitleScreenUI.OnPlayEndlessButtonClicked();
                }, () => CharacterTracker.Instance.GetPlayer(), 0.35f);
            });
            replaceOldImageWithNew(datas[1], "Endless.jpg");
            datas[2].ClickedCallback = new UnityEngine.Events.UnityEvent();
            datas[2].ClickedCallback.AddListener(delegate
            {
                OverhaulChallengesUI.ViewCoopChallenges = false;
                ShowWithUI(2);
            });
            replaceOldImageWithNew(datas[2], "Bot.jpg");

            GameModeCardData[] datas2 = GameUIRoot.Instance.TitleScreenUI.MultiplayerModeSelectScreen.GameModeData;
            replaceOldImageWithNew(datas2[0], "Humans.jpg");
            datas2[1].ClickedCallback = new UnityEngine.Events.UnityEvent();
            datas2[1].ClickedCallback.AddListener(delegate
            {
                OverhaulChallengesUI.ViewCoopChallenges = true;
                ShowWithUI(2);
            });
            replaceOldImageWithNew(datas2[1], "Bot.jpg");
            datas2[2].ClickedCallback = new UnityEngine.Events.UnityEvent();
            datas2[2].ClickedCallback.AddListener(delegate
            {
                ShowWithUI(1);
            });
            replaceOldImageWithNew(datas2[2], "Raptor.jpg");
            datas2[3].ClickedCallback = new UnityEngine.Events.UnityEvent();
            datas2[3].ClickedCallback.AddListener(delegate
            {
                ShowWithUI(3);
            });
            replaceOldImageWithNew(datas2[3], "DuelHumans.jpg");

            ShowWithUI(-1);
        }

        private void replaceOldImageWithNew(GameModeCardData data, string fileNameUNderPreviewsFolder)
        {
            OverhaulDownloadInfo info = new OverhaulDownloadInfo();
            info.DoneAction = delegate
            {
                if (info.Error)
                    return;

                Sprite sprite = (info.DownloadedTexture as Texture2D).ToSprite();
                data.ThumbnailSprite = sprite;
            };
            OverhaulNetworkAssetsController.DownloadTexture("file://" + OverhaulCore.ModDirectoryStatic + "Assets/Previews/" + fileNameUNderPreviewsFolder, info);
        }

        protected override void OnDisposed()
        {
            OverhaulChapterSelectionUI.DestroyBehaviour();
            base.OnDisposed();
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
            DestroyBackground();
        }

        public void ShowWithUI(int index)
        {
            if (index != -1)
            {
                Show();
            }

            OverhaulChapterSelectionUI.Hide();
            OverhaulLastBotStandingPlayUI.Hide();
            OverhaulChallengesUI.Hide();
            OverhaulDuelPlayUI.Hide();
            switch (index)
            {
                case 0:
                    OverhaulChapterSelectionUI.Show();
                    break;
                case 1:
                    OverhaulLastBotStandingPlayUI.Show();
                    break;
                case 2:
                    OverhaulChallengesUI.Show();
                    break;
                case 3:
                    OverhaulDuelPlayUI.Show();
                    break;
            }
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
            FullscreenWindow.Hide();
        }

        public void ChangeBackgroundTexture(string filePath)
        {
            _ = StaticCoroutineRunner.StartStaticCoroutine(changeBackgroundTextureCoroutine(filePath));
        }

        private IEnumerator changeBackgroundTextureCoroutine(string filePath)
        {
            bool hasLoadedImage = false;

            RawImage newBG = Instantiate(m_Background, m_BackgroundContainer);
            newBG.gameObject.SetActive(true);
            newBG.color = Color.clear;

            OverhaulDownloadInfo overhaulDownloadInfo = new OverhaulDownloadInfo();
            overhaulDownloadInfo.DoneAction = delegate
            {
                hasLoadedImage = true;

                if (!newBG)
                    return;

                newBG.texture = overhaulDownloadInfo.DownloadedTexture;
            };
            OverhaulNetworkAssetsController.DownloadTexture("file://" + filePath, overhaulDownloadInfo);

            yield return new WaitUntil(() => hasLoadedImage);
            for (int i = 0; i < 4; i++)
            {
                newBG.color = new Color(newBG.color.r + 0.25f, newBG.color.g + 0.25f, newBG.color.b + 0.25f, newBG.color.a + 0.25f);
                yield return new WaitForSecondsRealtime(0.016f);
            }
            newBG.color = Color.white;

            DestroyBackground();
            m_LastSpawnedBackground = newBG;

            yield break;
        }

        public void DestroyBackground()
        {
            if (m_LastSpawnedBackground)
            {
                if (m_LastSpawnedBackground.texture)
                {
                    Destroy(m_LastSpawnedBackground.texture);
                }
                Destroy(m_LastSpawnedBackground.gameObject);
            }
        }
    }
}
