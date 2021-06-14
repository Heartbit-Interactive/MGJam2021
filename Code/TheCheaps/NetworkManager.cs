using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheCheaps;
using TheCheapsLib;
using TheCheapsServer;

namespace TheCheaps
{
    public class NetworkManager
    {
        private static NetworkServer _server;

        public static string LocalIp { get; private set; }

        private static NetworkClient _client;
        public static NetworkClient Client { get { return _client; } }
        public static NetworkServer Server { get { return _server; } }
        public static int ThisPeerUnique = Rand.Generator.Next();
        public static string ThisPlayerName = Rand.GeneratePlayerName();

        internal static int Port
        {
            get;
            private set;
        }
        internal static int ExternalPort
        {
            get;
            private set;
        }

        public static IPAddress PublicIp { get; internal set; }
        static NetworkManager()
        {
            Port = 12345;
        }
        public static bool ServerRunning { get { return _server != null && _server.Started; } }

        public static void StartServer(bool use_upnp)
        {
            _server = ServerThreadManager.Start(Port, use_upnp);
            LocalIp = NetworkServer.GetLocalIPAddress();
            ExternalPort = _server.ExternalPort;
        }

        public static void Update(GameTime time)
        {
            if (_client != null)
                _client.Update(time);
        }
        public static void StopServer()
        {            
            ServerThreadManager.Stop();
            _server = null;
        }
        public static NetPeerStatus ServerStatus { get { return _server != null ? _server.Status : NetPeerStatus.NotRunning; } }


        internal static void BeginJoin(IPAddress ip, int port, bool use_upnp)
        {
            if (_client != null)
                throw new InvalidOperationException();
            _client = new NetworkClient(ip,port,use_upnp);
        }

        internal static void BeginHost(int port,bool use_upnp)
        {
            if (_server != null || _client!=null)
                throw new InvalidOperationException();
            Port = port;
            StartServer(use_upnp);
        }

        internal static void StopClient()
        {
            _client.Disconnect();
            _client.Dispose();
            _client = null;
        }

        internal static void StartCountDown()
        {
            _server.StartCountDown();
        }
    }
}
