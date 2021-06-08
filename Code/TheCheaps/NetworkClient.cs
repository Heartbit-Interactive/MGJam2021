
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
        public NetworkClient(IPAddress ip,int port)
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
            ReadSimulationState();
            var message = client.CreateMessage();
#if OLD

            var bytes = this.input.serializeInputState();
#else
            var bytes = this.input.serializeActionState();
#endif
#if VERBOSE
            System.Diagnostics.Debug.WriteLine("Sent {bytes.Length} bytes to server with GamepadState");
#endif
            message.Write(bytes.Length);
            message.Write(bytes);
            client.SendMessage(message, NetDeliveryMethod.UnreliableSequenced);
            num_message++;
        }

        private void ReadSimulationState()
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
                        var size = msg.ReadInt32();
                        var content = msg.ReadBytes(size);
                        simulation.DeserializeState(content);
                        break;
                    default:
                        break;

                }
                client.Recycle(msg);
                msg = client.ReadMessage();
            }
        }
    }
}