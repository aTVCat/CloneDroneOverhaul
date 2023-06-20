using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace CDOverhaul
{
    internal static class ExclusiveColorsController
    {
        private static Color GetCyberkickColor()
        {
            Color col = "#630330".ConvertHexToColor();
            Color resultColor = new Color(col.r, col.g, col.b, 0.87f);
            return resultColor;
        }

        private static readonly Dictionary<string, ExclusiveColorInfo> m_PlayerInfos = new Dictionary<string, ExclusiveColorInfo>()
        {
            { "883CC7F4CA3155A3", new ExclusiveColorInfo(new Color(0.76f, 0.85f, 1, 0.87f), 4) },
             { "193564D7A14F9C33", new ExclusiveColorInfo(new Color(0.45f, 0.04f, 0.65f, 1f), 10) },
              //{ "F08DA308234126FB", new ExclusiveColorInfo(GetCyberkickColor(), 10) },
               { "F08DA308234126FB", new ExclusiveColorInfo("#76ff7a".ConvertHexToColor(), 16) }
        };

        private static readonly ReadOnlyDictionary<string, ExclusiveColorInfo> m_PlayerColorsReadOnly = new ReadOnlyDictionary<string, ExclusiveColorInfo>(m_PlayerInfos);

        /// <summary>
        /// Does local player have unlocked exclusive colors?
        /// </summary>
        public static bool HasExclusiveAccess { get; private set; }

        public static void TryApplyExclusiveColorOnRobot(FirstPersonMover mover, Color curColor, out Color color)
        {
            color = curColor;
            if (mover == null)
                return;

            string playFabID = GameModeManager.IsMultiplayer() ? mover.GetPlayFabID() : ExclusivityController.GetLocalPlayFabID();
            _ = m_PlayerColorsReadOnly.TryGetValue(playFabID, out ExclusiveColorInfo info);
            if (info.ColorToReplace == 0)
                return;

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
            if (GetExclusivePlayerInfo(playfabID, out ExclusiveColorInfo? info) && info != null)
                HasExclusiveAccess = true;
        }

        public static bool GetExclusivePlayerInfo(string localID, out ExclusiveColorInfo? info)
        {
            info = null;
            if (!string.IsNullOrWhiteSpace(localID) && m_PlayerColorsReadOnly.ContainsKey(localID))
            {
                info = m_PlayerColorsReadOnly[localID];
                return true;
            }
            return false;
        }
    }
}
