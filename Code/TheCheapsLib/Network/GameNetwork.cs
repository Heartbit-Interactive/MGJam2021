using Lidgren.Network;
using System;
using System.Collections.Generic;
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
        public NetworkState GetNetworkState()
        {
            return new NetworkState(model);
        }
        
        NetConnection[] peerConnections = new NetConnection[Settings.maxPlayers];
        Dictionary<NetConnection, int> connectionToPlayerMapping = new Dictionary<NetConnection, int>();
        private int addPeer(NetConnection conn,int uniq)
        {
            int pl_index = Array.IndexOf(peerConnections, null);
            if (pl_index >= 0)
            {
                Console.WriteLine($"Peer added at index {pl_index}");
                connectionToPlayerMapping[conn] = pl_index;
                peerConnections[pl_index] = conn;
                model.players[pl_index] = new NetworkPlayerState(uniq);
            }
            return pl_index;
        }

        public int GetPlayerIndex(NetConnection conn)
        {
            if (connectionToPlayerMapping.TryGetValue(conn, out int pl_index))
                return pl_index;
            return -1;
        }

        public NetworkResponse ProcessOp(NetConnection conn, NetworkOp networkOp)
        {
            var pindex = GetPlayerIndex(conn);
            switch (networkOp.Type)
            {
                case NetworkOp.OpType.SetReady:
                    if (pindex < 0)
                        return new NetworkResponse(NetworkResponse.Type.Error, "Player not joined") ;
                    model.players[pindex].Ready = (bool)networkOp.Parameters[0];
                    break;
                case NetworkOp.OpType.HandShake:
                    if (pindex < 0)
                        addPeer(conn, (int)networkOp.Parameters[0]);
                    pindex = GetPlayerIndex(conn);
                    if (pindex < 0)
                        throw new InvalidOperationException("Player could not join, maybe game is complete?");
                    model.players[pindex].Name = (string)networkOp.Parameters[1];
                    break;
                default:
                    break;
            }
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public void SetState(NetworkState networkState)
        {
            model.players = networkState.players;
            model.serverState = networkState.serverState;
        }

    }
}
