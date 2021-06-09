using System.IO;

namespace TheCheapsLib
{
    public class NetworkServerState : IBinarizable
    {
        public Phase GamePhase = Phase.Unset;

        public NetworkServerState()
        {
        }
        public enum Phase
        {
            Unset,
            Gameplay,
            Lobby
        }
        public void BinaryRead(BinaryReader br)
        {
            GamePhase = (Phase)br.ReadInt32();
        }

        public void BinaryWrite(BinaryWriter bw)
        {
            bw.Write((int)GamePhase);
        }
    }
}