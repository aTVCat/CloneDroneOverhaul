using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementSettingsMenuCategoryTab : UIElementTab, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        public const float EXTRA_HEIGHT = 7f;

        [UIElement("Text")]
        private readonly Text _text;

        [UIElement("SubcategoriesContainer")]
        private readonly RectTransform _subcategoriesContainer;

        [UIElement("SubcategoriesContainer", false)]
        private readonly GameObject _subcategoriesContainerObject;

        [UIElement("SubcategoryDisplay", false)]
        private readonly Text _subcategoryDisplay;

        public string LocalizationID;

        private bool _mouseIn;

        private float _expandProgress, _height;

        protected override void OnInitialized()
        {
            _height = 0f;
            _text.text = LocalizationID.IsNullOrEmpty() ? tabId : LocalizationManager.Instance.GetTranslatedString(LocalizationID);

            switch (tabId)
            {
                case "Gameplay":
                    InitializeSubcategoryDisplay("Difficulty", false);
                    InitializeSubcategoryDisplay("Endless levels", false);
                    InitializeSubcategoryDisplay("Twitch", false);
                    InitializeSubcategoryDisplay("Player", false);
                    InitializeSubcategoryDisplay("Camera", false);
                    break;
                case "Interface":
                    InitializeSubcategoryDisplay("Game interface", false);
                    InitializeSubcategoryDisplay("Energy bar enhancements", false);
                    InitializeSubcategoryDisplay("Photo mode", false);
                    InitializeSubcategoryDisplay("Labels", false);
                    InitializeSubcategoryDisplay("Transitions", false);
                    break;
                case "Graphics":
                    InitializeSubcategoryDisplay("Window", false);
                    InitializeSubcategoryDisplay("Render", false);
                    InitializeSubcategoryDisplay("Post effects", false);
                    InitializeSubcategoryDisplay("Color blindness mode", false);
                    break;
                case "Effects":
                    InitializeSubcategoryDisplay("Particles", false);
                    InitializeSubcategoryDisplay("Voxel engine", false);
                    InitializeSubcategoryDisplay("Robots", false);
                    InitializeSubcategoryDisplay("Environment", false);
                    InitializeSubcategoryDisplay("Garbage", false);
                    break;
                case "Sounds":
                    InitializeSubcategoryDisplay("Volume", false);
                    InitializeSubcategoryDisplay("Filters", false);
                    InitializeSubcategoryDisplay("Misc.", false);
                    break;
                case "Multiplayer":
                    InitializeSubcategoryDisplay("Multiplayer settings", false);
                    InitializeSubcategoryDisplay("Player", false);
                    break;
                case "Advanced":
                    InitializeSubcategoryDisplay("Mod-bot", false);
                    InitializeSubcategoryDisplay("Rich presence", false);
                    InitializeSubcategoryDisplay("Reset settings", false);
                    break;
            }
        }

        public override void OnDisable()
        {
            _mouseIn = false;
            _expandProgress = 0f;
        }

        public override void Update()
        {
            RectTransform rt = _subcategoriesContainer;
            Vector2 sd = rt.sizeDelta;
            sd.y = Mathf.Lerp(0f, _height + EXTRA_HEIGHT, NumberUtils.EaseOutQuad(0f, 1f, _expandProgress));
            rt.sizeDelta = sd;

            _subcategoriesContainerObject.SetActive(_expandProgress > 0f);

            _expandProgress = Mathf.Clamp01(_expandProgress + ((_mouseIn ? 1f : -1f) * Time.unscaledDeltaTime * 7.5f));
        }

        public void InitializeSubcategoryDisplay(string text, bool subHeader)
        {
            _height += 20f + EXTRA_HEIGHT;
            Text text1 = Instantiate(_subcategoryDisplay, _subcategoriesContainer);
            text1.gameObject.SetActive(true);
            text1.text = LocalizationManager.Instance.GetTranslatedString($"settings_{(subHeader ? "subheader" : "header")}_{text.ToLower().Replace(' ', '_')}");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _mouseIn = _height != 0f;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _mouseIn = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _mouseIn = false;
        }
    }
}
