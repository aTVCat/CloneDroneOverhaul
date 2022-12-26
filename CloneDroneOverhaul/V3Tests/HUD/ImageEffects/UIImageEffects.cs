using CloneDroneOverhaul.V3Tests.Gameplay;
using ModLibrary;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.V3Tests.HUD
{
    public class UIImageEffects : V3_ModHUDBase
    {
        public const GameMode GAMEMODE_WHERE_IMAGE_EFFECTS_CAANOT_BE_VISIBLE = GameMode.LevelEditor;

        private Image _vignette;
        private (bool, bool) _vignetteEnabled;

        public const float _ditheringTextureColorAValue = 0.3f;
        private (bool, bool) _ditheringEnabled;
        private (float, float) _ditheringIntensity;
        private (DitheringResolution, DitheringResolution) _ditheringResolution;
        private DitheringRefreshRate _ditheringRefreshRate;

        private static bool _shouldGenerateDitheringTextures = true;
        private static Dictionary<DitheringResolution, Texture2D[]> _ditheringGeneratedTextures = new Dictionary<DitheringResolution, Texture2D[]>()
        {
            {DitheringResolution.Res_320x180, new Texture2D[10] },
            {DitheringResolution.Res_640x360, new Texture2D[10] },
            {DitheringResolution.Res_1280x720, new Texture2D[10] },
            {DitheringResolution.Res_1920x1080, new Texture2D[10] },
            {DitheringResolution.Res_2560x1440, new Texture2D[10] },
        };
        private int _ditheringTextureIndex = 0;
        private bool _isReadyToDither;
        private int _framesLeftToSwitchDitheringTexture = 2;
        private RawImage _ditheringTest;

        #region Utilities

        /// <summary>
        /// Generate perlin noise texture
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Texture2D GeneratePerlinNoise(in int width, in int height)
        {
            Texture2D tex = new Texture2D(width, height);
            Color[] pixels = new Color[width * height];

            float xOrg = 0;
            float yOrg = 0;
            float y = 0.0F;

            while (y < tex.height)
            {
                float x = 0.0F;
                while (x < tex.width)
                {

                    float xCoord = xOrg + x / tex.width;
                    float yCoord = yOrg + y / tex.height;
                    float sample = Mathf.PerlinNoise(xCoord, yCoord) + Random.Range(-0.200f, 0.400f);
                    pixels[(int)y * tex.width + (int)x] = new Color(sample, sample, sample, 0.1f);
                    x++;
                }
                y++;
            }

            for (int i = 0; i < pixels.Length; i++)
            {
                float s = Random.Range(-0.200f, 0.400f);
                pixels[i] = new Color(pixels[i].r + s, pixels[i].g + s, pixels[i].b + s, 0.1f);
            }

            tex.SetPixels(pixels);
            tex.Apply();

            return tex;
        }

        public static (int, int) GetResolutionByIndex(in int index)
        {
            switch (index)
            {
                case 0:
                    return (320, 180);
                case 1:
                    return (640, 360);
                case 2:
                    return (1280, 720);
                case 3:
                    return (1920, 1080);
                case 4:
                    return (2560, 1440);
            }
            return (320, 180);
        }

        private int getFramesToWait()
        {
            switch (_ditheringRefreshRate)
            {
                case DitheringRefreshRate.EveryFrame:
                    return 1;
                case DitheringRefreshRate.EverySecondFrame:
                    return 2;
                case DitheringRefreshRate.EveryThirdFrame:
                    return 3;
            }
            return 3;
        }

        #endregion

        private void generatePerlinNoiseTextures()
        {
            for (int j = 0; j < 5; j++)
            {
                (int, int) res = GetResolutionByIndex(j);
                for (int i = 0; i < 10; i++)
                {
                    _ditheringGeneratedTextures[(DitheringResolution)j][i] = GeneratePerlinNoise(res.Item1, res.Item2);
                }
            }
        }

        public static void RefreshImageEffects()
        {
            GetInstance<UIImageEffects>().RefreshEffects();
        }

        void Start()
        {
            if (_shouldGenerateDitheringTextures)
            {
                generatePerlinNoiseTextures();
                _shouldGenerateDitheringTextures = false;
            }

            RectTransform objectFromList2 = base.ModdedObject.GetObjectFromList<RectTransform>(1);
            objectFromList2.SetParent(Singleton<GameUIRoot>.Instance.transform);
            objectFromList2.SetAsFirstSibling();
            objectFromList2.sizeDelta = Vector2.zero;
            objectFromList2.localScale = Vector3.one;
            objectFromList2.localPosition = Vector2.zero;
            _ditheringTest = objectFromList2.GetComponent<RawImage>();
            _ditheringTest.color = new Color(1, 1, 1, 0.3f);
            _isReadyToDither = true;

            RectTransform objectFromList = base.ModdedObject.GetObjectFromList<RectTransform>(0);
            objectFromList.SetParent(Singleton<GameUIRoot>.Instance.transform);
            objectFromList.SetAsFirstSibling();
            objectFromList.sizeDelta = Vector2.zero;
            objectFromList.localScale = Vector3.one;
            objectFromList.localPosition = Vector2.zero;
            _vignette = objectFromList.GetComponent<Image>();

            RefreshEffects();
        }

        public override void OnSettingRefreshed(in string settingName, in object value)
        {
            if (settingName == "Graphics.Additions.Vignette")
            {
                _vignetteEnabled.Item2 = (bool)value;
            }
            if (settingName == "Graphics.Additions.Noise effect")
            {
                _ditheringEnabled.Item2 = (bool)value;
            }
            if (settingName == "Graphics.Additions.Noise Multipler")
            {
                _ditheringIntensity.Item2 = (float)value;
            }
            if (settingName == "Graphics.Additions.DitherRes")
            {
                _ditheringResolution.Item2 = (DitheringResolution)(int)value;
            }
            if (settingName == "Graphics.Additions.DitherRRate")
            {
                _ditheringRefreshRate = (DitheringRefreshRate)(int)value;
            }
            RefreshEffects();
        }

        public void RefreshEffects()
        {
            OverhaulMain.Timer.AddNoArgActionToCompleteNextFrame(delegate
            {
                GameMode _currrentGameMode = GameFlowManager.Instance.GetCurrentGameMode();
                _vignette.gameObject.SetActive((OverhaulGraphicsController.OverrideSettings ? _vignetteEnabled.Item1 : _vignetteEnabled.Item2) && _currrentGameMode != GAMEMODE_WHERE_IMAGE_EFFECTS_CAANOT_BE_VISIBLE);
                _ditheringTest.gameObject.SetActive(OverhaulGraphicsController.OverrideSettings ? _ditheringEnabled.Item1 : _ditheringEnabled.Item2 && _currrentGameMode != GAMEMODE_WHERE_IMAGE_EFFECTS_CAANOT_BE_VISIBLE && OverhaulGraphicsController.CachedMainCamera != null);
                _ditheringTest.color = new Color(1, 1, 1, _ditheringTextureColorAValue * (OverhaulGraphicsController.OverrideSettings ? _ditheringIntensity.Item1 : _ditheringIntensity.Item2));
            });
        }

        private void Update()
        {
            if(!(OverhaulGraphicsController.OverrideSettings ? _ditheringEnabled.Item1 : _ditheringEnabled.Item2) || !_isReadyToDither || _shouldGenerateDitheringTextures)
            {
                return;
            }

            _framesLeftToSwitchDitheringTexture--;
            if(_framesLeftToSwitchDitheringTexture <= 0)
            {
                _framesLeftToSwitchDitheringTexture = getFramesToWait();

                _ditheringTextureIndex++;
                if(_ditheringTextureIndex >= 10)
                {
                    _ditheringTextureIndex = 0;
                }

                _ditheringTest.texture = _ditheringGeneratedTextures[OverhaulGraphicsController.OverrideSettings ? _ditheringResolution.Item1 : _ditheringResolution.Item2][_ditheringTextureIndex];
            }
        }
    }
}
