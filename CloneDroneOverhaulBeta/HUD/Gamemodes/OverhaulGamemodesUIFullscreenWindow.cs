using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Gamemodes
{
    public class OverhaulGamemodesUIFullscreenWindow : OverhaulBehaviour
    {
        public static readonly Vector2 DefaultWindowSize = new Vector2(300, 150);
        public static readonly Vector2 GameCustomizationWindowSize = new Vector2(400, 425);
        public static readonly Vector2 CodeEnterWindowSize = new Vector2(275, 100);

        public OverhaulGamemodesUI GamemodesUI;

        public bool IsActive => base.gameObject.activeSelf;
        public bool StateSwitchingInProgress;
        public bool AllowPressingBackspace;

        private Image m_Shading;
        private RectTransform m_PanelTransform;
        private CanvasGroup m_CanvasGroup;

        private readonly GameObject[] PageGameObjects = new GameObject[6];

        private Action m_Callback;

        public void Initialize()
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            m_Shading = moddedObject.GetObject<Image>(0);
            m_PanelTransform = moddedObject.GetObject<RectTransform>(1);
            m_CanvasGroup = moddedObject.GetObject<CanvasGroup>(5);

            PageGameObjects[0] = moddedObject.GetObject<Transform>(2).gameObject;
            PageGameObjects[0].AddComponent<DifficultySelectionPage>().Initialize(this);
            PageGameObjects[1] = moddedObject.GetObject<Transform>(3).gameObject;
            PageGameObjects[1].AddComponent<HostTypeSelectionPage>().Initialize(this);
            PageGameObjects[2] = moddedObject.GetObject<Transform>(4).gameObject;
            PageGameObjects[2].AddComponent<LBSGameCustomization>().Initialize(this);
            PageGameObjects[3] = moddedObject.GetObject<Transform>(6).gameObject;
            PageGameObjects[3].AddComponent<CodeEnterPage>().Initialize(this);
            PageGameObjects[4] = moddedObject.GetObject<Transform>(7).gameObject;
            PageGameObjects[4].AddComponent<SandboxActionSelectionPage>().Initialize(this);
            PageGameObjects[5] = moddedObject.GetObject<Transform>(8).gameObject;
            PageGameObjects[5].AddComponent<SandboxCodeEnterPage>().Initialize(this);

            base.gameObject.SetActive(false);
        }

        public void Show(Action onPressedStartButtonCallback, int windowIndex)
        {
            if (IsActive || StateSwitchingInProgress)
                return;

            ShowPage(windowIndex);

            m_Callback = onPressedStartButtonCallback;
            _ = StaticCoroutineRunner.StartStaticCoroutine(showAnimationCoroutine());
        }

        public void Hide(bool forceHideIfSwitching = false)
        {
            if (!IsActive || StateSwitchingInProgress)
            {
                if (!forceHideIfSwitching)
                    return;

                base.gameObject.SetActive(false);
                return;
            }

            _ = StaticCoroutineRunner.StartStaticCoroutine(hideAnimationCoroutine());
        }

        public void ShowPage(int index)
        {
            int i = 0;
            foreach (GameObject gameObject in PageGameObjects)
            {
                if (!gameObject)
                {
                    i++;
                    continue;
                }

                FullscreenWindowPageBase pageBase = gameObject.GetComponent<FullscreenWindowPageBase>();
                if (!pageBase)
                {
                    i++;
                    continue;
                }

                if (i == index)
                {
                    gameObject.SetActive(true);
                    m_PanelTransform.sizeDelta = pageBase.GetWindowSize();
                    AllowPressingBackspace = pageBase.AllowPressingBackspace();
                }
                else
                    gameObject.SetActive(false);

                i++;
            }
        }

        public void GoToPage(int index)
        {
            FullscreenWindowPageBase pageBase = PageGameObjects[index].GetComponent<FullscreenWindowPageBase>();
            if (!pageBase)
                return;

            _ = StaticCoroutineRunner.StartStaticCoroutine(transitionToDifferentPage(pageBase.GetWindowSize(), index));
        }

        public void DoQuickStart()
        {
            if (StateSwitchingInProgress || m_Callback == null)
                return;

            m_Callback();
            m_Callback = null;
        }

        private IEnumerator showAnimationCoroutine()
        {
            StateSwitchingInProgress = true;

            base.gameObject.SetActive(true);
            m_Shading.color = Color.clear;
            m_PanelTransform.localScale = Vector3.one * 0.2f;
            m_PanelTransform.gameObject.SetActive(true);

            iTween.ScaleTo(m_PanelTransform.gameObject, Vector3.one, 0.3f);
            for (int i = 0; i < 10; i++)
            {
                Color color = m_Shading.color;
                color.a += 0.05f;
                m_Shading.color = color;

                yield return new WaitForSecondsRealtime(0.016f);
            }
            yield return new WaitForSecondsRealtime(0.16f);

            StateSwitchingInProgress = false;
            yield break;
        }

        private IEnumerator hideAnimationCoroutine()
        {
            StateSwitchingInProgress = true;

            m_Shading.color = new Color(0, 0, 0, 0.5f);
            m_PanelTransform.gameObject.SetActive(false);

            yield return new WaitForSecondsRealtime(0.1f);
            for (int i = 0; i < 10; i++)
            {
                Color color = m_Shading.color;
                color.a -= 0.05f;
                m_Shading.color = color;

                yield return new WaitForSecondsRealtime(0.016f);
            }
            base.gameObject.SetActive(false);

            StateSwitchingInProgress = false;
            yield break;
        }

        private IEnumerator transitionToDifferentPage(Vector2 targetSize, int pageIndex)
        {
            StateSwitchingInProgress = true;
            OverhaulCanvasController.SetCanvasPixelPerfect(false);

            for (int i2 = 0; i2 < 4; i2++)
            {
                m_PanelTransform.sizeDelta += (targetSize - m_PanelTransform.sizeDelta) * 0.25f;
                m_CanvasGroup.alpha -= 0.25f;
                yield return new WaitForSecondsRealtime(0.016f);
            }
            ShowPage(-1);

            int i = 0;
            while (i < 16)
            {
                m_PanelTransform.sizeDelta += (targetSize - m_PanelTransform.sizeDelta) * 0.25f;
                yield return new WaitForSecondsRealtime(0.016f);

                i++;
            }
            m_PanelTransform.sizeDelta = targetSize;
            m_CanvasGroup.alpha = 0f;

            ShowPage(pageIndex);
            OverhaulCanvasController.SetCanvasPixelPerfect(true);
            for (int i2 = 0; i2 < 4; i2++)
            {
                m_CanvasGroup.alpha += 0.25f;
                yield return new WaitForSecondsRealtime(0.016f);
            }

            StateSwitchingInProgress = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                DoQuickStart();

            if (AllowPressingBackspace && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace)))
                Hide();
        }
    }
}
