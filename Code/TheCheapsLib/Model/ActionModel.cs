﻿using Microsoft.Extensions.ObjectPool;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheCheapsLib
{
    public class ActionModel
    {
        public enum Type { Move, Throw, Dash, Interact }
        public Type type;
        public Vector2 direction;
        public ActionModel() { }
        public void binary_write(BinaryWriter bw)
        {
            bw.Write((int)type);
            bw.Write(direction.X);
            bw.Write(direction.Y);
        }
        public void binary_read(BinaryReader br)
        {
            type = (Type)br.ReadInt32();
            direction.X = br.ReadSingle();
            direction.Y = br.ReadSingle();
        }

        protected bool disposed;
        public virtual void Dispose()
        {
            if (this.disposed)
                return;
            Pool.Return(this);
            this.disposed = true;
        }
        private static ObjectPool<ActionModel> Pool = ObjectPool.Create<ActionModel>();
        public static ActionModel Create()
        {
            return Pool.Get();
        }
    }
}
