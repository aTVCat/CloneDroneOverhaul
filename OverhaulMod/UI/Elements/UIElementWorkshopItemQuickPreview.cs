using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementWorkshopItemQuickPreview : OverhaulUIBehaviour
    {
        [UIElement("DescriptionText")]
        public Text m_description;

        [UIElement("AuthorText")]
        public Text m_authorText;

        [UIElement("StarsFill")]
        public Image m_starsFill;

        public void Populate(WorkshopItem workshopItem)
        {
            string description = workshopItem.Description;
            if (description.Length > 400)
                description = $"{description.Remove(400)}{"...".AddColor(Color.white)}";

            m_description.text = description;

            if (!workshopItem.Author.IsNullOrEmpty() && workshopItem.Author != "[unknown]")
                m_authorText.text = $"By {workshopItem.Author.AddColor(Color.white)}";
            else
                m_authorText.text = $"By {workshopItem.AuthorID.ToString().AddColor(Color.white)}";

            m_starsFill.fillAmount = Mathf.Ceil(workshopItem.Rating * 5f) / 5f;
        }
    }
}
