using CloneDroneOverhaul.V3Tests.Gameplay;
using ModLibrary;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI
{
    public class VisualEffectsUI : ModGUIBase
    {
        private GameMode _currrentGameMode;
        public const GameMode GAMEMODE_WHERE_IMAGE_EFFECTS_CAANOT_BE_VISIBLE = GameMode.LevelEditor;

        private (bool, bool) _vignetteEnabled;
        private (bool, bool) _noiseEnabled;

        private (float, float) _noiseIntensity;

        private Image _vignette;
        private Image _noise;

        public static VisualEffectsUI Instance;

        public override void OnInstanceStart()
        {
            Instance = this;
            base.MyModdedObject = base.GetComponent<ModdedObject>();

            GameObject obj1 = GameObject.Instantiate(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "Noise"));
            _noise = obj1.transform.GetChild(0).gameObject.GetComponent<Image>();
            _noise.rectTransform.SetParent(Singleton<GameUIRoot>.Instance.transform);
            _noise.rectTransform.SetAsFirstSibling();
            _noise.rectTransform.sizeDelta = Vector2.zero;
            _noise.rectTransform.localScale = Vector3.one;
            _noise.rectTransform.localPosition = Vector2.zero;
            Destroy(obj1);

            RectTransform objectFromList = base.MyModdedObject.GetObjectFromList<RectTransform>(0);
            objectFromList.SetParent(Singleton<GameUIRoot>.Instance.transform);
            objectFromList.SetAsFirstSibling();
            objectFromList.sizeDelta = Vector2.zero;
            objectFromList.localScale = Vector3.one;
            objectFromList.localPosition = Vector2.zero;
            _vignette = objectFromList.GetComponent<Image>();
        }

        public override void OnSettingRefreshed(string ID, object value, bool isRefreshedOnStart = false)
        {
            if (ID == "Graphics.Additions.Vignette")
            {
                _vignetteEnabled.Item2 = (bool)value;
            }
            if (ID == "Graphics.Additions.Noise effect")
            {
                _noiseEnabled.Item2 = (bool)value;
            }
            if (ID == "Graphics.Additions.Noise Multipler")
            {
                _noiseIntensity.Item2 = (float)value;
            }
            RefreshEffects();
        }

        public override void RunFunction<T>(string name, T obj)
        {
            if (name == "onGameModeUpdated")
            {
                GameMode gm = (GameMode)(int)(object)obj;
                _currrentGameMode = gm;

                RefreshEffects();
            }
        }

        public void RefreshEffects()
        {
            OverhaulMain.Timer.AddNoArgActionToCompleteNextFrame(delegate
            {
                _vignette.gameObject.SetActive((OverhaulGraphicsController.OverrideSettings ? _vignetteEnabled.Item1 : _vignetteEnabled.Item2) && _currrentGameMode != GAMEMODE_WHERE_IMAGE_EFFECTS_CAANOT_BE_VISIBLE);
                _noise.gameObject.SetActive((OverhaulGraphicsController.OverrideSettings ? _noiseEnabled.Item1 : _noiseEnabled.Item2) && _currrentGameMode != GAMEMODE_WHERE_IMAGE_EFFECTS_CAANOT_BE_VISIBLE);

                _noise.color = new Color(1, 1, 1, 0.33f * (OverhaulGraphicsController.OverrideSettings ? _noiseIntensity.Item1 : _noiseIntensity.Item2));
            });
        }
    }
}
