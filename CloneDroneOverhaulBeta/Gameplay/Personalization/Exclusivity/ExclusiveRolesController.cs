using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul
{
    internal static class ExclusiveRolesController
    {
        private static readonly Dictionary<string, ExclusivePlayerInfo> m_PlayerInfos = new Dictionary<string, ExclusivePlayerInfo>()
        {
            { "883CC7F4CA3155A3", new ExclusivePlayerInfo("A TVHuman", "Programmer,Clan Memeber,FCI", new Color(0.76f, 0.85f, 1, 0.87f), 4, "All@") },
             { "193564D7A14F9C33", new ExclusivePlayerInfo("Zolor", "Clan Memeber,FCI", new Color(0.45f, 0.04f, 0.65f, 1f), 10, "Purple Power Blade@") }
        };

        public static bool HasExclusiveAccess { get; private set; }

        public static void TryApplyExlusiveColorOnRobot(FirstPersonMover mover, Color curColor, out Color color)
        {
            color = curColor;
            if (mover == null)
            {
                return;
            }

            string playFabID = GameModeManager.IsMultiplayer() ? mover.GetPlayFabID() : ExclusivityController.GetLocalPlayfabID();
            m_PlayerInfos.TryGetValue(playFabID, out ExclusivePlayerInfo info);
            if (string.IsNullOrEmpty(info.Name))
            {
                return;
            }

            int index = 0;
            foreach (HumanFavouriteColor favColor in HumanFactsManager.Instance.FavouriteColors)
            {
                if (favColor.ColorValue.Equals(curColor) && index == info.ReplaceColorIndex)
                {
                    color = info.FavColor;
                    break;
                }
                index++;
            }
        }

        public static void OnGotPlayfabID(string playfabID)
        {
            HasExclusiveAccess = false;
            if (string.IsNullOrWhiteSpace(playfabID))
            {
                return;
            }
            if (GetExclusivePlayerInfo(playfabID, out ExclusivePlayerInfo? info))
            {
                if (info == null)
                {
                    return;
                }
                HasExclusiveAccess = true;
                //HumanFactsManager.Instance.FavouriteColors[info.Value.ReplaceColorIndex].ColorValue = info.Value.FavColor;
            }
        }

        public static bool GetExclusivePlayerInfo(string localID, out ExclusivePlayerInfo? info)
        {
            if (string.IsNullOrWhiteSpace(localID))
            {
                info = null;
                return false;
            }
            if (m_PlayerInfos.ContainsKey(localID))
            {
                info = m_PlayerInfos[localID];
                return true;
            }
            info = null;
            return false;
        }
    }
}
