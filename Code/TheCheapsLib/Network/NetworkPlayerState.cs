using System.IO;

namespace TheCheapsLib
{
    public class NetworkPlayerState : NetworkMessageBase
    {
        public int Unique;
        public string Name = "Unnamed";
        public bool Ready;

        public NetworkPlayerState() { }
        public NetworkPlayerState(int uniq) { this.Unique = uniq; }
        public override void BinaryRead(BinaryReader br)
        {
            base.BinaryRead(br);
            Unique = br.ReadInt32();
            Name = br.ReadString();
            Ready = br.ReadBoolean();
        }
        public override void BinaryWrite(BinaryWriter bw)
        {
            base.BinaryWrite(bw);
            bw.Write(Unique);
            bw.Write(Name);
            bw.Write(Ready);
        }
        public override string ToString()
        {
            return $"{Name} ({(Ready?"Ready":"Not Ready")})";
        }
    }
}