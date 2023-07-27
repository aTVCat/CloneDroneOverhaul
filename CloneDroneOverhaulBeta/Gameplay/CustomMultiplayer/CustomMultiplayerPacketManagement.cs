using Steamworks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace CDOverhaul.CustomMultiplayer
{
    public static class CustomMultiplayerPacketManagement
    {
        public static int FixedFrames;

        public static void SendToEveryone(this OverhaulPacket packet, EP2PSend sendType = EP2PSend.k_EP2PSendUnreliable)
        {
            if (!CheckPacket(packet, out byte[] array))
                return;

            CustomMultiplayerLobby lobby = CustomMultiplayerController.Lobby;
            CSteamID[] users = lobby.GetLobbyMemberSteamIDs();
            int all = lobby.GetMemberCount();
            int index = 0;
            do
            {
                CSteamID steamID = users[index];
                if (steamID.IsExcluded())
                {
                    index++;
                    continue;
                }

                packet.SendTo(steamID, array, sendType);
                index++;
            } while (index < all);
        }

        public static void SendToOthers(this OverhaulPacket packet, EP2PSend sendType = EP2PSend.k_EP2PSendUnreliable)
        {
            if (!CheckPacket(packet, out byte[] array))
                return;

            CustomMultiplayerLobby lobby = CustomMultiplayerController.Lobby;
            CSteamID owner = SteamUser.GetSteamID();
            CSteamID[] users = lobby.GetLobbyMemberSteamIDs();
            int all = lobby.GetMemberCount();
            int index = 0;
            do
            {
                CSteamID steamID = users[index];
                if (steamID.IsExcluded(owner))
                {
                    index++;
                    continue;
                }

                packet.SendTo(steamID, array, sendType);
                index++;
            } while (index < all);
        }

        public static void SendToHost(this OverhaulPacket packet, EP2PSend sendType = EP2PSend.k_EP2PSendUnreliable)
        {
            if (!CheckPacket(packet, out byte[] array))
                return;

            CSteamID cSteamID = CustomMultiplayerController.Lobby.GetLobbyOwner();
            packet.SendTo(cSteamID, array, sendType);
        }

        public static void SendTo(this OverhaulPacket packet, CSteamID steamID, byte[] data, EP2PSend sendType = EP2PSend.k_EP2PSendUnreliable)
        {
            if (data.IsNullOrEmpty() && !CheckPacket(packet, out data))
                return;

            _ = SteamNetworking.SendP2PPacket(steamID, data, (uint)data.Length, sendType, packet.GetChannel());
        }

        public static bool CheckPacket(this OverhaulPacket packet, out byte[] bytes)
        {
            bytes = null;

            if (!CustomMultiplayerController.FullInitialization)
                return false;

            if (packet == null)
            {
                Debug.LogWarning("[CDO_MS] Packet is null!");
                return false;
            }

            bytes = packet.SerializeObject();
            if (bytes.IsNullOrEmpty())
            {
                Debug.LogWarning("[CDO_MS] Byte array is empty!");
                return false;
            }
            return true;
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
                        OverhaulPacket receivedPacket = array.GetPacket();
                        receivedPacket.Handle();
                    }
                    else
                    {
                        Debug.LogWarning("[CDO_MS] We got empty byte array!");
                        continue;
                    }
                }
            }
        }

        public static OverhaulPacket GetPacket(this byte[] array)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(array, 0, array.Length);
                _ = memoryStream.Seek(0, SeekOrigin.Begin);
                BinaryFormatter formatter = new BinaryFormatter();
                return (OverhaulPacket)formatter.Deserialize(memoryStream);
            }
        }
    }
}
