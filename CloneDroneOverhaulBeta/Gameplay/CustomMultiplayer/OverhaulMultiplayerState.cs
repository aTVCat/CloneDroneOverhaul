using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.CustomMultiplayer
{
   public static class OverhaulMultiplayerState
   {
        public static bool IsMultiplayerMode
        {
            get
            {
                return OverhaulMultiplayerController.Mode != EOverhaulMultiplayerMode.None;
            }
        }

        public static bool IsHost
        {
            get
            {
                if (!IsMultiplayerMode)
                    return true;

                OverhaulMultiplayerLobby lobby = OverhaulMultiplayerController.Lobby;
                return lobby.OwnerUserID == lobby.LocalUserID;
            }
        }
   }
}
