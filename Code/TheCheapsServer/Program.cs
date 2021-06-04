#define VERBOSE
#define ECHO
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
                var message = server.WaitMessage(1000);
                if (message == null)
                {
#if VERBOSE
                    Console.WriteLine($"No messages recevied for 1s, time is {DateTime.Now.ToShortTimeString()}, Status is : {server.Status}");
#endif
                    continue;
                }
#if ECHO
                Console.WriteLine($"Message received {message}");
#endif
            }
        }
    }
}
