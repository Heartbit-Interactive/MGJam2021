using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheCheapsLib
{
    public class GameNetwork
    {
        public NetworkModel model;
        public GameNetwork()
        {
            model = new NetworkModel();
        }
        public NetworkState GetState()
        {
            return new NetworkState(model);
        }
        
        public NetConnection[] peerConnections = new NetConnection[Settings.maxPlayers];
        public Dictionary<NetConnection, ConnectionInfo> ConnectionInfos = new Dictionary<NetConnection, ConnectionInfo>();
        private int addPeer(NetConnection conn,int uniq)
        {
            int pl_index = Array.IndexOf(peerConnections, null);
            if (pl_index >= 0)
            {
                Console.WriteLine($"Peer added at index {pl_index}");
                peerConnections[pl_index] = conn;
                model.players[pl_index] = new NetworkPlayerState(uniq);
                ConnectionInfos[conn] = new ConnectionInfo(pl_index);
            }
            return pl_index;
        }

        public int GetPlayerIndex(NetConnection conn)
        {
            if (ConnectionInfos.TryGetValue(conn, out var info))
                return info.PlayerIndex;
            return -1;
        }

        public NetworkResponse ProcessOp(NetConnection conn, NetworkOp networkOp)
        {
            var pindex = GetPlayerIndex(conn);
            if (pindex < 0 && networkOp.Type != NetworkOp.OpType.HandShake)
                return new NetworkResponse(networkOp, NetworkResponse.Type.Error, "Player not joined");
            switch (networkOp.Type)
            {
                case NetworkOp.OpType.SetReady:
                    model.players[pindex].Ready = (bool)networkOp.Parameters[0];
                    break;
                case NetworkOp.OpType.Disconnect:
                    model.players[pindex] = null;
                    break;
                case NetworkOp.OpType.HandShake:
                    if (pindex < 0)
                        addPeer(conn, (int)networkOp.Parameters[0]);
                    pindex = GetPlayerIndex(conn);
                    if (pindex < 0)
                        return new NetworkResponse(networkOp, NetworkResponse.Type.Error, "Player could not join, maybe game is complete?");
                    model.players[pindex].Name = (string)networkOp.Parameters[1];
                    break;
                default:
                    break;
            }
            return new NetworkResponse(networkOp,NetworkResponse.Type.Ok);
        }

        public void Update()
        {
            for (int i = peerConnections.Length - 1; i >= 0; i--)
            {
                if(peerConnections[i] != null)
                if (peerConnections[i].Status == NetConnectionStatus.Disconnected)
                {
                    removePeer(i);
                }
            }
            if (model.serverState.GamePhase == NetworkServerState.Phase.Lobby)
                model.serverState.ReadyToStart = model.players.All(x => x == null || x.Ready);
        }



        private void removePeer(int i)
        {
            ConnectionInfos.Remove(peerConnections[i]);
            peerConnections[i] = null;
            model.players[i] = null;
        }

        public void SetState(NetworkState networkState)
        {
            model.players = networkState.players;
            model.serverState = networkState.serverState;
        }

    }
}
