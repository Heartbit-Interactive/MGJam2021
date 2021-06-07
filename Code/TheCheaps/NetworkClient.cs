
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using TheCheapsLib;

namespace TheCheaps
{
    internal class NetworkClient : GameComponent
    {
        private NetPeerConfiguration config;
        private NetClient client;
        private int num_message = 0;
        private NetConnection connection;
        public NetworkClient(Game game) : base(game)
        {
            config = new NetPeerConfiguration("TheCheaps");
            client = new NetClient(config);
            client.Start();
            connection = client.Connect(host: "192.168.01.92", port: 12345);
        }
        public override void Initialize()
        {
            base.Initialize();
        }
        DataContractSerializer gamepadSerializer = new DataContractSerializer(typeof(GamePadState));
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            ReadSimulationState();
            var gpState = GamePad.GetState(0);
            var message = client.CreateMessage();
            using (var memstream = new MemoryStream(32 * 1024))
            {
                gamepadSerializer.WriteObject(memstream, gpState);
                var bytes = memstream.ToArray();
                message.Write(bytes.Length);
                message.Write(bytes);
#if VERBOSE
                System.Diagnostics.Debug.WriteLine("Sent {bytes.Length} bytes to server with GamepadState");
#endif
            }
            client.SendMessage(message, NetDeliveryMethod.UnreliableSequenced);
            num_message++;
        }

        private void ReadSimulationState()
        {
            var msg = client.ReadMessage();
            while (msg != null)
            {
#if VERBOSE
                System.Diagnostics.Debug.WriteLine($"{msg} received from server");
#endif
                var size = msg.ReadInt32();
                var content = msg.ReadBytes(size);
                msg = client.ReadMessage();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}