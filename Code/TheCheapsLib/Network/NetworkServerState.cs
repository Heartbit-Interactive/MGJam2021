using System.IO;

namespace TheCheapsLib
{
    public class NetworkServerState : NetworkMessageBase
    {
        public Phase GamePhase = Phase.Unset;
        internal bool ReadyToStart;

        public NetworkServerState()
        {
        }
        public enum Phase
        {
            Unset,
            Gameplay,
            Lobby
        }
        public override void BinaryRead(BinaryReader br)
        {
            base.BinaryRead(br);
            GamePhase = (Phase)br.ReadInt32();
            ReadyToStart = br.ReadBoolean();
        }

        public override void BinaryWrite(BinaryWriter bw)
        {
            base.BinaryWrite(bw);
            bw.Write((int)GamePhase);
            bw.Write(ReadyToStart);
        }
    }
}