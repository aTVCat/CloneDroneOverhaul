using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul
{
    internal static class ExclusiveRolesController
    {
        private static readonly Dictionary<string, ExclusivePlayerInfo> _players = new Dictionary<string, ExclusivePlayerInfo>()
        {
            { "883CC7F4CA3155A3", new ExclusivePlayerInfo("A TVHuman", "Programmer,Clan Memeber,FCI", new Color(0.76f, 0.85f, 1, 0.95f), 4, "All@") },
             { "193564D7A14F9C33", new ExclusivePlayerInfo("Zolor", "Clan Memeber,FCI", new Color(0.45f, 0.04f, 0.65f, 0.96f), 10, "Purple Power Blade@") },
        };

        public static bool HasExclusiveAccess { get; private set; }

        public static void TryApplyExclusivityOnRobot(FirstPersonMover mover, Color curColor, out Color color)
        {
            if(mover == null)
            {
                color = curColor;
                return;
            }

            string playfabID = mover.GetPlayFabID();
            if (string.IsNullOrEmpty(playfabID) ||
                playfabID.Equals(ExclusivityController.GetLocalPlayfabID()) ||
                !_players.ContainsKey(playfabID))
            {
                color = curColor;
                return;
            }

            foreach(HumanFavouriteColor favColor in HumanFactsManager.Instance.FavouriteColors)
            {
                if(favColor.ColorValue == curColor)
                {
                    ExclusivePlayerInfo info = _players[playfabID];
                    color = info.FavColor;
                    break;
                }
            }
            color = curColor;
        }

        public static void OnGotPlayfabID(string playfabID)
        {
            HasExclusiveAccess = false; 
            if (string.IsNullOrWhiteSpace(playfabID))
            {
                return;
            }
            if(GetExclusivePlayerInfo(playfabID, out ExclusivePlayerInfo? info))
            {
                if(info == null)
                {
                    return;
                }
                HasExclusiveAccess = true;
                HumanFactsManager.Instance.FavouriteColors[info.Value.ReplaceColorIndex].ColorValue = info.Value.FavColor;
            }
        }

        public static bool GetExclusivePlayerInfo(string localID, out ExclusivePlayerInfo? info)
        {
            if (string.IsNullOrWhiteSpace(localID))
            {
                info = null;
                return false;
            }
            if (_players.ContainsKey(localID))
            {
                info = _players[localID];
                return true;
            }
            info = null;
            return false;
        }
    }
}
