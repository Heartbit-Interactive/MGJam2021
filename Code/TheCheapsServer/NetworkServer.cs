using Lidgren.Network;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using TheCheapsLib;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Microsoft.Xna.Framework;

namespace TheCheapsServer
{
    class NetworkServer
    {
        NetServer server;
        NetConnection[] peerConnections = new NetConnection[Settings.maxPlayers];
        Dictionary<NetConnection, int> connectionToPlayerMapping = new Dictionary<NetConnection, int>();
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
            process_message();
            GameSimulation.Step();
            foreach (var peer in peerConnections)
            {
                if(peer!=null)
                    sendState(server, peer);
            }
        }
        private void process_message()
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
                    var conn = msg.SenderConnection;
                    var pl_index = 0;
                    if (!connectionToPlayerMapping.TryGetValue(conn,out pl_index))
                    {
                        pl_index = addPeer(msg, conn);
                    }
                    var count = msg.ReadInt32();
                    var buffer = msg.ReadBytes(count);
                    GameInput.deserializeInputState(buffer,pl_index);
                    break;
                default:
                    break;

            }
            server.Recycle(msg);
        }

        private int addPeer(NetIncomingMessage msg, NetConnection conn)
        {
            int pl_index = Array.IndexOf(peerConnections, null);
            if (pl_index >= 0)
            {
                Console.WriteLine($"Peer added at index {pl_index}");
                connectionToPlayerMapping[conn] = pl_index;
                peerConnections[pl_index] = msg.SenderConnection;
            }

            return pl_index;
        }

        private void sendState(NetServer server, NetConnection destinationConnection)
        {
            NetOutgoingMessage msg = server.CreateMessage();
            var array = GameSimulation.GetSerializedState();
            msg.Write(array.Length);
            msg.Write(array);
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
