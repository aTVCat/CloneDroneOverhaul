using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul
{
    internal static class ExclusiveColorsController
    {
        private static readonly Dictionary<string, ExclusiveColorInfo> m_PlayerInfos = new Dictionary<string, ExclusiveColorInfo>()
        {
            { "883CC7F4CA3155A3", new ExclusiveColorInfo(new Color(0.76f, 0.85f, 1, 0.87f), 4) },
             { "193564D7A14F9C33", new ExclusiveColorInfo(new Color(0.45f, 0.04f, 0.65f, 1f), 10) }
        };

        /// <summary>
        /// Does local player have unlocked exclusive colors?
        /// </summary>
        public static bool HasExclusiveAccess { get; private set; }

        public static void TryApplyExclusiveColorOnRobot(FirstPersonMover mover, Color curColor, out Color color)
        {
            color = curColor;
            if (mover == null)
            {
                return;
            }

            string playFabID = GameModeManager.IsMultiplayer() ? mover.GetPlayFabID() : ExclusivityController.GetLocalPlayfabID();
            m_PlayerInfos.TryGetValue(playFabID, out ExclusiveColorInfo info);
            if (info.ColorToReplace == 0)
            {
                return;
            }

            int index = 0;
            foreach (HumanFavouriteColor favColor in HumanFactsManager.Instance.FavouriteColors)
            {
                if (favColor.ColorValue.Equals(curColor) && index == info.ColorToReplace)
                {
                    color = info.NewColor;
                    break;
                }
                index++;
            }
        }

        public static void OnGotPlayfabID(string playfabID)
        {
            HasExclusiveAccess = false;
            if (string.IsNullOrWhiteSpace(playfabID)) return;
            if (GetExclusivePlayerInfo(playfabID, out ExclusiveColorInfo? info))
            {
                if (info != null) HasExclusiveAccess = true;
            }
        }

        public static bool GetExclusivePlayerInfo(string localID, out ExclusiveColorInfo? info)
        {
            info = null;
            if (!string.IsNullOrWhiteSpace(localID) && m_PlayerInfos.ContainsKey(localID))
            {
                info = m_PlayerInfos[localID];
                return true;
            }
            return false;
        }
    }
}
