﻿using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul
{
    internal static class ExclusiveRolesController
    {
        private static readonly Dictionary<string, ExclusivePlayerInfo> _players = new Dictionary<string, ExclusivePlayerInfo>()
        {
            { "883CC7F4CA3155A3", new ExclusivePlayerInfo("A TVHuman", "Programmer,Clan Memeber,FCI", new Color(0.76f, 0.85f, 1, 1), 4, "All@") },
             { "193564D7A14F9C33", new ExclusivePlayerInfo("Zolor", "Clan Memeber,FCI", new Color(0.45f, 0.04f, 0.65f, 1), 10, "Purple Power Blade@") },
        };

        public static void OnGotPlayfabID(string playfabID)
        {
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
