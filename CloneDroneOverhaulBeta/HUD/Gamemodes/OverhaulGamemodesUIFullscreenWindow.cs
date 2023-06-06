using Steamworks;
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
    public class OverhaulGamemodesUIFullscreenWindow : OverhaulBehaviour
    {
        public bool IsActive => base.gameObject.activeSelf;
        public bool StateSwitchingInProgress;

        private Image m_Shading;
        private Transform m_PanelTransform;

        private Action m_Callback;

        public void Initialize()
        {
            ModdedObject moddedObject = base.GetComponent<ModdedObject>();
            m_Shading = moddedObject.GetObject<Image>(0);
            m_PanelTransform = moddedObject.GetObject<Transform>(1);

            base.gameObject.SetActive(false);
        }

        public void Show(Action onPressedStartButtonCallback)
        {
            if (IsActive || StateSwitchingInProgress)
                return;

            m_Callback = onPressedStartButtonCallback;
            StaticCoroutineRunner.StartStaticCoroutine(showAnimationCoroutine());
        }

        public void Hide()
        {
            if (!IsActive || StateSwitchingInProgress)
                return;

            StaticCoroutineRunner.StartStaticCoroutine(hideAnimationCoroutine());
        }

        public void OnPressedStart()
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                OnPressedStart();

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
                Hide();
        }
    }
}
