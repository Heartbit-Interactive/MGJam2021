using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheCheapsLib
{
    public class NetworkOp : IBinarizable
    {
        public OpType Type;
        public object[] Parameters;
        private bool ready;
        public NetworkOp()
        {

        }
        public NetworkOp(OpType Type, params object[] parameters)
        {
            this.Type = Type;
            this.Parameters = parameters;
        }

        public void BinaryRead(BinaryReader br)
        {
            Type = (OpType)br.ReadInt32();
            switch (Type)
            {
                case OpType.SetReady:
                    Parameters = new object[] { br.ReadBoolean() };
                    break;
                case OpType.HandShake:
                    Parameters = new object[] { br.ReadInt32(), br.ReadString() };
                    break;
                default:
                    break;
            }
        }

        public void BinaryWrite(BinaryWriter bw)
        {
            bw.Write((int)Type);
            switch (Type)
            {
                case OpType.SetReady:
                    bw.Write((bool)Parameters[0]);
                    break;
                case OpType.HandShake:
                    bw.Write((int)Parameters[0]);
                    bw.Write((string)Parameters[1]);
                    break;
                default:
                    break;
            }
        }

        public enum OpType {
            SetReady,
            HandShake,
            Disconnect
        }
        
    }
}
