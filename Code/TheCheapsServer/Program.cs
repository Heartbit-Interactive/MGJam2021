#define VERBOSE
#define ECHO
using Lidgren.Network;
using System;
using System.Net;
using TheCheaps;

namespace TheCheapsServer
{
    class Program
    {
        static GameSimulation simulation;
        static void Main(string[] args)
        {
            var config = new Lidgren.Network.NetPeerConfiguration("TheCheaps");
            config.Port = 12345;
            var server = new Lidgren.Network.NetServer(config);
            server.Start();
            Console.WriteLine($"Server for {config.AppIdentifier} starting... Listening on IP:{GetLocalIPAddress()} on port {server.Port}");
            simulation = new GameSimulation();
            simulation.Start();
            while (true)
            {
                NetIncomingMessage msg = server.ReadMessage();
                if (msg == null)
                {
#if VERBOSE
                    Console.WriteLine($"No messages recevied for 1s, time is {DateTime.Now.ToShortTimeString()}, Status is : {server.Status}");
#endif
                    continue;
                }
#if ECHO
                Console.WriteLine($"Message received {msg}");
#endif
                process_message(server, msg);
                simulation.Step();
            }
        }

        private static void process_message(NetServer server, NetIncomingMessage msg)
        {
            switch (msg.MessageType)
            {

                case NetIncomingMessageType.VerboseDebugMessage:
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.ErrorMessage:
                    Console.WriteLine(msg.ReadString());
                    break;
                default:
                    var text = "Unhandled type: " + msg.MessageType;
                    Console.WriteLine(text);
                    NetOutgoingMessage sendMsg = server.CreateMessage();
                    sendMsg.Write(simulation.X);
                    sendMsg.Write(simulation.Y);
                    server.SendMessage(sendMsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                    break;

            }
            server.Recycle(msg);
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
