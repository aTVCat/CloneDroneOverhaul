using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.Patches.Behaviours
{
    internal class RandomPatchesBehaviour : GamePatchBehaviour
    {
        public override void Patch()
        {
            GameUIRoot gameUIRoot = ModCache.UIRoot;
            if (gameUIRoot)
            {
                // reduce camera resolution
                foreach (Camera cam in Camera.allCameras)
                    if (cam.name == "ArenaCamera")
                    {
                        cam.pixelRect = new Rect(new Vector2(0f, 0f), new Vector2(460f, 240f));
                        break;
                    }

                // reduce trophy shadows
                foreach (ChallengeDefinition challenge in ChallengeManager.Instance.GetChallenges(false))
                    if (challenge.TrophyPrefab)
                    {
                        Light light = challenge.TrophyPrefab.GetComponentInChildren<Light>();
                        //light.shadowResolution = UnityEngine.Rendering.LightShadowResolution.Low;
                        light.shadows = LightShadows.None;
                        light.intensity = 3f;
                    }

                /*
                CrosshairsUI crosshairsUI = gameUIRoot.CrosshairsUI;
                if (crosshairsUI && crosshairsUI.Child)
                {
                    CrosshairOffsetController crosshairOffsetController = crosshairsUI.Child.GetComponent<CrosshairOffsetController>();
                    if (!crosshairOffsetController)
                    {
                        _ = crosshairsUI.Child.AddComponent<CrosshairOffsetController>();
                    }
                }*/

                // fix missing flag image
                LocalizationManager localizationManager = LocalizationManager.Instance;
                if (localizationManager) localizationManager.SupportedLanguages[11].FlagImage = localizationManager.SupportedLanguages[6].FlagImage;

                // remove background from emote selection screen
                GameObject emoteSelectionUIObject = gameUIRoot.EmoteSelectionUI?.gameObject;
                if (emoteSelectionUIObject)
                {
                    Image image = emoteSelectionUIObject.GetComponent<Image>();
                    image.enabled = false;
                }

                // remove background from control mapper
                GameObject controlMapperObject = gameUIRoot.ControlMapper?.gameObject;
                if (controlMapperObject)
                {
                    Transform canvasTransform = TransformUtils.FindChildRecursive(controlMapperObject.transform, "Canvas");
                    if (canvasTransform)
                    {
                        GameObject gameObject = new GameObject("Shading");
                        RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
                        rectTransform.anchorMax = Vector2.one;
                        rectTransform.anchorMin = Vector2.zero;
                        rectTransform.anchoredPosition = Vector2.zero;
                        rectTransform.SetParent(canvasTransform);
                        rectTransform.SetAsFirstSibling();
                        rectTransform.localScale = Vector3.one;
                        Image image = gameObject.AddComponent<Image>();
                        image.color = new Color(0f, 0f, 0f, 0.4f);
                    }
                }

                // refresh game ui after toggling visiblity setting
                gameUIRoot.SettingsMenu.HideGameUIToggle.onValueChanged.AddListener(delegate
                {
                    ModUIManager.Instance.RefreshUIVisibility();
                });

                // make spectator ui not overlap pause menu
                gameUIRoot.CurrentlySpectatingUI.transform.SetSiblingIndex(gameUIRoot.EndlessResultScreen.transform.GetSiblingIndex());

                if (ModFeatures.IsEnabled(ModFeatures.FeatureType.PauseMenuLogoAsRenderTexture))
                {
                    ArenaCameraManager.Instance.TitleScreenLogoCamera.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
                }
            }

            /*
            PhotoManager photoManager = PhotoManager.Instance;
            if (photoManager)
            {
                FlyingCameraController flyingCameraController = photoManager.CameraControllerPrefab;
                if (flyingCameraController)
                {
                    flyingCameraController.FieldOfViewMultiplier = -1000f;
                }
            }*/

            // fix directional light shadow stripes
            DirectionalLightManager directionalLightManager = DirectionalLightManager.Instance;
            if (directionalLightManager)
            {
                Light light = directionalLightManager.DirectionalLight;
                if (light)
                {
                    light.shadowNormalBias = 0f;
                    light.shadowBias = 0.4f;
                }
            }

            // add reverb filter to audio sources
            AudioManager audioManager = AudioManager.Instance;
            if (audioManager)
            {
                PooledPrefab pooledPrefab = audioManager.WorldAudioSourcePool;
                if (pooledPrefab != null)
                {
                    Transform prefab = pooledPrefab.Prefab;
                    if (prefab)
                    {
                        AudioReverbFilter audioReverbFilter = prefab.GetComponent<AudioReverbFilter>();
                        if (!audioReverbFilter)
                        {
                            audioReverbFilter = prefab.gameObject.AddComponent<AudioReverbFilter>();
                            audioReverbFilter.diffusion = 100f;
                            audioReverbFilter.density = 100f;
                            audioReverbFilter.decayTime = 0f;
                        }
                    }
                }
            }
        }
    }
}
