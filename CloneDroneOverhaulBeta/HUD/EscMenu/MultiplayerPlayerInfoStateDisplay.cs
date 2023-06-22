using CDOverhaul.NetworkAssets;
using ModLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    public class MultiplayerPlayerInfoStateDisplay : OverhaulBehaviour
    {
        private MultiplayerPlayerInfoState m_PlayerInfoState;
        private ModdedObject m_ModdedObject;

        private Text m_PlayerNameText;
        private Text m_PlayerPlatformText;
        private Text m_PlayerWinsText;
        private Image m_ColorIndicator;
        private RawImage m_RobotHeadImage;
        private Transform m_CDOUserTick;

        private Transform m_DetachedOrDisconnectedShading;

        public void Initialize(MultiplayerPlayerInfoState multiplayerPlayerInfo, ModdedObject moddedObject)
        {
            m_PlayerInfoState = multiplayerPlayerInfo;
            m_ModdedObject = moddedObject;

            m_PlayerNameText = m_ModdedObject.GetObject<Text>(0);
            m_PlayerPlatformText = m_ModdedObject.GetObject<Text>(1);
            m_ColorIndicator = m_ModdedObject.GetObject<Image>(2);
            m_DetachedOrDisconnectedShading = m_ModdedObject.GetObject<Transform>(4);
            m_RobotHeadImage = m_ModdedObject.GetObject<RawImage>(5);
            m_CDOUserTick = m_ModdedObject.GetObject<Transform>(6);
            m_PlayerWinsText = m_ModdedObject.GetObject<Text>(7);
            RefreshPlayerInfo();
        }

        public void RefreshPlayerInfo()
        {
            if (!m_PlayerInfoState || !m_ModdedObject)
                DestroyGameObject();

            m_DetachedOrDisconnectedShading.gameObject.SetActive(m_PlayerInfoState.IsDetached() || m_PlayerInfoState.state.IsDisconnected);
            if (m_PlayerInfoState.IsDetached())
            {
                m_CDOUserTick.gameObject.SetActive(false);
                m_PlayerNameText.text = string.Empty;
                m_PlayerPlatformText.text = string.Empty;
                m_ColorIndicator.color = Color.clear;
                m_PlayerWinsText.text = "N/A";
                return;
            }

            LoadRobotHead(m_PlayerInfoState.state.CharacterModelIndex, m_PlayerInfoState.state.FavouriteColor);
            m_DetachedOrDisconnectedShading.gameObject.SetActive(m_PlayerInfoState.state.IsDisconnected);
            m_CDOUserTick.gameObject.SetActive(m_PlayerInfoState.IsAnOverhaulModUser());
            m_PlayerNameText.text = (m_PlayerInfoState.IsAnOverhaulModUser() ? "<color=#FF3B26>[Overhaul]</color> " : string.Empty) + 
                (ModBotUserIdentifier.Instance.IsUsingModBot(m_PlayerInfoState.state.PlayFabID) ? "<color=#ffac00>[Mod-Bot]</color> " : string.Empty) +
                m_PlayerInfoState.state.DisplayName;
            m_PlayerPlatformText.text = GetPlatformString((PlayFab.ClientModels.LoginIdentityProvider)m_PlayerInfoState.state.PlatformID);
            m_PlayerWinsText.text = GameModeManager.IsBattleRoyale() ? m_PlayerInfoState.state.LastBotStandingWins.ToString() : "N/A";

            bool successfullyGotColor = false;
            HumanFavouriteColor humanFavouriteColor = null;
            try
            {
                humanFavouriteColor = HumanFactsManager.Instance.GetFavColor(m_PlayerInfoState.state.FavouriteColor);
                successfullyGotColor = true;
            }
            catch { }
            
            if (humanFavouriteColor == null || !successfullyGotColor)
            {
                m_ColorIndicator.color = Color.clear;
                return;
            }

            /*
            Color favouriteColor = humanFavouriteColor.ColorValue;
            if (ExclusiveColorsController.GetExclusivePlayerInfo(m_PlayerInfoState.state.PlayFabID, out ExclusiveColorInfo? info) && info != null && info.Value.ColorToReplace == m_PlayerInfoState.state.FavouriteColor)
                favouriteColor = info.Value.NewColor;

            m_ColorIndicator.color = favouriteColor;*/
        }

        protected override void OnDisposed()
        {
            OverhaulDisposable.AssignNullToAllVars(this);
        }

        public void LoadRobotHead(int characterModelIndex, int favouriteColorIndex)
        {
            string modelName = MultiplayerCharacterCustomizationManager.Instance.CharacterModels[characterModelIndex].Name;
            string filename = modelName + "_" + favouriteColorIndex + ".png";
            filename = filename.Replace("Bow 2", "Bow 1");
            if (filename.Contains("Business Bot")) filename = "Business Bot_0.png";
            if (filename.Contains("Emperor")) filename = "Emperor_0.png";
            if (filename.Contains("Sword 5")) filename = "Sword 5_0.png";

            OverhaulNetworkDownloadHandler n = new OverhaulNetworkDownloadHandler();
            n.DoneAction = delegate
            {
                if (!this || this.IsDisposedOrDestroyed() || n == null || n.Error)
                    return;

                m_RobotHeadImage.texture = n.DownloadedTexture;
            };
            OverhaulNetworkController.DownloadTexture("file://" + OverhaulMod.Core.ModDirectory + "Assets/RobotHeads/" + filename, n);
        }

        public static string GetPlatformString(PlayFab.ClientModels.LoginIdentityProvider login)
        {
            switch(login)
            {
                case PlayFab.ClientModels.LoginIdentityProvider.Custom:
                    return "<color=#cacaca>Custom</color>";
                case PlayFab.ClientModels.LoginIdentityProvider.CustomServer:
                    return "<color=#cacaca>Custom Server</color>";

                case PlayFab.ClientModels.LoginIdentityProvider.NintendoSwitch:
                    return "<color=#FF3B26>Switch</color>";
                case PlayFab.ClientModels.LoginIdentityProvider.NintendoSwitchAccount:
                    return "<color=#FF3B26>Switch</color>";
                case PlayFab.ClientModels.LoginIdentityProvider.PlayFab:
                    return "<color=#ffffff>PlayFab</color>";
                case PlayFab.ClientModels.LoginIdentityProvider.PSN:
                    return "<color=#4F85FF>PSN</color>";
                case PlayFab.ClientModels.LoginIdentityProvider.Steam:
                    return "<color=#2769E5>Steam</color>";
                case PlayFab.ClientModels.LoginIdentityProvider.Twitch:
                    return "<color=#A426E4>Twitch</color>";
                case PlayFab.ClientModels.LoginIdentityProvider.XBoxLive:
                    return "<color=#0DB30F>XBox</color>";
            }
            return "<color=#ffffff>N/A</color>";
        }
    }
}
