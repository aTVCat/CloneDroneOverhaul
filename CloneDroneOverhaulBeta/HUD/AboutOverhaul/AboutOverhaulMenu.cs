using Steamworks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class AboutOverhaulMenu : OverhaulUIVer2
    {
        public override void Initialize()
        {
            base.Initialize();
            AssignActionToButton(MyModdedObject, "BackButton", OnBackButtonClicked);
            AssignActionToButton(MyModdedObject, "DiscordServerButton", OnDiscordServerButtonClicked);
            AssignActionToButton(MyModdedObject, "SteamButton", OnAuthorSteamButtonClicked);
            AssignActionToButton(MyModdedObject, "ModBotButton", OnModBotButtonClicked);
            AssignActionToButton(MyModdedObject, "GitHubButton", OnGitHubButtonClicked);
        }

        public override void Show()
        {
            base.Show();
            HideTitleScreenButtons();
        }

        public override void Hide()
        {
            base.Hide();
            ShowTitleScreenButtons();
        }

        public void OnBackButtonClicked()
        {
            Hide();
        }

        public void OnDiscordServerButtonClicked()
        {
            Application.OpenURL("https://discord.gg/qUkRhaqZZZ");
        }

        public void OnGitHubButtonClicked()
        {
            Application.OpenURL("https://github.com/aTVCat/CloneDroneOverhaul");
        }

        public void OnModBotButtonClicked()
        {
            Application.OpenURL("https://modbot.org/modPreview.html?modID=rAnDomPaTcHeS1");
        }

        public void OnAuthorSteamButtonClicked()
        {
            if (SteamUtils.IsOverlayEnabled())
            {
                SteamFriends.ActivateGameOverlayToWebPage("https://steamcommunity.com/profiles/76561199028311109/");
                return;
            }
            Application.OpenURL("https://steamcommunity.com/profiles/76561199028311109/");
        }
    }
}
