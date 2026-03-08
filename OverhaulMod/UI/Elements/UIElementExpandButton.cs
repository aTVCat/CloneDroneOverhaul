using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementExpandButton : OverhaulUIBehaviour
    {
        [UIElement("ExpandImage", true)]
        public GameObject _expandImageObject;

        [UIElement("CollapseImage", false)]
        public GameObject _collapseImageObject;

        private Button _button;

        public RectTransform rectTransform
        {
            get;
            set;
        }

        public Vector2 collapsedSize
        {
            get;
            set;
        }

        public Vector2 expandedSize
        {
            get;
            set;
        }

        private bool _expanded;
        public bool expanded
        {
            get
            {
                return _expanded;
            }
            set
            {
                rectTransform.sizeDelta = value ? expandedSize : collapsedSize;
                _collapseImageObject.SetActive(value);
                _expandImageObject.SetActive(!value);
                _expanded = value;
            }
        }

        protected override void OnInitialized()
        {
            Button button = base.GetComponent<Button>();
            button.onClick.AddListener(ToggleExpand);
            _button = button;
        }

        public void ToggleExpand()
        {
            expanded = !expanded;
            _button.OnDeselect(null);
        }
    }
}
