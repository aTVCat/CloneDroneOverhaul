using ModLibrary;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI
{
    public class VisualEffectsUI : ModGUIBase
    {
        private GameMode _currrentGameMode;
        public const GameMode GAMEMODE_WHERE_IMAGE_EFFECTS_CAANOT_BE_VISIBLE = GameMode.LevelEditor;

        private bool _vignetteEnabled;

        private Image _vignette;
        private Image _noise;

        public static VisualEffectsUI Instance;

        public override void OnInstanceStart()
        {
            Instance = this;
            base.MyModdedObject = base.GetComponent<ModdedObject>();

            GameObject obj1 = GameObject.Instantiate(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "Noise"));
            _noise = obj1.transform.GetChild(1).gameObject.GetComponent<Image>();
            _noise.rectTransform.SetParent(Singleton<GameUIRoot>.Instance.transform);
            _noise.rectTransform.SetAsFirstSibling();
            _noise.rectTransform.sizeDelta = Vector2.zero;
            _noise.rectTransform.localScale = Vector2.one;
            _noise.rectTransform.localPosition = Vector2.zero;

            RectTransform objectFromList = base.MyModdedObject.GetObjectFromList<RectTransform>(0);
            objectFromList.SetParent(Singleton<GameUIRoot>.Instance.transform);
            objectFromList.SetAsFirstSibling();
            objectFromList.sizeDelta = Vector2.zero;
            objectFromList.localScale = Vector2.one;
            objectFromList.localPosition = Vector2.zero;
            this._vignette = objectFromList.GetComponent<Image>();
        }

        public override void OnSettingRefreshed(string ID, object value, bool isRefreshedOnStart = false)
        {
            if (ID == "Graphics.Additions.Vignette")
            {
                _vignetteEnabled = (bool)value;
                RefreshEffects();
            }
            if (ID == "Graphics.Additions.Noise effect")
            {
                OverhaulMain.Visuals.NoiseEnabled = (bool)value;
            }
            if (ID == "Graphics.Additions.Noise Multipler")
            {
                RefreshEffects();
            }
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
                Modules.VisualsModule visuals = OverhaulMain.Visuals;

                this._vignette.gameObject.SetActive(_vignetteEnabled && _currrentGameMode != GAMEMODE_WHERE_IMAGE_EFFECTS_CAANOT_BE_VISIBLE);

                this._noise.gameObject.SetActive(visuals.OverrideSettings ? visuals.Override_NoiseEnabled : visuals.NoiseEnabled && _currrentGameMode != GAMEMODE_WHERE_IMAGE_EFFECTS_CAANOT_BE_VISIBLE);
                this._noise.color = new Color(1, 1, 1, 0.33f * (visuals.OverrideSettings ? visuals.Override_NoiseMultipler : visuals.NoiseMultipler));
            });
        }
    }
}
