using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UICinematicEffects : OverhaulUIBehaviour
    {
        public static UICinematicEffects instance
        {
            get;
            private set;
        }

        [UIElement("Borders", false)]
        private GameObject m_bordersObject;

        [UIElement("UpperBorder")]
        private RectTransform m_upperBorder;

        [UIElement("LowerBorder")]
        private RectTransform m_lowerBorder;

        private PhotoManager m_photoManager;

        public override bool dontRefreshUI => true;

        private bool m_borders;
        public bool borders
        {
            get
            {
                return m_borders;
            }
            set
            {
                m_borders = value;
            }
        }

        private float m_bordersHeight;
        public float bordersHeight
        {
            get
            {
                return m_bordersHeight;
            }
            set
            {
                m_bordersHeight = value;

                Vector2 vector = new Vector2(0f, value);
                m_upperBorder.sizeDelta = vector;
                m_lowerBorder.sizeDelta = vector;
            }
        }

        protected override void OnInitialized()
        {
            m_photoManager = PhotoManager.Instance;
            instance = this;

            borders = false;
            bordersHeight = 100f;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            instance = null;
        }

        public override void Update()
        {
            base.Update();
            m_bordersObject.SetActive(m_photoManager.IsInPhotoMode() && borders);
        }
    }
}
