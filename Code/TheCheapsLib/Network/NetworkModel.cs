using System.Collections.Generic;

namespace TheCheapsLib
{
    public class NetworkModel
    {
        public NetworkPlayerState[] players;
        public NetworkServerState serverState;
        public NetworkModel()
        {
            players = new NetworkPlayerState[Settings.maxPlayers];
            serverState = new NetworkServerState();
        }
    }
}