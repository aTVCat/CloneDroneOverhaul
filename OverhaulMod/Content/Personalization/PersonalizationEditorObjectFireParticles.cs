using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectFireParticles : PersonalizationEditorObjectComponentBase
    {
        private ParticleSystem m_particleSystem;

        private bool m_hasStarted;

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
            m_particleSystem = GetComponent<ParticleSystem>();
            m_hasStarted = true;
            RefreshColor();
        }

        private void OnEnable()
        {
            if (!m_hasStarted)
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
                HSBColor hsb = new HSBColor(objectBehaviour.ControllerInfo.Reference.owner.GetCharacterModel().GetFavouriteColor())
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

            ParticleSystem.MainModule main = m_particleSystem.main;
            main.startColor = colorToApply;
        }
    }
}
