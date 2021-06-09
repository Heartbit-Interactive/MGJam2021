using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheCheapsLib
{
    public interface IBinarizable
    {
        void BinaryRead(BinaryReader br);
        void BinaryWrite(BinaryWriter br);
    }
}
