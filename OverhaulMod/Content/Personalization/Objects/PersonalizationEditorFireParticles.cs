using OverhaulMod.UI;
using UnityEngine;

namespace OverhaulMod.Content.Personalization.Objects
{
    public class PersonalizationEditorFireParticles : PersonalizationEditorComponent
    {
        private ParticleSystem _particleSystem;

        private bool _hasStarted;

        public Color Color
        {
            get => GetPropertyValue(nameof(PersonalizationEditorFireParticles), nameof(Color), Color.white);
            set => SetPropertyValue(nameof(PersonalizationEditorFireParticles), nameof(Color), value);
        }

        public bool EnableSmoke
        {
            get => GetPropertyValue(nameof(PersonalizationEditorFireParticles), nameof(EnableSmoke), true);
            set => SetPropertyValue(nameof(PersonalizationEditorFireParticles), nameof(EnableSmoke), value);
        }

        public bool ApplyFavoriteColor
        {
            get => GetPropertyValue(nameof(PersonalizationEditorFireParticles), nameof(ApplyFavoriteColor), false);
            set => SetPropertyValue(nameof(PersonalizationEditorFireParticles), nameof(ApplyFavoriteColor), value);
        }

        public float FavoriteColorHueOffset
        {
            get => GetPropertyValue(nameof(PersonalizationEditorFireParticles), nameof(FavoriteColorHueOffset), 0.05f);
            set => SetPropertyValue(nameof(PersonalizationEditorFireParticles), nameof(FavoriteColorHueOffset), value);
        }

        public float FavoriteColorBrightness
        {
            get => GetPropertyValue(nameof(PersonalizationEditorFireParticles), nameof(FavoriteColorBrightness), 1f);
            set => SetPropertyValue(nameof(PersonalizationEditorFireParticles), nameof(FavoriteColorBrightness), value);
        }

        public float FavoriteColorSaturation
        {
            get => GetPropertyValue(nameof(PersonalizationEditorFireParticles), nameof(FavoriteColorSaturation), 1f);
            set => SetPropertyValue(nameof(PersonalizationEditorFireParticles), nameof(FavoriteColorSaturation), value);
        }

        private void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            _hasStarted = true;
            RefreshColor();
        }

        private void OnEnable()
        {
            if (!_hasStarted)
                return;

            refreshColor();
        }

        public void RefreshColor()
        {
            if (!base.enabled || !base.gameObject.activeInHierarchy)
                return;

            refreshColor();
        }

        private void refreshColor()
        {
            base.transform.GetChild(0).gameObject.SetActive(EnableSmoke);

            Color colorToApply;
            if (ApplyFavoriteColor)
            {
                Color favoriteColor = PlacedObject.SpawnInfo == null ? UIPE.Instance.Utilities.GetFavoriteColor() : PlacedObject.SpawnInfo.GetFavoriteColor();

                HSBColor hsb = new HSBColor(favoriteColor)
                {
                    s = FavoriteColorSaturation,
                    b = FavoriteColorBrightness
                };
                hsb.h += FavoriteColorHueOffset;
                colorToApply = hsb.ToColor();
                colorToApply.a = 1f;
            }
            else
            {
                colorToApply = Color;
            }

            ParticleSystem.MainModule main = _particleSystem.main;
            main.startColor = colorToApply;
        }
    }
}
