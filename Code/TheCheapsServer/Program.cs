#define VERBOSE
#define ECHO
using Lidgren.Network;
using System;

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
            Console.WriteLine($"Server for {config.AppIdentifier} starting... Status is : {server.Status} on port {server.Port}");
            while (true)
            {
                NetIncomingMessage msg = server.WaitMessage(1000);
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

                while ((msg = server.ReadMessage()) != null)
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

                            Console.WriteLine("Unhandled type: " + msg.MessageType);

                            break;

                    }

                    server.Recycle(msg);

                }


            }
        }
    }
}
