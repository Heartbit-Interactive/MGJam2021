using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheCheaps;
using TheCheapsServer;

namespace TheCheaps
{
    public class NetworkManager
    {
        private static NetworkServer server;
        private static NetworkClient _client;
        public static NetworkClient Client { get { return _client; } }
        private static CancellationTokenSource serverCancellation;
        private static int _port;

        internal static int Port
        {
            get { return _port; }
            set
            {
                if (value == _port)
                    return;
                _port = value;
                if (server != null && server.Started)
                {
                    StopServer();
                    StartServer();
                }
            }
        }

        public static IPAddress PublicIp { get; internal set; }
        public static bool ServerRunning { get { return server != null && server.Started; } }

        public static void StartServer()
        {
            serverCancellation = new CancellationTokenSource();
            var ct = serverCancellation.Token;
            System.Threading.Tasks.Task.Factory.StartNew(() => runServer(serverCancellation.Token), ct);
            while (server == null || !server.Started)
            {
                System.Threading.Thread.Yield();
                System.Threading.Thread.Sleep(1);
            }
        }
        private static void runServer(CancellationToken ctoken)
        {
            if (ctoken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }
            server = new TheCheapsServer.NetworkServer(_port);
            server.Start();
            var msperstep = 8;
            while (true)
            {
                //GESTIONE DEL CLOCK BASILARE
                var ms = DateTime.Now.Ticks / 10000;
                server.Tick();
                var newms = DateTime.Now.Ticks / 10000;
                var elapsedms = newms - ms;
                if (ctoken.IsCancellationRequested)
                {
                    server.Stop("Task Cancelled");
                    server.Dispose();
                    server = null;
                    throw new TaskCanceledException();
                }
                while (elapsedms < msperstep)
                {
                    System.Threading.Thread.Yield();
                    System.Threading.Thread.Sleep(1);
                    if (ctoken.IsCancellationRequested)
                    {
                        server.Stop("Task Cancelled");
                        server.Dispose();
                        server = null;
                        throw new TaskCanceledException();
                    }
                    newms = DateTime.Now.Ticks / 10000;
                    elapsedms = newms - ms;
                }
            }
        }

        public static void Update(GameTime time)
        {
            if (_client != null)
                _client.Update(time);
        }
        public static void StopServer()
        {
            serverCancellation.Cancel();
        }
        public static NetPeerStatus ServerStatus { get { return server != null ? server.Status : NetPeerStatus.NotRunning; } } 
        internal static void BeginJoin(IPAddress ip, int port)
        {
            if (server != null || _client != null)
                throw new InvalidOperationException();
            _client = new NetworkClient(ip,port);
        }

        internal static void BeginHost(int port)
        {
            if (server != null || _client!=null)
                throw new InvalidOperationException();
            _port = port;
            StartServer();
        }
    }
}
