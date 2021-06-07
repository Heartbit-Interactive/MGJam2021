using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using TheCheapsLib;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace TheCheapsServer
{
    class NetworkServer
    {
        NetServer server;
        List<NetConnection> peerConnections;
        private NetPeerConfiguration config;
        public NetworkServer()
        {

            config = new Lidgren.Network.NetPeerConfiguration("TheCheaps");
            config.Port = 12345;
            server = new Lidgren.Network.NetServer(config);
        }
        internal void Start()
        {
            server.Start();
            Console.WriteLine($"Server for {config.AppIdentifier} starting... Listening on IP:{GetLocalIPAddress()} on port {server.Port}");
        }
        internal void Tick()
        {
            process_message(server);
            GameSimulation.Step();
            foreach (var peer in peerConnections)
                sendState(server, peer);
        }

        DataContractSerializer gamepadDeserializer = new DataContractSerializer(typeof(GamePadState));
        private void process_message(NetServer server)
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
                case NetIncomingMessageType.Data:
                    SimulationModel.gamepads.Clear();
                    var count = msg.ReadInt32();
                    var buffer = msg.ReadBytes(count);
                    using (var memstream = new MemoryStream(buffer))
                    {
                        var gpstate = (GamePadState)gamepadDeserializer.ReadObject(memstream);
                        SimulationModel.gamepads.Add(gpstate);
                    }
                    break;
                default:
                    break;

            }
            server.Recycle(msg);
        }

        private void sendState(NetServer server, NetConnection destinationConnection)
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
