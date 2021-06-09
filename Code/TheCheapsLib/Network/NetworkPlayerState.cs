using System.IO;

namespace TheCheapsLib
{
    public class NetworkPlayerState : IBinarizable
    {
        public int Unique;
        public string Name = "Unnamed";
        public bool Ready;

        public NetworkPlayerState() { }
        public NetworkPlayerState(int uniq) { this.Unique = uniq; }
        public void BinaryRead(BinaryReader br)
        {
            Unique = br.ReadInt32();
            Name = br.ReadString();
            Ready = br.ReadBoolean();
        }
        public void BinaryWrite(BinaryWriter bw)
        {
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