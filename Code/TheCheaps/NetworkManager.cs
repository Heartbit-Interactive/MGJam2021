﻿using Lidgren.Network;
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
        private static NetworkClient _client;
        public static NetworkClient Client { get { return _client; } }
        public static NetworkServer Server { get { return _server; } }
        private static int _port = 12345;
        public static int ThisPeerUnique = Rand.Generator.Next();
        public static string ThisPlayerName = Rand.GeneratePlayerName();

        internal static int Port
        {
            get { return _port; }
            set
            {
                if (value == _port)
                    return;
                _port = value;
                if (_server != null && _server.Started)
                {
                    StopServer();
                    StartServer();
                }
            }
        }

        public static IPAddress PublicIp { get; internal set; }
        public static bool ServerRunning { get { return _server != null && _server.Started; } }

        public static void StartServer()
        {
            _server = ServerThreadManager.Start(_port);
        }

        public static void Update(GameTime time)
        {
            if (_client != null)
                _client.Update(time);
        }
        public static void StopServer()
        {
            ServerThreadManager.Stop();
        }
        public static NetPeerStatus ServerStatus { get { return _server != null ? _server.Status : NetPeerStatus.NotRunning; } }


        internal static void BeginJoin(IPAddress ip, int port)
        {
            if (_client != null)
                throw new InvalidOperationException();
            _client = new NetworkClient(ip,port);
        }

        internal static void BeginHost(int port)
        {
            if (_server != null || _client!=null)
                throw new InvalidOperationException();
            _port = port;
            StartServer();
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
