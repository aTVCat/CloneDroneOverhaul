using OverhaulMod.Content;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementWorkshopItemQuickPreview : OverhaulUIBehaviour
    {
        [UIElement("DescriptionText")]
        public Text _description;

        [UIElement("AuthorText")]
        public Text _authorText;

        [UIElement("StarsFill")]
        public Image _starsFill;

        [UIElement("Stars")]
        public GameObject _starsObject;

        [UIElement("NotEnoughRatingsText")]
        public GameObject _notEnoughRatingsTextObject;

        public void Populate(WorkshopItem workshopItem)
        {
            string description = workshopItem.Description;
            if (description.Length > 400)
                description = $"{description.Remove(400)}{"...".AddColor(Color.white)}";

            int votes = workshopItem.Votes;
            _notEnoughRatingsTextObject.SetActive(votes < 25);
            _starsObject.SetActive(!_notEnoughRatingsTextObject.activeSelf);

            _description.text = description;

            if (!workshopItem.Author.IsNullOrEmpty() && workshopItem.Author != "[unknown]")
                _authorText.text = $"{LocalizationManager.Instance.GetTranslatedString("workshop_leveldetails_author")} {workshopItem.Author.AddColor(Color.white)}";
            else
                _authorText.text = $"{LocalizationManager.Instance.GetTranslatedString("workshop_leveldetails_author")} {workshopItem.AuthorID.ToString().AddColor(Color.white)}";

            _starsFill.fillAmount = Mathf.Ceil(workshopItem.Rating * 5f) / 5f;
        }
    }
}
