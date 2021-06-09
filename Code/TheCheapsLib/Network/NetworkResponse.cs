using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheCheapsLib
{
    public class NetworkResponse : IBinarizable
    {
        public Type type;
        public enum Type { }
        public void BinaryRead(BinaryReader br)
        {
            type = (Type)br.ReadInt32();
        }

        public void BinaryWrite(BinaryWriter bw)
        {
            bw.Write((int)type);
        }
    }
}
