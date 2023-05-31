using CDOverhaul.HUD;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul
{
    public class OverhaulBootUI : OverhaulBehaviour
    {
        public static OverhaulBootUI Instance;
        public static bool IsActive => Instance != null;

        private static bool m_HasEverShownUI;

        private float m_UnscaledTimeToHide;
        private bool m_IsPlayingAnimation;

        public static void Show()
        {
            if (!OverhaulFeatureAvailabilitySystem.BuildImplements.IsBootScreenEnabled || m_HasEverShownUI || !GameModeManager.IsOnTitleScreen())
                return;

            m_HasEverShownUI = true;
            GameObject prefab = OverhaulAssetsController.GetAsset("OverhaulBootUI", OverhaulAssetPart.Preload);
            GameObject spawnedPrefab = UnityEngine.Object.Instantiate(prefab);
            ModdedObject moddedObject = spawnedPrefab.GetComponent<ModdedObject>();
            Transform mainViewTransform = moddedObject.GetObject<Transform>(0);
            OverhaulCanvasController.ParentTransformToGameUIRoot(mainViewTransform);
            Instance = mainViewTransform.gameObject.AddComponent<OverhaulBootUI>();
            Destroy(spawnedPrefab);
        }

        private void Start()
        {
            Animator animator = GetComponent<Animator>();
            animator.speed = 0f;

            ArenaCameraManager.Instance.TitleScreenLogoCamera.gameObject.SetActive(false);
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);
            GameUIRoot.Instance.TitleScreenUI.Hide();

            StartCoroutine(waitThenPlayAnimation());
        }

        private void Update()
        {
            if (IsDisposedOrDestroyed()) return;
            if (ErrorManager.Instance.HasCrashed())
            {
                destroyUI();
                return;
            }

            AudioListener.volume = 0f;
            Time.timeScale = 0f;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void destroyUI()
        {
            Destroy(this.gameObject);
            Instance = null;
            AudioListener.volume = SettingsManager.Instance.GetSoundVolume();
            Time.timeScale = 1f;
            GameUIRoot.Instance.TitleScreenUI.Show();
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(true);
            ArenaCameraManager.Instance.TitleScreenLogoCamera.gameObject.SetActive(true);
            ArenaCameraManager.Instance.TitleScreenLogoCamera.GetComponent<Animator>().Play(string.Empty);
            if(OverhaulVersionLabel.Instance != null)
            {
                OverhaulVersionLabel.Instance.RefreshVersionLabel();
                OverhaulVersionLabel.Instance.ShowDiscordPanel();
            }
        }

        private void playAnimation()
        {
            m_IsPlayingAnimation = true;
            m_UnscaledTimeToHide = Time.unscaledTime + 5;
            Animator animator = GetComponent<Animator>();
            animator.speed = 1f;
        }

        private IEnumerator waitThenPlayAnimation()
        {
            yield return new WaitForSecondsRealtime(4f);

            playAnimation();
            yield return new WaitForSecondsRealtime(5f);
            destroyUI();

            yield break;
        }
    }
}
