using OverhaulMod.UI;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectFireParticles : PersonalizationEditorObjectComponentBase
    {
        private ParticleSystem _particleSystem;

        private bool _hasStarted;

        public Color color
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue(nameof(color), Color.white);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(color), value);
            }
        }

        public bool enableSmoke
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue(nameof(enableSmoke), true);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(enableSmoke), value);
            }
        }

        public bool applyFavoriteColor
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue(nameof(applyFavoriteColor), false);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(applyFavoriteColor), value);
            }
        }

        public float favoriteColorHueOffset
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue(nameof(favoriteColorHueOffset), 0.05f);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(favoriteColorHueOffset), value);
            }
        }

        public float favoriteColorBrightness
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue(nameof(favoriteColorBrightness), 1f);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(favoriteColorBrightness), value);
            }
        }

        public float favoriteColorSaturation
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue(nameof(favoriteColorSaturation), 1f);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(favoriteColorSaturation), value);
            }
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
            base.transform.GetChild(0).gameObject.SetActive(enableSmoke);

            Color colorToApply;
            if (applyFavoriteColor)
            {
                Color favoriteColor = objectBehaviour.ControllerInfo == null ? UIPersonalizationEditor.instance.Utilities.GetFavoriteColor() : objectBehaviour.ControllerInfo.GetFavoriteColor();

                HSBColor hsb = new HSBColor(favoriteColor)
                {
                    s = favoriteColorSaturation,
                    b = favoriteColorBrightness
                };
                hsb.h += favoriteColorHueOffset;
                colorToApply = hsb.ToColor();
                colorToApply.a = 1f;
            }
            else
            {
                colorToApply = color;
            }

            ParticleSystem.MainModule main = _particleSystem.main;
            main.startColor = colorToApply;
        }
    }
}
