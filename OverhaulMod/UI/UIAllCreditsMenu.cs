using OverhaulMod.Utils;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIAllCreditsMenu : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElement("ScrollRect")]
        private readonly ScrollRect m_scrollRect;

        [UIElement("SectionPrefab", false)]
        private readonly ModdedObject m_sectionPrefab;
        [UIElement("UserDisplayPrefab", false)]
        private readonly ModdedObject m_userDisplayPrefab;

        [UIElement("Content")]
        private readonly Transform m_content;

        protected override void OnInitialized()
        {
            AddSection(LocalizationManager.Instance.GetTranslatedString("credits_header_author"), "FFFFFF");
            AddUser("A TVCat", "hi", "https://steamcommunity.com/profiles/76561199028311109");

            AddSection(LocalizationManager.Instance.GetTranslatedString("credits_header_playtested_by"), "FFECD9");
            AddUser("SonicGleb", LocalizationManager.Instance.GetTranslatedString("credits_tooltip_playtested_by_sonicgleb"), "https://steamcommunity.com/id/sonicgleb");
            AddUser("Electrified_CyberKick", "", "https://steamcommunity.com/id/Lexium-Rosewarne");
            AddUser("ッizanami", "", "https://steamcommunity.com/profiles/76561198258900316");
            AddUser("bow1__", "", "https://steamcommunity.com/id/EggRolly");

            AddSection(LocalizationManager.Instance.GetTranslatedString("credits_header_special_thanks"), "D9E0FF");
            AddUser("SonicGleb", LocalizationManager.Instance.GetTranslatedString("credits_tooltip_special_thanks_sonicgleb"), "https://steamcommunity.com/id/sonicgleb");
            AddUser("Igrok_x_xp", LocalizationManager.Instance.GetTranslatedString("credits_tooltip_special_thanks_igrok_x_xp"), "https://steamcommunity.com/profiles/76561199014733748");
            AddUser("Pharawill-MK2 (Water)", LocalizationManager.Instance.GetTranslatedString("credits_tooltip_special_thanks_water"), "https://steamcommunity.com/profiles/76561198995153570");
        }

        public void AddUser(string username, string description, string steamLink, bool isLibrary = false)
        {
            bool descriptionIsEmpty = description.IsNullOrEmpty();

            ModdedObject moddedObject = Instantiate(m_userDisplayPrefab, m_content);
            moddedObject.GetObject<Text>(0).text = username;
            moddedObject.GetObject<GameObject>(0).SetActive(!descriptionIsEmpty);
            moddedObject.GetObject<Text>(4).text = username;
            moddedObject.GetObject<GameObject>(4).SetActive(descriptionIsEmpty);
            moddedObject.GetObject<Text>(1).text = description;
            moddedObject.GetObject<GameObject>(1).SetActive(!descriptionIsEmpty);
            moddedObject.GetObject<GameObject>(2).gameObject.SetActive(!steamLink.IsNullOrEmpty());
            moddedObject.GetObject<Button>(2).onClick.AddListener(delegate
            {
                if (SteamManager.Instance && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
                    SteamFriends.ActivateGameOverlayToWebPage(steamLink);
                else
                    Application.OpenURL(steamLink);
            });
            moddedObject.GetObject<GameObject>(2).SetActive(false);
            moddedObject.gameObject.SetActive(true);
        }

        public void AddSection(string name, string hexColor)
        {
            Color color1 = ModParseUtils.TryParseToColor(hexColor, Color.white);
            Color color2 = color1;
            color2.a = 0.25f;

            ModdedObject moddedObject = Instantiate(m_sectionPrefab, m_content);
            moddedObject.GetObject<Text>(0).text = name;
            moddedObject.GetObject<Text>(0).color = color1;
            moddedObject.GetObject<Image>(1).color = color2;
            moddedObject.gameObject.SetActive(true);
        }
    }
}