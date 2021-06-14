using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheCheapsLib;

namespace TheCheapsServer
{
    public class ServerThreadManager
    {
        public static void Stop()
        {
            if(serverCancellation!=null)
                serverCancellation.Cancel();
            while(server!=null)
            {
                System.Threading.Thread.Sleep(1);
            }
        }

        static NetworkServer server;
        private static CancellationTokenSource serverCancellation;

        public static NetworkServer Start(int port, bool use_upnp)
        {
            serverCancellation = new CancellationTokenSource();
            var ct = serverCancellation.Token;
            System.Threading.Tasks.Task.Factory.StartNew(() => runServer(port,serverCancellation.Token, use_upnp), ct);
            while (server == null || !server.Started)
            {
                System.Threading.Thread.Sleep(1);
            }
            return server;
        }
        public static void runServer(int port,CancellationToken ctoken, bool use_upnp)
        {
            if (ctoken.IsCancellationRequested)
            {
                return;
            }
            server = new TheCheapsServer.NetworkServer(port, use_upnp);
            server.Start();

            var msperstep = 1000/(float)Settings.ServerTicksPerSecond;
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
                    return;
                }
                while (elapsedms < msperstep)
                {
                    System.Threading.Thread.Sleep(1);
                    if (ctoken.IsCancellationRequested)
                    {
                        server.Stop("Task Cancelled");
                        server.Dispose();
                        server = null;
                        return;
                    }
                    newms = DateTime.Now.Ticks / 10000;
                    elapsedms = newms - ms;
                }
            }
        }
    }
}
