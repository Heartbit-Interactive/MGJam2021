using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheCheapsLib
{
    public class State : IBinarizable
    {
        public virtual void BinaryRead(BinaryReader br)
        {
        }

        public virtual void BinaryWrite(BinaryWriter bw)
        {
        }
    }
}
