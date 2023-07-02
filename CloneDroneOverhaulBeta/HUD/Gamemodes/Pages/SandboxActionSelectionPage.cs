using InjectionClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using CDOverhaul.MultiplayerSandbox;

namespace CDOverhaul.HUD.Gamemodes
{
    public class SandboxActionSelectionPage : FullscreenWindowPageBase
    {
        private Button m_CreateLobbyButton;
        private Button m_JoinButton;

        public override void Initialize(OverhaulGamemodesUIFullscreenWindow fullscreenWindow)
        {
            base.Initialize(fullscreenWindow);

            m_CreateLobbyButton = MyModdedObject.GetObject<Button>(0);
            m_CreateLobbyButton.onClick.AddListener(OnCreateLobbyClick);
            m_JoinButton = MyModdedObject.GetObject<Button>(1);
            m_JoinButton.onClick.AddListener(OnJoinLobbyClick);
        }

        public void OnCreateLobbyClick()
        {
            FullscreenWindow.GamemodesUI.Hide();
            MultiplayerSandboxController.Instance.CreateLobby();
        }

        public void OnJoinLobbyClick()
        {
            FullscreenWindow.GoToPage(5);
        }
    }
}
