using Microsoft.Extensions.ObjectPool;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheCheapsLib
{
    public class S2CActionModel : IBinarizable
    {
        public enum Type { SE,Shake,Popup }
        public Type type;
        public int[] parameters;
        public S2CActionModel() { }
        public void BinaryWrite(BinaryWriter bw)
        {
            bw.Write((int)type);
            bw.Write(parameters.Length);
            for (int i = 0; i < parameters.Length; i++)
                bw.Write(parameters[i]);
        }
        public void BinaryRead(BinaryReader br)
        {
            type = (Type)br.ReadInt32();
            parameters = new int[br.ReadInt32()];
            for (int i = 0; i < parameters.Length; i++)
                parameters[i] = br.ReadInt32();
        }

        protected bool disposed;
        public virtual void Dispose()
        {
            if (this.disposed)
                return;
            Pool.Return(this);
            this.disposed = true;
        }
        private static ObjectPool<S2CActionModel> Pool = ObjectPool.Create<S2CActionModel>();
        public static S2CActionModel Create()
        {
            return Pool.Get();
        }
    }
}
