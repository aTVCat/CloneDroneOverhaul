using Bolt;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Steamworks;

namespace CloneDroneOverhaul.LAN
{
    public class LANMultiplayerManager : MonoBehaviour
    {
        public static LANMultiplayerManager Instance;

        public Server LANServer;
        public Client LANClient;
        public SteamHostingTest SteamTest;

        public const string IP = "127.0.0.1";
        public const int Port = 8080;

        public static LANMultiplayerManager CreateManager()
        {
            LANMultiplayerManager.Instance = new GameObject("LANMultiplayerManager").AddComponent<LANMultiplayerManager>();
            LANMultiplayerManager.Instance.LANServer = new Server();
            LANMultiplayerManager.Instance.LANClient = new Client();
            LANMultiplayerManager.Instance.SteamTest = new SteamHostingTest();
;            return Instance;
        }

        public class SteamHostingTest
        {
            public void StartSteamHost()
            {
            }
        }

        public class Server
        {
            IPEndPoint tcpEndPoint;
            Socket tcpSocket;
            bool hasStarted;

            public void StartLANGame()
            {
                tcpEndPoint = new IPEndPoint(IPAddress.Parse(LANMultiplayerManager.IP), Port);
                tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                tcpSocket.Bind(tcpEndPoint);
                tcpSocket.Listen(5);
                hasStarted = true;
            }

            public void Update()
            {
                if (!hasStarted)
                {
                    return;
                }
                var listener = tcpSocket.Accept();
                var buffer = new byte[256];
                var size = 0;
                var data = new StringBuilder();

                size = listener.Receive(buffer);
                data.Append(Encoding.UTF8.GetString(buffer, 0, size));

                if(listener.Available > 0)
                {
                    Debug.Log(data);

                    listener.Send(Encoding.UTF8.GetBytes("Test"));

                    listener.Shutdown(SocketShutdown.Both);
                    listener.Close();
                }
            }
        }

        public class Client
        {
            IPEndPoint tcpEndPoint;
            Socket tcpSocket;
            bool hasStarted;
            public void StartClient()
            {
                tcpEndPoint = new IPEndPoint(IPAddress.Parse(IP), Port);
                tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                var message = Encoding.UTF8.GetBytes("Test message");

                tcpSocket.Connect(tcpEndPoint);
                tcpSocket.Send(message);
                hasStarted = true;
            }

            public void Update()
            {
                if (hasStarted)
                {
                    var buffer = new byte[256];
                    var size = 0;
                    var answer = new StringBuilder();
                    size = tcpSocket.Receive(buffer);
                    answer.Append(Encoding.UTF8.GetString(buffer, 0, size));

                    if (tcpSocket.Available > 0)
                    {
                        Debug.Log(answer.ToString());
                    }

                    tcpSocket.Shutdown(SocketShutdown.Both);
                    tcpSocket.Close();
                }
            }
        }
    }
}

