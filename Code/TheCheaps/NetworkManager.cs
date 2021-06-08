using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheCheapsServer
{
    public class NetworkManager
    {
        private static NetworkServer server;
        private static CancellationTokenSource serverCancellation;

        public static IPAddress PublicIp { get; internal set; }

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
            server = new TheCheapsServer.NetworkServer();
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
                        throw new TaskCanceledException();
                    }
                    newms = DateTime.Now.Ticks / 10000;
                    elapsedms = newms - ms;
                }
            }
        }

        public static void StopServer()
        {
            serverCancellation.Cancel();
        }
    }
}
