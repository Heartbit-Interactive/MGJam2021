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
            var server = ServerThreadManager.Start(12345,true);
            while (server.Status != NetPeerStatus.NotRunning)
            {
                System.Threading.Thread.Sleep(2);
            }
        }
    }
}
