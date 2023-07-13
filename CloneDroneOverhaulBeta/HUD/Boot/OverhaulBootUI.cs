using CDOverhaul.HUD;
using CDOverhaul.HUD.Vanilla;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul
{
    public class OverhaulBootUI : OverhaulBehaviour
    {
        private static bool m_HasEverShownUI;

        public static OverhaulBootUI Instance;
        public static bool IsActive => Instance != null;

        private Slider m_LoadingBar;

        private float m_ProgressLastFrame;

        public static bool Show()
        {
            if (!OverhaulFeatureAvailabilitySystem.ImplementedInBuild.IsBootScreenEnabled || m_HasEverShownUI || !GameModeManager.IsOnTitleScreen())
                return false;

            m_HasEverShownUI = true;
            GameObject prefab = OverhaulAssetsController.GetAsset("OverhaulBootUI", OverhaulAssetPart.Preload);
            GameObject spawnedPrefab = UnityEngine.Object.Instantiate(prefab);
            ModdedObject moddedObject = spawnedPrefab.GetComponent<ModdedObject>();
            Transform mainViewTransform = moddedObject.GetObject<Transform>(0);
            OverhaulCanvasController.ParentTransformToGameUIRoot(mainViewTransform);

            Instance = mainViewTransform.gameObject.AddComponent<OverhaulBootUI>();
            Instance.m_LoadingBar = moddedObject.GetObject<Slider>(1);
            Instance.m_LoadingBar.value = 0f;

            Destroy(spawnedPrefab);
            return true;
        }

        private void Start()
        {
            Animator animator = GetComponent<Animator>();
            animator.speed = 0f;

            ArenaCameraManager.Instance.TitleScreenLogoCamera.gameObject.SetActive(false);
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(false);
            GameUIRoot.Instance.TitleScreenUI.Hide();

            _ = StartCoroutine(waitThenPlayAnimation());
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

            if (m_LoadingBar)
            {
                if (OverhaulMod.HasBootProcessEnded)
                {
                    m_LoadingBar.value = 1f;
                    return;
                }

                float newProgress = OverhaulAssetsController.GetAllAssetBundlesLoadPercent();
                if (newProgress > m_ProgressLastFrame)
                    m_LoadingBar.value = newProgress;
                m_ProgressLastFrame = newProgress;
            }
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void destroyUI()
        {
            VanillaUIImprovements uIImprovements = OverhaulController.GetController<VanillaUIImprovements>();
            if (uIImprovements && uIImprovements.TitleScreenUI && uIImprovements.TitleScreenUI.MessagePanel)
            {
                uIImprovements.TitleScreenUI.MessagePanel.PopulateTitleScreenMessage();
            }

            Destroy(gameObject);
            Instance = null;
            AudioListener.volume = SettingsManager.Instance.GetSoundVolume();
            Time.timeScale = 1f;
            GameUIRoot.Instance.TitleScreenUI.Show();
            GameUIRoot.Instance.TitleScreenUI.SetLogoAndRootButtonsVisible(true);
            ArenaCameraManager.Instance.TitleScreenLogoCamera.gameObject.SetActive(true);
            ArenaCameraManager.Instance.TitleScreenLogoCamera.GetComponent<Animator>().Play(string.Empty);
            if (OverhaulVersionLabel.Instance)
            {
                OverhaulVersionLabel.Instance.Refresh();
            }
        }

        private void playAnimation()
        {
            Animator animator = GetComponent<Animator>();
            animator.speed = 1f;
        }

        private IEnumerator waitThenPlayAnimation()
        {
            yield return new WaitForSecondsRealtime(4f);

            playAnimation();
            yield return new WaitForSecondsRealtime(2f);

            yield return StaticCoroutineRunner.StartStaticCoroutine(OverhaulMod.Core.LoadAsyncStuff());
            OverhaulMod.Core.LoadSyncStuff();

            yield return new WaitForSecondsRealtime(0.3f);
            destroyUI();

            yield break;
        }
    }
}
