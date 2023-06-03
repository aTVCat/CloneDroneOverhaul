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
        private Image m_ColorIndicator;
        private Transform m_PlayerIsHostIndicator;

        private Transform m_DetachedOrDisconnectedShading;

        public void Initialize(MultiplayerPlayerInfoState multiplayerPlayerInfo, ModdedObject moddedObject)
        {
            m_PlayerInfoState = multiplayerPlayerInfo;
            m_ModdedObject = moddedObject;

            m_PlayerNameText = m_ModdedObject.GetObject<Text>(0);
            m_PlayerPlatformText = m_ModdedObject.GetObject<Text>(1);
            m_ColorIndicator = m_ModdedObject.GetObject<Image>(2);
            m_PlayerIsHostIndicator = m_ModdedObject.GetObject<Transform>(3);
            m_DetachedOrDisconnectedShading = m_ModdedObject.GetObject<Transform>(4);
            RefreshPlayerInfo();
        }

        public void RefreshPlayerInfo()
        {
            if (!m_PlayerInfoState || !m_ModdedObject)
                DestroyGameObject();

            m_DetachedOrDisconnectedShading.gameObject.SetActive(m_PlayerInfoState.IsDetached() || m_PlayerInfoState.state.IsDisconnected);
            if (m_PlayerInfoState.IsDetached())
            {
                m_PlayerIsHostIndicator.gameObject.SetActive(false);
                m_PlayerNameText.text = string.Empty;
                m_PlayerPlatformText.text = string.Empty;
                m_ColorIndicator.color = Color.clear;
                return;
            }

            m_DetachedOrDisconnectedShading.gameObject.SetActive(false);
            m_PlayerIsHostIndicator.gameObject.SetActive(m_PlayerInfoState.state.IsHost);
            m_PlayerNameText.text = (ModBotUserIdentifier.Instance.IsUsingModBot(m_PlayerInfoState.state.PlayFabID) ? "<color=#ffac00>[Mod-Bot]</color> " : string.Empty) +
                m_PlayerInfoState.state.DisplayName;
            m_PlayerPlatformText.text = ((PlayFab.ClientModels.LoginIdentityProvider)m_PlayerInfoState.state.PlatformID).ToString();

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

            Color favouriteColor = humanFavouriteColor.ColorValue;
            favouriteColor.a = 0.25f;
            m_ColorIndicator.color = favouriteColor;
        }

        protected override void OnDisposed()
        {
            OverhaulDisposable.AssignNullToAllVars(this);
        }
    }
}
