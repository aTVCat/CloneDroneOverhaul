using Steamworks;
using UnityEngine;

namespace CDOverhaul.CustomMultiplayer
{
    public static class OverhaulMultiplayerPacketManagement
    {
        public static bool PreparePacket(this OverhaulPacket packet, out byte[] bytes)
        {
            bytes = null;
            if (packet == null)
            {
                Debug.LogWarning("[CDO_MS] Packet is null!");
                return false;
            }

            bytes = packet.SerializeObject();
            return true;
        }

        public static void SendToEveryone(this OverhaulPacket packet, EP2PSend sendType = EP2PSend.k_EP2PSendUnreliable)
        {
            if (!PreparePacket(packet, out byte[] array))
                return;

            CSteamID[] users = OverhaulMultiplayerManager.Lobby.Members;
            int all = users.Length;
            int index = 0;
            do
            {
                packet.Send(users[index], array, sendType);
                index++;
            } while (index < all);
        }

        public static void SendToOthers(this OverhaulPacket packet, EP2PSend sendType = EP2PSend.k_EP2PSendUnreliable)
        {
            if (!PreparePacket(packet, out byte[] array))
                return;

            OverhaulMultiplayerLobby lobby = OverhaulMultiplayerManager.Lobby;
            CSteamID owner = lobby.LocalUserID;
            CSteamID[] users = lobby.Members;
            int all = users.Length;
            int index = 0;
            do
            {
                CSteamID steamID = users[index];
                if (steamID == owner)
                {
                    index++;
                    continue;
                }

                packet.Send(steamID, array, sendType);
                index++;
            } while (index < all);
        }

        public static void SendToHost(this OverhaulPacket packet, EP2PSend sendType = EP2PSend.k_EP2PSendUnreliable)
        {
            if (!PreparePacket(packet, out byte[] array))
                return;

            CSteamID cSteamID = OverhaulMultiplayerManager.Lobby.OwnerUserID;
            packet.Send(cSteamID, array, sendType);
        }

        public static void Send(this OverhaulPacket packet, CSteamID steamID, byte[] data, EP2PSend sendType = EP2PSend.k_EP2PSendUnreliable)
        {
            if (steamID.IsNil())
                return;

            if (data.IsNullOrEmpty() && !PreparePacket(packet, out data))
                return;

            _ = SteamNetworking.SendP2PPacket(steamID, data, (uint)data.Length, sendType, packet.GetChannel());
        }

        public static void HandleIncomingPackets()
        {
            for (int i = 0; i < 3; i++)
            {
                while (SteamNetworking.IsP2PPacketAvailable(out uint s, i))
                {
                    byte[] array = new byte[s];
                    if (!SteamNetworking.ReadP2PPacket(array, s, out _, out _, i))
                    {
                        Debug.LogWarning("[CDO_MS] Cannot read P2P packet!");
                        continue;
                    }

                    if (!array.IsNullOrEmpty())
                    {
                        OverhaulPacket receivedPacket = array.DeserializeObject<OverhaulPacket>();
                        if (receivedPacket != default || receivedPacket != null)
                        {
                            receivedPacket.Handle();
                        }
                    }
                    else
                    {
                        Debug.LogWarning("[CDO_MS] We got empty byte array!");
                        continue;
                    }
                }
            }
        }
    }
}
