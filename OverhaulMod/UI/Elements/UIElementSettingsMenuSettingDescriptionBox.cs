using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementSettingsMenuSettingDescriptionBox : OverhaulUIBehaviour
    {
        [UIElement("DescriptionText")]
        public Text _descriptionText;

        [UIElement("SubDescriptionText")]
        public Text _subDescriptionText;

        [UIElement("SubDescriptionText")]
        public GameObject _subDescriptionTextObject;

        [UIElement("Separator")]
        public GameObject _separatorObject;

        private VerticalLayoutGroup _layoutGroup;

        protected override void OnInitialized()
        {
            _layoutGroup = base.GetComponent<VerticalLayoutGroup>();
        }

        public void SetText(string description, string subDescription)
        {
            if (description.IsNullOrEmpty())
                description = "No description provided.";

            bool subDescriptionIsEmpty = subDescription.IsNullOrEmpty();
            _layoutGroup.padding.bottom = subDescriptionIsEmpty ? 10 : 30;
            _subDescriptionTextObject.SetActive(!subDescriptionIsEmpty);
            //_separatorObject.SetActive(!subDescriptionIsEmpty);

            _descriptionText.text = description;
            _subDescriptionText.text = subDescription;
        }

        public void SetYPosition(float y)
        {
            RectTransform rectTransform = base.transform as RectTransform;
            Vector3 vector = rectTransform.position;
            vector.y = y;
            rectTransform.position = vector;

            Vector2 vector2 = rectTransform.anchoredPosition;
            if (vector2.y <= -180f)
            {
                rectTransform.pivot = new Vector2(0.5f, 0f);
                vector2.y = vector2.y + 20f;
            }
            else
            {
                rectTransform.pivot = new Vector2(0.5f, 1f);
                vector2.y = vector2.y - 20f;
            }
            rectTransform.anchoredPosition = vector2;
        }
    }
}
