using OverhaulMod.Engine;
using UnityEngine;

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
        private readonly GameObject _bordersObject;

        [UIElement("UpperBorder")]
        private readonly RectTransform _upperBorder;

        [UIElement("LowerBorder")]
        private readonly RectTransform _lowerBorder;

        private PhotoManager _photoManager;

        public override bool refreshOnlyCursor => true;
        public override bool closeOnEscapeButtonPress => false;

        public bool borders { get; set; }

        private float _bordersHeight;
        public float bordersHeight
        {
            get
            {
                return _bordersHeight;
            }
            set
            {
                _bordersHeight = value;

                Vector2 vector = new Vector2(0f, value);
                _upperBorder.sizeDelta = vector;
                _lowerBorder.sizeDelta = vector;
            }
        }

        protected override void OnInitialized()
        {
            _photoManager = PhotoManager.Instance;
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
            _bordersObject.SetActive(AdvancedPhotoModeManager.Instance.IsActive() && borders);
        }
    }
}
