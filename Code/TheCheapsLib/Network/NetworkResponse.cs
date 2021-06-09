using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheCheapsLib
{
    public class NetworkResponse : NetworkMessageBase
    {
        public ulong OriginId;
        public Type type;
        public string message = "";

        public NetworkResponse() { }
        public NetworkResponse(NetworkMessageBase mess,Type type, string message = "")
        {
            this.OriginId = mess.MessageId;
            this.type = type;
            this.message = message;
        }

        public enum Type
        {
            Error,
            Ok,
            Timeout
        }
        public override void BinaryRead(BinaryReader br)
        {
            base.BinaryRead(br);
            OriginId = br.ReadUInt64();
            type = (Type)br.ReadInt32();
            message = br.ReadString();
        }

        public override void BinaryWrite(BinaryWriter bw)
        {
            base.BinaryWrite(bw);
            bw.Write(OriginId);
            bw.Write((int)type);
            bw.Write(message);
        }
    }
}
