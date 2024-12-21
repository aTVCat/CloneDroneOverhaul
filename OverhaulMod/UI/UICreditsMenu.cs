using OverhaulMod.Utils;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;
using static Mono.Security.X509.X520;

namespace OverhaulMod.UI
{
    public class UICreditsMenu : OverhaulUIBehaviour
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

            AddSection("Libraries used in the mod:", "6FE5FF");
            AddLibrary("Amplify Occlusion", "The post-effect that adds cool shadows.\nMade by Amplify Creations", "https://assetstore.unity.com/packages/vfx/shaders/fullscreen-camera-effects/amplify-occlusion-56739", false);
            AddLibrary("SEGI", "The Global Illumination post effect.\nMade by Stanislav Denisov", "https://github.com/sonicether/SEGI", false);
            AddLibrary("Custom robot model editor", "Some of its code is used in this mod to import .cvm models.\nMade by X606", "https://modbot.org/modPreview.html?modID=de731a6b-0a96-4882-a02b-a336904f9853", false);
            AddLibrary("Mesh Serializer", "The utility used to save meshes on the disk and read them.\nMade by BUNNY83", "https://pastebin.com/yW91qEQh", false);
            AddLibrary("Discord RPC", "The library used to display custom status in discord.\nMade by Discord", "https://discord.com/developers/docs/topics/rpc", false);
        }

        public void AddLibrary(string libraryName, string description, string hyperlink, bool openInSteamOverlay)
        {
            bool descriptionIsEmpty = description.IsNullOrEmpty();

            ModdedObject moddedObject = Instantiate(m_userDisplayPrefab, m_content);
            moddedObject.GetObject<Text>(0).text = libraryName;
            moddedObject.GetObject<GameObject>(0).SetActive(!descriptionIsEmpty);
            moddedObject.GetObject<Text>(4).text = libraryName;
            moddedObject.GetObject<GameObject>(4).SetActive(descriptionIsEmpty);
            moddedObject.GetObject<Text>(1).text = description;
            moddedObject.GetObject<GameObject>(1).SetActive(!descriptionIsEmpty);
            moddedObject.GetObject<GameObject>(2).gameObject.SetActive(!hyperlink.IsNullOrEmpty());
            moddedObject.GetObject<Button>(2).onClick.AddListener(delegate
            {
                if (openInSteamOverlay && SteamManager.Instance && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
                    SteamFriends.ActivateGameOverlayToWebPage(hyperlink);
                else
                    Application.OpenURL(hyperlink);
            });
            moddedObject.gameObject.SetActive(true);
        }

        public void AddUser(string username, string description, string steamLink)
        {
            bool descriptionIsEmpty = description.IsNullOrEmpty();

            ModdedObject moddedObject = Instantiate(m_userDisplayPrefab, m_content);
            moddedObject.GetObject<Text>(0).text = username;
            moddedObject.GetObject<GameObject>(0).SetActive(!descriptionIsEmpty);
            moddedObject.GetObject<Text>(4).text = username;
            moddedObject.GetObject<GameObject>(4).SetActive(descriptionIsEmpty);
            moddedObject.GetObject<Text>(1).text = description;
            moddedObject.GetObject<GameObject>(1).SetActive(!descriptionIsEmpty);
            moddedObject.GetObject<GameObject>(2).gameObject.SetActive(false);
            moddedObject.GetObject<Button>(2).onClick.AddListener(delegate
            {
                if (SteamManager.Instance && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
                    SteamFriends.ActivateGameOverlayToWebPage(steamLink);
                else
                    Application.OpenURL(steamLink);
            });
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