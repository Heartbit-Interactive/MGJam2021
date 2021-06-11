
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private NetConnection connection;

        private GameInput input;
        public GameSimulation simulation;
        public GameNetwork network;

        public event EventHandler StateChanged;
        private void OnStateChanged()
        {
            if (StateChanged != null)
                StateChanged.Invoke(this, null);
        }
        public int PlayerIndex { get { return Array.FindIndex(network.model.players,x => x!=null && x.Unique == NetworkManager.ThisPeerUnique); } }


        public NetPeerStatus Status { get { return client == null ? NetPeerStatus.NotRunning : client.Status; } }

        public NetworkClient(IPAddress ip, int server_port, bool use_upnp)
        {
            int client_port = server_port+1;
            while (client_port < server_port + 10)
            {
                try
                {
                    config = new NetPeerConfiguration("TheCheaps");
                    config.EnableUPnP = use_upnp;
                    config.MaximumConnections = 4;
                    //config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                    config.Port = client_port;
                    client = new NetClient(config);
                    client.Start();
                    if (use_upnp)
                    {
                        bool success = client.UPnP.ForwardPort(client_port, "TheCheaps");
                        if (!success)
                            throw new Exception($"UPnP could not forward port {client_port}");
                    }
                    break;
                }
                catch
                {
                    client_port++;
                }
            }
            var string_ip = ip.ToString();
            connection = client.Connect(host: string_ip, port: server_port);

            this.simulation = new GameSimulation();
            this.network = new GameNetwork();
            this.input = new GameInput(simulation.model);
            StateChanged += SendHandshake;
        }

        private void SendHandshake(object sender, EventArgs e)
        {
            if (this.Status != NetPeerStatus.Running)
                return;
            if (this.client.ServerConnection == null)
                return;
            if (this.client.ServerConnection.Status != NetConnectionStatus.Connected)
                return;
            StateChanged -= SendHandshake;
            SendOp(NetworkOp.OpType.HandShake, NetworkManager.ThisPeerUnique, NetworkManager.ThisPlayerName);
        }
        public void Update(GameTime gameTime)
        {
            _stateChanged = false;
            var currentConnected = (client.ServerConnection == null ? false : client.ServerConnection.Status == NetConnectionStatus.Connected);
            if (Status != LastStatus || LastConnected != currentConnected)
            {
                _stateChanged = true;
            }
            ProcessIncomingMessages();
            StepResponseTimeouts();
            switch (network.model.serverState.GamePhase)
            {
                case NetworkServerState.Phase.Unset:
                    break;
                case NetworkServerState.Phase.Gameplay:
                    var actionState = this.input.getActionState();
                    if (actionState.Count > 0)
                    {
                        SendMessage(ClientMessageType.ActionState, actionState);
                    }
                    break;
                case NetworkServerState.Phase.Lobby:
                    break;
            }
            if (_stateChanged)
            {
                OnStateChanged();
            }
            LastStatus = Status;
            LastConnected = currentConnected;
        }

        private void StepResponseTimeouts()
        {
            var ticks = DateTime.UtcNow.Ticks;
            if (TimeoutsTicks.Count == 0)
                return;
            var first = TimeoutsTicks.FirstOrDefault();
            while (first.Key < ticks)
            {
                processResponse(new NetworkResponse() { type = NetworkResponse.Type.Timeout, OriginId = first.Value });
                if (TimeoutsTicks.Count == 0)
                    return;
                first = TimeoutsTicks.FirstOrDefault();
            }
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
                            case NetworkServer.MessageType.Response:
                                processResponse(msg.Deserialize<NetworkResponse>());
                                break;
                            case NetworkServer.MessageType.PeerState:
                                network.SetState(msg.Deserialize<NetworkState>());
                                _stateChanged = true;
                                break;
                            case NetworkServer.MessageType.SimulationState:
                                simulation.SetState(msg.Deserialize<SimulationState>());
                                break;
                            case NetworkServer.MessageType.SimulationDelta:
                                simulation.ApplyDelta(msg.Deserialize<SimulationDelta>());
                                if (delta_received == 0)
                                    delta_timer = DateTime.Now;
                                delta_received++;
                                if (delta_received > 500)
                                {
                                    System.Diagnostics.Debug.WriteLine($"{delta_received / (DateTime.Now - delta_timer).TotalSeconds:0.000} updates per second received");
                                    delta_received = 0;
                                }
                                break;
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
        int delta_received = 0;
        DateTime delta_timer;
        Dictionary<ulong, EventHandler<NetworkResponseEventArgs>> MessagesWaitingResponse = new Dictionary<ulong, EventHandler<NetworkResponseEventArgs>>(); 
        private void processResponse(NetworkResponse networkResponse)
        {
            EventHandler<NetworkResponseEventArgs> handler;
            if (!MessagesWaitingResponse.TryGetValue(networkResponse.OriginId, out handler))
                return;
            TimeoutsTicks.Remove(TimeoutsTicks.FirstOrDefault(x => x.Value == networkResponse.OriginId).Key);
            MessagesWaitingResponse.Remove(networkResponse.OriginId);
            handler.Invoke(this, new NetworkResponseEventArgs(networkResponse));
        }
        public class NetworkResponseEventArgs : EventArgs
        {
            public NetworkResponse response;
            public NetworkResponseEventArgs(NetworkResponse networkResponse)
            {
                this.response = networkResponse;
            }
        }
        internal void Dispose() 
        {
            StateChanged = null;
        }

        private void SendOp(NetworkOp.OpType type, params object[] pars)
        {
            SendMessage(ClientMessageType.NetworkOp, new NetworkOp(type, pars), NetDeliveryMethod.ReliableOrdered);
        }
        int defaultTimeoutS = 5;
        SortedList<long, ulong> TimeoutsTicks = new SortedList<long, ulong>();
        private NetPeerStatus LastStatus;
        private bool LastConnected;
        private bool _stateChanged;

        private void SendOp(NetworkOp.OpType type, EventHandler<NetworkResponseEventArgs> handler, params object[] pars)
        {
            var message = new NetworkOp(type, pars);
            MessagesWaitingResponse[message.MessageId] = handler;
            TimeoutsTicks.Add(DateTime.UtcNow.Ticks + TimeSpan.FromSeconds(defaultTimeoutS).Ticks + Rand.Generator.Next(10000), message.MessageId);
            SendMessage(ClientMessageType.NetworkOp,message, NetDeliveryMethod.ReliableOrdered);
        }
        private void SendMessage(ClientMessageType messageType, IBinarizable state, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.UnreliableSequenced)
        {
            NetOutgoingMessage msg = client.CreateMessage();
            byte[] array = null;
            using (var memstream = new MemoryStream())
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

        internal void SetReady(bool ready)
        {
            if (GetReady(PlayerIndex) != ready)
                SendMessage(ClientMessageType.NetworkOp, new NetworkOp(NetworkOp.OpType.SetReady, ready), NetDeliveryMethod.ReliableOrdered);
        }

        internal bool GetReady(int playerIndex)
        {
            if (playerIndex < 0)
                return false;
            return network.model.players[playerIndex].Ready;
        }

        internal void Disconnect()
        {
            SendOp(NetworkOp.OpType.Disconnect);
            client.Disconnect("DisconnectRequestedByUser");
        }
    }
}