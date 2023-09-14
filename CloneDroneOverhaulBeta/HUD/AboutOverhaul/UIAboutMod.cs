using Steamworks;
using UnityEngine;

namespace CDOverhaul.HUD
{
    public class UIAboutMod : UIController
    {
        protected override bool HideTitleScreen() => true;
        protected override bool WaitForEscapeKeyToHide() => true;

        public override void Initialize()
        {
            base.Initialize();
            AssignActionToButton(MyModdedObject, "BackButton", OnBackButtonClicked);
            AssignActionToButton(MyModdedObject, "DiscordServerButton", OnDiscordServerButtonClicked);
            AssignActionToButton(MyModdedObject, "SteamButton", OnAuthorSteamButtonClicked);
            AssignActionToButton(MyModdedObject, "ModBotButton", OnModBotButtonClicked);
            AssignActionToButton(MyModdedObject, "GitHubButton", OnGitHubButtonClicked);
        }

        public void OnBackButtonClicked()
        {
            Hide();
        }

        public void OnDiscordServerButtonClicked()
        {
            Application.OpenURL("https://discord.gg/RXN7uDUfwx");
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
