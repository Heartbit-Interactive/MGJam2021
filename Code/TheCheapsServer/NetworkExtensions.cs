using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheCheapsLib
{
    public static class NetworkExtensions
    {
        public static T Deserialize<T>(this NetIncomingMessage msg) where T : IBinarizable, new()
        {
            var size = msg.ReadInt32();
            var content = msg.ReadBytes(size);
            T result = new T();
            using (var memstream = new MemoryStream(content))
            {
                var br = new BinaryReader(memstream);
                result.BinaryRead(br);
            }
            return result;
        }

    }
}
