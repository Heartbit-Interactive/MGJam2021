using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheCheapsServer
{
    public class ServerThreadManager
    {
        public static void Stop()
        {
            serverCancellation.Cancel();
        }

        static NetworkServer server;
        private static CancellationTokenSource serverCancellation;

        public static NetworkServer Start(int port)
        {
            serverCancellation = new CancellationTokenSource();
            var ct = serverCancellation.Token;
            System.Threading.Tasks.Task.Factory.StartNew(() => runServer(port,serverCancellation.Token), ct);
            while (server == null || !server.Started)
            {
                System.Threading.Thread.Sleep(1);
            }
            return server;
        }
        public static void runServer(int port,CancellationToken ctoken)
        {
            if (ctoken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }
            server = new TheCheapsServer.NetworkServer(port);
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
    }
}
