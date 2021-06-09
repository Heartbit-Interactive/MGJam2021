
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using TheCheapsLib;
using TheCheapsServer;

namespace TheCheaps
{
    public class NetworkClient
    {
        private NetPeerConfiguration config;
        private NetClient client;
        private int num_message = 0;
        private NetConnection connection;

        private GameInput input;
        public GameSimulation simulation;
        public NetworkClient(IPAddress ip, int port)
        {
            config = new NetPeerConfiguration("TheCheaps");
            client = new NetClient(config);
            client.Start();
            var string_ip = ip.ToString();
            connection = client.Connect(host: string_ip, port: port);
            this.simulation = new GameSimulation();
            this.input = new GameInput(simulation.model);
        }
        public void Update(GameTime gameTime)
        {
            ProcessIncomingMessages();
            SendMessage(ClientMessageType.ActionState, this.input.getActionState());
            num_message++;
        }
        private void ProcessIncomingMessages()
        {
            NetIncomingMessage msg = client.ReadMessage();
            while (msg != null)
            {
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
#if VERBOSE
                        System.Diagnostics.Debug.WriteLine($"{msg} received from server");
#endif
                        var type = (NetworkServer.MessageType)msg.ReadByte();
                        switch (type)
                        {
                            case NetworkServer.MessageType.PeerState:
                                ReadPeerState(msg);
                                break;
                            case NetworkServer.MessageType.SimulationState:
                                simulation.SetState(msg.Deserialize<SimulationState>());
                                break;
                            case NetworkServer.MessageType.PeerUpdate:
                            case NetworkServer.MessageType.SimulationUpdate:
                                throw new NotImplementedException();
                            default:
                                throw new Exception("Invalid message type");
                        }
                        break;
                    default:
                        break;

                }
                client.Recycle(msg);
                msg = client.ReadMessage();
            }
        }

        private void ReadPeerState(NetIncomingMessage msg)
        {
            throw new NotImplementedException();
        }

        private void SendMessage(ClientMessageType messageType, IBinarizable state, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.UnreliableSequenced)
        {
            NetOutgoingMessage msg = client.CreateMessage();
            byte[] array = null;
            using (var memstream = new MemoryStream(128 * 1024))
            {
                using (var bw = new BinaryWriter(memstream))
                {
                    state.BinaryWrite(bw);
                }
                array = memstream.ToArray();
            }
            msg.Write((byte)messageType);
            msg.Write(array.Length);
            msg.Write(array);
            client.SendMessage(msg, deliveryMethod);
#if VERBOSE
            System.Diagnostics.Debug.WriteLine("Sent {bytes.Length} bytes to server with GamepadState");
#endif
        }
    }
}