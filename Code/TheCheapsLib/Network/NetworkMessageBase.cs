using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheCheapsLib
{
    public abstract class NetworkMessageBase : IBinarizable
    {
        public static int PeerUniqueId = 0;
        static int CurrentId =0;
        public ulong MessageId { get; private set; }
        public NetworkMessageBase()
        {
            MessageId = (ulong)CurrentId + ((ulong)PeerUniqueId * (ulong)int.MaxValue);
            CurrentId++;
        }
        public virtual void BinaryRead(BinaryReader br)
        {
            MessageId = br.ReadUInt64();
        }

        public virtual void BinaryWrite(BinaryWriter bw)
        {
            bw.Write(MessageId);
        }
    }
}
