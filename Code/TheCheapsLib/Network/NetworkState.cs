using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheCheapsLib
{
    public class NetworkState:State
    {
        public NetworkServerState serverState;
        public NetworkPlayerState[] players { get; private set; }
        public NetworkState()
        {

        }
        public NetworkState(NetworkModel model)
        {
            players = model.players;
            serverState = model.serverState;
        }
        public override void BinaryRead(BinaryReader br)
        {
            base.BinaryRead(br);

            serverState = new NetworkServerState();
            serverState.BinaryRead(br);

            players = new NetworkPlayerState[Settings.maxPlayers];
            for (int i = 0; i < Settings.maxPlayers; i++)
            {
                if (!br.ReadBoolean())
                    continue; //null player
                var entity = new NetworkPlayerState();
                entity.BinaryRead(br);
                players[i] = entity;
            }
        }
        public override void BinaryWrite(BinaryWriter bw)
        {
            base.BinaryWrite(bw);
            serverState.BinaryWrite(bw);
            foreach (var entity in players)
            {
                if (entity == null)
                {
                    bw.Write(false);
                    continue;
                }
                bw.Write(true);
                entity.BinaryWrite(bw);
            }
        }
    }
}
