using UnityEngine;
using UnityEngine.Events;
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

        public RectTransform RectTransformReference;

        public Vector2 CollapsedSize;

        public Vector2 ExpandedSize;

        private bool _expanded;
        public bool IsExpanded
        {
            get
            {
                return _expanded;
            }
            set
            {
                bool invokeCallback = _expanded != value;

                RectTransformReference.sizeDelta = value ? ExpandedSize : CollapsedSize;
                _collapseImageObject.SetActive(value);
                _expandImageObject.SetActive(!value);
                _expanded = value;

                if (invokeCallback && Callback != null) Callback(value);
            }
        }

        public UnityAction<bool> Callback;

        protected override void OnInitialized()
        {
            Button button = base.GetComponent<Button>();
            button.onClick.AddListener(ToggleExpand);
            _button = button;
        }

        public void ToggleExpand()
        {
            IsExpanded = !IsExpanded;
            _button.OnDeselect(null);
        }
    }
}
