﻿using Lidgren.Network;
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
    public class NetworkServer
    {
        NetServer server;
        private NetPeerConfiguration config;
        private GameInput input;
        private GameSimulation simulation;
        private GameNetwork network;
        public bool Started { get { return _started; } }
        private bool _started;
        public NetPeerStatus Status { get { return server.Status; } }
        public int CurrentPort { get { return server.Configuration.Port; } }

        public NetworkServer(int port_suggestion)
        {
            config = new Lidgren.Network.NetPeerConfiguration("TheCheaps");
            config.EnableUPnP = true;
            config.MaximumConnections = 4;
            //config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            int server_port = port_suggestion;
            bool success = false;
            while (server_port < port_suggestion + 10)
            {
                try
                {
                    config.Port = server_port;
                    server = new NetServer(config);
                    server.Start();
                    success = server.UPnP.ForwardPort(port_suggestion, "TheCheaps");
                    if (!success)
                        throw new Exception($"UPnP could not forward port {port_suggestion}");
                    break;
                }
                catch {
                    server_port++;
                }
            }
            if (!success)
                throw new Exception($"UPnP could not forward ANY port in the {port_suggestion}-{port_suggestion+10} range");
        }
        public void Start()
        {
            this.network = new GameNetwork();
            this.simulation = new GameSimulation();
            this.simulation.Start();
            this.input = new GameInput(simulation.model);
            server.Start();
            network.model.serverState.GamePhase = NetworkServerState.Phase.Lobby;
            Console.WriteLine($"Server for {config.AppIdentifier} starting... Listening on IP:{GetLocalIPAddress()} on port {server.Port}");
            this._started = true;
        }
        private DateTime lastTime;

        public void Tick()
        {
            var time = DateTime.UtcNow;
            var elapsedTime = time - lastTime;
            process_message();
            foreach (var connection in server.Connections.ToArray())
            {
                UpdateConnection(elapsedTime, connection);
            }            
            switch (network.model.serverState.GamePhase)
            {
                case NetworkServerState.Phase.Gameplay:
                    simulation.Step();
                    BroadCast(MessageType.SimulationState, simulation.GetState());
                    break;
                case NetworkServerState.Phase.Lobby:
                    BroadCast(MessageType.PeerState, network.GetState());
                    break;
                default:
                    break;
            }
            network.Update(elapsedTime);
            if (network.model.serverState.CountDown == 0)
            {
                StartMatch();
            }
            lastTime = time;
        }

        Dictionary<NetConnection, ConnectionInfo> temp_connection_infos = new Dictionary<NetConnection, ConnectionInfo>();
        private void UpdateConnection(TimeSpan elapsedTime, NetConnection connection)
        {
            if (!network.ConnectionInfos.TryGetValue(connection, out var info))
            {
                if (!temp_connection_infos.TryGetValue(connection, out info))
                {
                    info = new ConnectionInfo(-1);
                }
                info.Timeout -= elapsedTime.TotalSeconds;
                if (info.Timeout < 0)
                {
                    connection.Disconnect("Client did not handshake before timeout");
                }
            }
            //info.Heartbeat -= elapsedTime.TotalSeconds;
            //if (info.Heartbeat)
            //    SendMessage(connection, MessageType.HeartBeat,
            //info.Timeout -= elapsedTime.TotalSeconds;
            //if (info.Timeout < 0)
            //    connection.Disconnect("Client disconnected due to missing infos");
        }

        public void StartMatch()
        {
            network.model.serverState.GamePhase = NetworkServerState.Phase.Gameplay;
            BroadCast(MessageType.PeerState, network.GetState());
        }

        public void StartCountDown()
        {
            network.StartCountDown();
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
#if VERBOSE
                    System.Diagnostics.Debug.WriteLine($"{msg} received from server");
#endif
                    var type = (ClientMessageType)msg.ReadByte();
                    switch (type)
                    {
                        case ClientMessageType.ActionState:
                            var pl_index = network.GetPlayerIndex(msg.SenderConnection);
                            if(pl_index>=0)
                                input.SetActionState(msg.Deserialize<FrameActionState>(), pl_index);
                            break;
                        case ClientMessageType.NetworkOp:                            
                            var response = network.ProcessOp(msg.SenderConnection,msg.Deserialize<NetworkOp>());
                            SendMessage(msg.SenderConnection,MessageType.Response, response, NetDeliveryMethod.ReliableOrdered);
                            break;
                        default:
                            throw new Exception("Invalid message type");
                    }
                    break;
                default:
                    break;

            }
            server.Recycle(msg);
        }
        private void SendMessage(NetConnection destinationConnection, MessageType messageType, IBinarizable state, NetDeliveryMethod method = NetDeliveryMethod.UnreliableSequenced)
        {
            NetOutgoingMessage msg = server.CreateMessage();
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
            server.SendMessage(msg, destinationConnection, method);
        }

        private void BroadCast(MessageType messageType, IBinarizable state)
        {
            if (server.Connections.Count == 0)
                return;
            NetOutgoingMessage msg = server.CreateMessage();
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
            server.SendToAll(msg, NetDeliveryMethod.UnreliableSequenced);
        }

        public void Stop(string reason)
        {
            server.Shutdown($"Server shutting down: {reason}");
            //server.Stop();
        }

        public void Dispose()
        {
            simulation.Dispose();
            //server.Dispose();
        }

        public static string GetLocalIPAddress()
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

        public enum MessageType
        {
            PeerState,
            SimulationState,
            Op,
            Response,
        }
    }
}
