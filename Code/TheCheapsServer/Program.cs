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
            var config = new Lidgren.Network.NetPeerConfiguration("TheCheaps");
            config.Port = 12345;
            var server = new Lidgren.Network.NetServer(config);
            server.Start();
            Console.WriteLine($"Server for {config.AppIdentifier} starting... Listening on IP:{GetLocalIPAddress()} on port {server.Port}");
            
            GameSimulation.Start();
            var msperstep = 8;
            while (true)
            {
                //GESTIONE DEL CLOCK BASILARE
                var ms = DateTime.Now.Ticks/10000;
                process_message(server);
                GameSimulation.Step();
                foreach(var peer in peerConnections)
                    sendState(server, peer);
                var newms = DateTime.Now.Ticks / 10000;
                var elapsedms = newms - ms;
                while(elapsedms<msperstep)
                {
                    System.Threading.Thread.Yield();
                    System.Threading.Thread.Sleep(1);
                    newms = DateTime.Now.Ticks / 10000;
                    elapsedms = newms - ms;
                }
            }
        }
        static List<NetConnection> peerConnections;
        private static void process_message(NetServer server)
        {
            NetIncomingMessage msg = server.ReadMessage();
            if (msg == null)
            {
#if VERBOSE
                Console.WriteLine($"No messages recevied for 1s, time is {DateTime.Now.ToShortTimeString()}, Status is : {server.Status}");
#endif
                return;
            }
#if ECHO
            Console.WriteLine($"Message received {msg}");
#endif
            switch (msg.MessageType)
            {

                case NetIncomingMessageType.VerboseDebugMessage:
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.ErrorMessage:
                    Console.WriteLine(msg.ReadString());
                    break;
                default:
                    break;

            }
            server.Recycle(msg);
        }

        private static void sendState(NetServer server, NetConnection destinationConnection)
        {
            NetOutgoingMessage msg = server.CreateMessage();
            msg.Write(GameSimulation.GetSerializedState());
            server.SendMessage(msg, destinationConnection, NetDeliveryMethod.UnreliableSequenced);
        }

        static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
