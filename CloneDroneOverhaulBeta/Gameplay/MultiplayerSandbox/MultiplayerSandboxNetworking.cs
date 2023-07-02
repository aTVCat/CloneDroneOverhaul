using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace CDOverhaul.MultiplayerSandbox
{
    public static class MultiplayerSandboxNetworking
    {
        public static int FixedFrames;

        public static void SendToEveryone(this Packet packet)
        {
            if (!CheckPacket(packet, out byte[] array))
                return;

            MultiplayerSandboxLobby lobby = MultiplayerSandboxController.Instance.Lobby;
            CSteamID[] users = lobby.GetLobbyMemberSteamIDs();
            int all = lobby.GetMemberCount();
            int index = 0;
            do
            {
                CSteamID steamID = users[index];
                if(steamID.IsExcluded())
                {
                    index++;
                    continue;
                }

                packet.SendTo(steamID, array);
                index++;
            } while (index < all);
        }

        public static void SendToOthers(this Packet packet)
        {
            if (!CheckPacket(packet, out byte[] array))
                return;

            MultiplayerSandboxLobby lobby = MultiplayerSandboxController.Instance.Lobby;
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

                packet.SendTo(steamID, array);
                index++;
            } while (index < all);
        }

        public static void SendToHost(this Packet packet)
        {
            if (!CheckPacket(packet, out byte[] array))
                return;

            CSteamID cSteamID = MultiplayerSandboxController.Instance.Lobby.GetLobbyOwner();
            packet.SendTo(cSteamID, array);
        }

        public static void SendTo(this Packet packet, CSteamID steamID, byte[] data)
        {
            if (data.IsNullOrEmpty() && !CheckPacket(packet, out data))
                return;

            SteamNetworking.SendP2PPacket(steamID, data, (uint)data.Length, packet.GetSendType(), packet.GetChannel());
        }

        public static bool CheckPacket(this Packet packet, out byte[] bytes)
        {
            bytes = null; 

            if (!MultiplayerSandboxController.FullInitialization)
                return false;

            if (packet == null)
            {
                Debug.LogWarning("[CDO_MS] Packet is null!");
                return false;
            }

            bytes = packet.GetBytes();
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
                    if(!SteamNetworking.ReadP2PPacket(array, s, out uint msgSize, out CSteamID owner, i))
                    {
                        Debug.LogWarning("[CDO_MS] Cannot read P2P packet!");
                        continue;
                    }

                    if (!array.IsNullOrEmpty())
                    {
                        Packet receivedPacket = Packet.GetPacket(array);
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

        [Serializable]
        public class Packet
        {
            public ulong SteamID;

            public virtual void Handle()
            {
                Debug.Log("Houston, we got a packet with number 1!!");
            }

            public virtual EP2PSend GetSendType() => EP2PSend.k_EP2PSendUnreliable;
            public virtual int GetChannel() => 0;

            public byte[] GetBytes()
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    bf.Serialize(ms, this);
                    return ms.ToArray();
                }
            }

            public static Packet GetPacket(byte[] array)
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    BinaryFormatter binForm = new BinaryFormatter();
                    memStream.Write(array, 0, array.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    return (Packet)binForm.Deserialize(memStream);
                }
            }
        }
    }
}
