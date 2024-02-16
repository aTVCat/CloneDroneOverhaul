using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementExpandButton : OverhaulUIBehaviour
    {
        [UIElement("ExpandImage", true)]
        public GameObject m_expandImageObject;

        [UIElement("CollapseImage", false)]
        public GameObject m_collapseImageObject;

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

        private bool m_expanded;
        public bool expanded
        {
            get
            {
                return m_expanded;
            }
            set
            {
                rectTransform.sizeDelta = value ? expandedSize : collapsedSize;
                m_collapseImageObject.SetActive(value);
                m_expandImageObject.SetActive(!value);
                m_expanded = value;
            }
        }

        protected override void OnInitialized()
        {
            Button button = base.GetComponent<Button>();
            button.onClick.AddListener(ToggleExpand);
        }

        public void ToggleExpand()
        {
            expanded = !expanded;
        }
    }
}
