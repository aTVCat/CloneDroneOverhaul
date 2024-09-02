using OverhaulMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI.Elements
{
    public class UIElementSettingsMenuSettingDescriptionBox : OverhaulUIBehaviour
    {
        [UIElement("DescriptionText")]
        public Text m_descriptionText;

        [UIElement("SubDescriptionText")]
        public Text m_subDescriptionText;

        [UIElement("SubDescriptionText")]
        public GameObject m_subDescriptionTextObject;

        [UIElement("Separator")]
        public GameObject m_separatorObject;

        private VerticalLayoutGroup m_layoutGroup;

        protected override void OnInitialized()
        {
            m_layoutGroup = base.GetComponent<VerticalLayoutGroup>();
        }

        public void SetText(string description, string subDescription)
        {
            if (description.IsNullOrEmpty())
                description = "No description provided.";

            bool subDescriptionIsEmpty = subDescription.IsNullOrEmpty();
            m_layoutGroup.padding.bottom = subDescriptionIsEmpty ? 10 : 30;
            m_subDescriptionTextObject.SetActive(!subDescriptionIsEmpty);
            //m_separatorObject.SetActive(!subDescriptionIsEmpty);

            m_descriptionText.text = description;
            m_subDescriptionText.text = subDescription;
        }

        public void SetYPosition(float y)
        {
            RectTransform rectTransform = base.transform as RectTransform;
            Vector3 vector = rectTransform.position;
            vector.y = y;
            rectTransform.position = vector;

            Vector2 vector2 = rectTransform.anchoredPosition;
            if(vector2.y <= -180f)
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
