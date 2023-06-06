using CDOverhaul.NetworkAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class OverhaulGamemodesUI : OverhaulUI
    {
        public ChapterSelectionUI OverhaulChapterSelectionUI;
        public OverhaulGamemodesUIFullscreenWindow FullscreenWindow;

        private RawImage m_Background;

        private OverhaulNetworkDownloadHandler m_CurrentDownloadHandler;

        public bool AllowSwitching;

        public override void Initialize()
        {
            base.gameObject.SetActive(false);
            AllowSwitching = true;

            m_Background = MyModdedObject.GetObject<RawImage>(0);
            FullscreenWindow = MyModdedObject.GetObject<Transform>(1).gameObject.AddComponent<OverhaulGamemodesUIFullscreenWindow>();
            FullscreenWindow.Initialize();
            OverhaulChapterSelectionUI = MyModdedObject.GetObject<Transform>(2).gameObject.AddComponent<ChapterSelectionUI>().Initialize<ChapterSelectionUI>(this);

            if (!OverhaulFeatureAvailabilitySystem.BuildImplements.IsOverhaulGamemodesUIEnabled)
                return;

            GameModeCardData[] datas = GameUIRoot.Instance.TitleScreenUI.SingleplayerModeSelectScreen.GameModeData;
            datas[0].ClickedCallback = new UnityEngine.Events.UnityEvent();
            datas[0].ClickedCallback.AddListener(delegate
            {
                ShowWithUI(0);
            });

            ShowWithUI(-1);
        }

        protected override void OnDisposed()
        {
            OverhaulChapterSelectionUI.DestroyBehaviour();
            base.OnDisposed();
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
        }

        public void ShowWithUI(int index)
        {
            if (index != -1)
                Show();

            OverhaulChapterSelectionUI.Hide();
            switch (index)
            {
                case 0:
                    OverhaulChapterSelectionUI.Show();
                    break;
            }
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
            FullscreenWindow.Hide();
            ShowWithUI(-1);
        }

        public void ChangeBackgroundTexture(string filePath)
        {
            StaticCoroutineRunner.StartStaticCoroutine(changeBackgroundTextureCoroutine(filePath));
        }

        private IEnumerator changeBackgroundTextureCoroutine(string filePath)
        {
            AllowSwitching = false;

            m_Background.color = Color.white;
            for (int i = 0; i< 4; i++)
            {
                m_Background.color = new Color(m_Background.color.r - 0.25f, m_Background.color.g - 0.25f, m_Background.color.b - 0.25f, 1);
                yield return new WaitForSecondsRealtime(0.017f);
            }

            yield return null;

            m_CurrentDownloadHandler = new OverhaulNetworkDownloadHandler();
            m_CurrentDownloadHandler.DoneAction = delegate
            {
                if (m_Background.texture)
                    Destroy(m_Background.texture);

                m_Background.texture = m_CurrentDownloadHandler.DownloadedTexture;
            };
            OverhaulNetworkController.DownloadTexture("file://" + filePath, m_CurrentDownloadHandler);
            yield return new WaitForSecondsRealtime(0.15f);

            for (int i = 0; i < 4; i++)
            {
                m_Background.color = new Color(m_Background.color.r + 0.25f, m_Background.color.g + 0.25f, m_Background.color.b + 0.25f, 1);
                yield return new WaitForSecondsRealtime(0.017f);
            }
            m_Background.color = Color.white;
            AllowSwitching = true;
            yield break;
        }
    }
}
