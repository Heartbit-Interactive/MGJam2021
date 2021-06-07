using Lidgren.Network;
using System;
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
        List<NetConnection> peerConnections = new List<NetConnection>();
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
                sendState(server, peer);
        }

        DataContractSerializer gamepadDeserializer = new DataContractSerializer(typeof(GamePadState));
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
            if (!peerConnections.Contains(msg.SenderConnection))
            {
                peerConnections.Add(msg.SenderConnection);
                        Console.WriteLine("peer added");
            }
                    SimulationModel.gamepads.Clear();
                    var count = msg.ReadInt32();
                    var buffer = msg.ReadBytes(count);
                    using (var memstream = new MemoryStream(buffer))
                    {
                        var gpstate = deserializegamepadstate(memstream);
                        SimulationModel.gamepads.Add(gpstate);
                    }
                    break;
                default:
                    break;

            }
            server.Recycle(msg);
        }

        private GamePadState deserializegamepadstate(MemoryStream memstream)
        {
            using (var br = new BinaryReader(memstream))
            {
                var left = Vector2.Zero;
                left.X = br.ReadSingle();
                left.Y = br.ReadSingle();
                var buttonA = br.ReadBoolean();
                var buttonB = br.ReadBoolean();
                var button_list = new List<Buttons>();
                if (buttonA)
                    button_list.Add(Buttons.A);
                if (buttonB)
                    button_list.Add(Buttons.B);
                GamePadState gamePadState = new GamePadState(left, Vector2.Zero, 0, 0, button_list.ToArray());
                return gamePadState;
            }
            //bw.Write(gpState.ThumbSticks.Left.X);
            //bw.Write(gpState.ThumbSticks.Left.Y);
            //bw.Write(gpState.Buttons.A == ButtonState.Pressed);
            //bw.Write(gpState.Buttons.B == ButtonState.Pressed);
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
