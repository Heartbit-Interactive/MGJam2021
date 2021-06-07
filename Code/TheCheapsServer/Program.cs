#define VERBOSE
#define ECHO
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Net;
using TheCheapsLib;

namespace TheCheapsServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new NetworkServer();
            server.Start();
            GameSimulation.Start();
            var msperstep = 8;
            while (true)
            {
                //GESTIONE DEL CLOCK BASILARE
                var ms = DateTime.Now.Ticks / 10000;
                server.Tick();
                var newms = DateTime.Now.Ticks / 10000;
                var elapsedms = newms - ms;
                while (elapsedms < msperstep)
                {
                    System.Threading.Thread.Yield();
                    System.Threading.Thread.Sleep(1);
                    newms = DateTime.Now.Ticks / 10000;
                    elapsedms = newms - ms;
                }
            }
        }
    }
}
