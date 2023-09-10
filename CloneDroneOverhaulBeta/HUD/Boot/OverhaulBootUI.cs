using CDOverhaul.HUD;
using CDOverhaul.HUD.Vanilla;
using CDOverhaul.NetworkAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul
{
    public class OverhaulBootUI : OverhaulBehaviour
    {
        public static bool HasBeenInstantiated;

        private Slider m_LoadingBar;
        private Image m_Shading;
        private Animator m_Animator;

        private ModInitialize m_ModInitialize;
        private float m_ProgressLastFrame;

        public static bool Show(ModInitialize modInitialize)
        {
            if (HasBeenInstantiated || !GameModeManager.IsOnTitleScreen())
                return false;

            HasBeenInstantiated = true;
            GameObject spawnedPrefab = Instantiate(OverhaulAssetLoader.GetAsset("OverhaulBootUI", OverhaulAssetPart.Preload));
            ModdedObject moddedObject = spawnedPrefab.GetComponent<ModdedObject>();
            Transform mainViewTransform = moddedObject.GetObject<Transform>(0);
            OverhaulUIManager.ParentTransformToGameUIRoot(mainViewTransform);
            Destroy(spawnedPrefab);

            OverhaulBootUI bootUI = mainViewTransform.gameObject.AddComponent<OverhaulBootUI>();
            bootUI.m_ModInitialize = modInitialize;
            bootUI.m_ProgressLastFrame = 0f;
            bootUI.m_Animator = mainViewTransform.GetComponent<Animator>();
            bootUI.m_Animator.enabled = false;
            bootUI.m_LoadingBar = moddedObject.GetObject<Slider>(1);
            bootUI.m_LoadingBar.value = 0f;
            bootUI.m_Shading = moddedObject.GetObject<Image>(2);
            bootUI.m_Shading.color = Time.timeSinceLevelLoad < 6f ? Color.white : Color.black;

            OverhaulNetworkAssetsController.DownloadTexture(OverhaulMod.Core.ModDirectory + "Assets/Previews/BootUI_" + UnityEngine.Random.Range(1, 5) + ".jpg", moddedObject.GetObject<RawImage>(3));
            return true;
        }

        private void Start()
        {
            setEnvironmentActive(false);
            _ = StartCoroutine(waitThenPlayAnimation());
        }

        private void Update()
        {
            if (IsDisposedOrDestroyed()) 
                return;

            if (ErrorManager.Instance.HasCrashed())
            {
                destroyUI();
                return;
            }

            AudioListener.volume = 0f;
            Time.timeScale = 0f;

            if (OverhaulMod.HasBootProcessEnded)
            {
                m_LoadingBar.value = 1f;
                return;
            }

            float newProgress = OverhaulAssetLoader.GetAllAssetBundlesLoadPercent();
            if (newProgress > m_ProgressLastFrame)
                m_LoadingBar.value = newProgress;
            m_ProgressLastFrame = newProgress;
        }

        private void destroyUI()
        {
            VanillaUIImprovements uIImprovements = OverhaulController.Get<VanillaUIImprovements>();
            if (uIImprovements && uIImprovements.TitleScreenUI && uIImprovements.TitleScreenUI.MessagePanel)
            {
                uIImprovements.TitleScreenUI.MessagePanel.PopulateTitleScreenMessage();
            }

            AudioListener.volume = SettingsManager.Instance.GetSoundVolume();
            Time.timeScale = 1f;
            setEnvironmentActive(true);

            Destroy(gameObject);
        }

        private void setEnvironmentActive(bool value)
        {
            TitleScreenUI titleScreenUI = GameUIRoot.Instance?.TitleScreenUI;
            if (titleScreenUI)
            {
                titleScreenUI.SetLogoAndRootButtonsVisible(value);
                if (value)
                    titleScreenUI.Show();
                else
                    titleScreenUI.Hide();
            }
            Camera arenaTitleScreenLogoCamera = ArenaCameraManager.Instance?.TitleScreenLogoCamera;
            if (arenaTitleScreenLogoCamera)
            {
                arenaTitleScreenLogoCamera.gameObject.SetActive(value);
            }
        }

        private IEnumerator waitThenPlayAnimation()
        {
            yield return new WaitForSecondsRealtime(4f);

            m_Animator.enabled = true;
            yield return new WaitForSecondsRealtime(2f);

            if (!OverhaulMod.HasBootProcessEnded)
            {
                yield return StaticCoroutineRunner.StartStaticCoroutine(m_ModInitialize.LoadAssetsFramework());
                yield return new WaitForSecondsRealtime(1f);
            }
            destroyUI();
            yield break;
        }
    }
}
