﻿using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheaps
{
    internal class NetworkClient : GameComponent
    {
        private NetPeerConfiguration config;
        private NetClient client;
        private int num_message = 0;
        private NetConnection connection;
        private int counter = 0;
        public int posx;
        public int posy;
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var msg = client.ReadMessage();
            while (msg != null)
            {
                System.Diagnostics.Debug.WriteLine($"{msg} received from server");
                posx = msg.ReadInt32();
                posy = msg.ReadInt32();
                msg = client.ReadMessage();
            }
            if (counter >= 5)
            {
                var message = client.CreateMessage();
                var text = $"this is message {num_message} connection is {connection.Status}";
                message.Write(text);
                System.Diagnostics.Debug.WriteLine(text);
                client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
                num_message++;
                counter = 0;
            }
            counter++;


        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}