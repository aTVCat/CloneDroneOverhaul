using OverhaulMod.Utils;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIOverhaulInfoWindow : OverhaulUIBehaviour
    {
        [UIElementAction(nameof(Hide))]
        [UIElement("CloseButton")]
        private readonly Button m_exitButton;

        [UIElementAction(nameof(OnModBotPageButtonClicked))]
        [UIElement("ModBotPageButton")]
        private readonly Button m_modBotPageButton;

        [UIElementAction(nameof(OnDiscordServerButtonClicked))]
        [UIElement("DiscordServerButton")]
        private readonly Button m_discordServerButton;

        [UIElementAction(nameof(OnAuthorButtonClicked))]
        [UIElement("authorNameButton")]
        private readonly Button m_authorNameButton;

        [UIElementAction(nameof(OnTester0ButtonClicked))]
        [UIElement("tester0NameButton")]
        private readonly Button m_tester0NameButton;

        [UIElementAction(nameof(OnTester1ButtonClicked))]
        [UIElement("tester1NameButton")]
        private readonly Button m_tester1NameButton;

        [UIElementAction(nameof(OnTester2ButtonClicked))]
        [UIElement("tester2NameButton")]
        private readonly Button m_tester2NameButton;

        [UIElementAction(nameof(OnTester3ButtonClicked))]
        [UIElement("tester3NameButton")]
        private readonly Button m_tester3NameButton;

        [UIElementAction(nameof(OnSpecialPerson0ButtonClicked))]
        [UIElement("person0Button")]
        private readonly Button m_person0Button;

        [UIElementAction(nameof(OnSpecialPerson1ButtonClicked))]
        [UIElement("person1Button")]
        private readonly Button m_person1Button;

        [UIElement("KeyValueDisplay", false)]
        private readonly ModdedObject m_keyValuePrefab;

        [UIElement("Content")]
        private readonly Transform m_content;

        private bool m_hasPopulatedInfo;
        private int m_keyValueCount;

        public override void Show()
        {
            base.Show();

            if (!m_hasPopulatedInfo)
            {
                AddKeyValueDisplay("Mod version", ModBuildInfo.version.ToString());
                if (!ModBuildInfo.extraInfoError)
                {
                    AddKeyValueDisplay("Compilation date", ModBuildInfo.extraInfo.CompileTime.ToShortDateString());
                }
                AddKeyValueDisplay("Milestone", ModBuildInfo.milestoneNaming);
                AddKeyValueDisplay("Is Mod-Bot release?", ModBuildInfo.modBotRelease.ToString());
                AddKeyValueDisplay("Is GitHub release?", ModBuildInfo.gitHubRelease.ToString());
                AddKeyValueDisplay("Is internal release?", ModBuildInfo.internalRelease.ToString());
                AddKeyValueDisplay("Debug mode?", ModBuildInfo.debug.ToString());
                m_hasPopulatedInfo = true;
            }
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void AddKeyValueDisplay(string key, string value)
        {
            ModdedObject moddedObject = Instantiate(m_keyValuePrefab, m_content);
            moddedObject.gameObject.SetActive(true);
            moddedObject.GetObject<Text>(0).text = key + ":";
            moddedObject.GetObject<Text>(1).text = value;
            moddedObject.GetObject<GameObject>(2).SetActive(m_keyValueCount % 2 == 0);
            m_keyValueCount++;
        }

        public void OnAuthorButtonClicked()
        {
            if (SteamManager.Instance && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
            {
                SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/profiles/76561199028311109");
            }
            else
            {
                Application.OpenURL("https://steamcommunity.com/profiles/76561199028311109");
            }
        }

        public void OnTester0ButtonClicked()
        {
            if (SteamManager.Instance && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
            {
                SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/id/sonicgleb");
            }
            else
            {
                Application.OpenURL("https://steamcommunity.com/id/sonicgleb");
            }
        }

        public void OnTester1ButtonClicked()
        {
            if (SteamManager.Instance && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
            {
                SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/id/Lexium-Rosewarne");
            }
            else
            {
                Application.OpenURL("https://steamcommunity.com/id/Lexium-Rosewarne");
            }
        }

        public void OnTester2ButtonClicked()
        {
            if (SteamManager.Instance && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
            {
                SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/profiles/76561198258900316");
            }
            else
            {
                Application.OpenURL("https://steamcommunity.com/profiles/76561198258900316");
            }
        }

        public void OnTester3ButtonClicked()
        {
            if (SteamManager.Instance && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
            {
                SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/id/EggRolly");
            }
            else
            {
                Application.OpenURL("https://steamcommunity.com/id/EggRolly");
            }
        }

        public void OnSpecialPerson0ButtonClicked()
        {
            if (SteamManager.Instance && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
            {
                SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/id/sonicgleb");
            }
            else
            {
                Application.OpenURL("https://steamcommunity.com/id/sonicgleb");
            }
        }

        public void OnSpecialPerson1ButtonClicked()
        {
            if (SteamManager.Instance && SteamManager.Instance.Initialized && SteamUtils.IsOverlayEnabled())
            {
                SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/profiles/76561199014733748");
            }
            else
            {
                Application.OpenURL("https://steamcommunity.com/profiles/76561199014733748");
            }
        }

        public void OnModBotPageButtonClicked()
        {
            Application.OpenURL("https://modbot.org/modPreview.html?modID=rAnDomPaTcHeS1");
        }

        public void OnDiscordServerButtonClicked()
        {
            Application.OpenURL("https://discord.gg/RXN7uDUfwx");
        }
    }
}
