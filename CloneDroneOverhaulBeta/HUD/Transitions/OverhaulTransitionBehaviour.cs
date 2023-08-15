using ModLibrary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CDOverhaul
{
    public class OverhaulTransitionBehaviour : OverhaulBehaviour
    {
        public const float UNSCALED_DELTA_TIME_MULTIPLIER = 12.5f;

        private static readonly object s_Mutex = new object();

        private Image m_Image;

        private Transform m_TextTransform;
        private Text m_Text;

        private bool m_IsRealTransition;
        private bool m_FadeOut;

        private bool m_HasInitialized;
        private float m_TimeToDestroy;
        private float m_TimeToDisconnect;
        private readonly float m_TimeToAllowColorUpdates;

        public void Initialize(bool fadeOut, bool realTransition = false)
        {
            if (!m_FadeOut)
                m_FadeOut = fadeOut;

            if (!m_IsRealTransition)
                m_IsRealTransition = realTransition;

            if (!m_HasInitialized)
            {
                ModdedObject moddedObject = base.GetComponent<ModdedObject>();
                m_Image = moddedObject.GetObject<Image>(0);
                m_Image.color = fadeOut ? Color.black : Color.clear;
                m_TextTransform = moddedObject.GetObject<Transform>(1);
                m_TextTransform.localScale = fadeOut ? Vector3.one : Vector3.one * 1.5f;
                m_Text = m_TextTransform.GetComponent<Text>();
                m_Text.color = fadeOut ? Color.white : Color.clear;
                m_HasInitialized = true;
            }

            m_Image.color = fadeOut ? Color.black : Color.clear;
            m_TimeToDestroy = fadeOut ? Time.unscaledTime + 0.75f : -1f;
            m_TimeToDisconnect = realTransition ? Time.unscaledTime + 0.5f : -1f;
            //m_TimeToAllowColorUpdates = fadeOut ? Time.unscaledTime + 0.25f : -1f;
        }

        private void LateUpdate()
        {
            if (IsDisposedOrDestroyed())
                return;

            bool settingsManagerAvailable = SettingsManager.Instance && SettingsManager.Instance.IsInitialized();
            float setVolume = settingsManagerAvailable ? SettingsManager.Instance.GetSoundVolume() : 1f;

            float time = Time.unscaledTime;
            if (time >= m_TimeToAllowColorUpdates)
            {
                float multiplier = Time.unscaledDeltaTime * UNSCALED_DELTA_TIME_MULTIPLIER;

                Color currentColor = m_Image.color;
                currentColor.a = Mathf.Lerp(currentColor.a, m_FadeOut ? 0f : 1f, multiplier);
                m_Image.color = currentColor;

                Color textColor = Color.white;
                textColor.a = currentColor.a;
                m_Text.color = textColor;

                Vector3 currentScale = m_TextTransform.localScale;
                currentScale.x = Mathf.Lerp(currentScale.x, m_FadeOut ? 1.5f : 1f, multiplier * 0.75f);
                currentScale.y = Mathf.Lerp(currentScale.y, m_FadeOut ? 1.5f : 1f, multiplier * 0.75f);
                currentScale.z = Mathf.Lerp(currentScale.z, m_FadeOut ? 1.5f : 1f, multiplier * 0.75f);
                m_TextTransform.localScale = currentScale;
            }

            AudioListener.volume = !m_FadeOut && m_IsRealTransition ? (1f - m_Image.color.a) * setVolume : setVolume;
            if (m_TimeToDestroy != -1f && time >= m_TimeToDestroy)
            {
                DestroyGameObject();
                AudioListener.volume = setVolume;
            }

            if (m_IsRealTransition && m_TimeToDisconnect != -1f && time >= m_TimeToDisconnect)
            {
                Disconnect();
                m_TimeToDisconnect = -1f;
            }
        }

        private void Disconnect()
        {
            SceneTransitionManager sceneTransitionManager = SceneTransitionManager.Instance;
            sceneTransitionManager.SetPrivateField<bool>("_isExitingToMainMenu", true);
            GlobalEventManager.Instance.Dispatch("ExitingToMainMenu");
            sceneTransitionManager.SetPrivateField<bool>("_isDisconnecting", true);

            SceneTransitionManager.LastDisconnectTime = Time.realtimeSinceStartup;
            SceneTransitionManager.LastDisconnectHadBoltRunning = false;

            if (BoltNetwork.IsConnected || BoltNetwork.IsRunning)
            {
                sceneTransitionManager.SetPrivateField<bool>("_isDisconnecting", true);
                SceneTransitionManager.LastDisconnectHadBoltRunning = true;

                bool shutdown = false;
                object obj = s_Mutex;
                lock (obj)
                {
                    if (!sceneTransitionManager.GetPrivateField<bool>("_isBoltDisconnectInProgress"))
                    {
                        shutdown = true;
                        sceneTransitionManager.SetPrivateField<bool>("_isBoltDisconnectInProgress", true);
                    }
                }

                if (!shutdown)
                    return;

                try
                {
                    BoltLauncher.Shutdown();
                    return;
                }
                catch
                {
                    sceneTransitionManager.SetPrivateField<bool>("_isDisconnecting", false);
                    SceneManager.LoadScene("Gameplay");
                    return;
                }
            }

            sceneTransitionManager.SetPrivateField<bool>("_isDisconnecting", false);
            SceneManager.LoadScene("Gameplay");
        }
    }
}
