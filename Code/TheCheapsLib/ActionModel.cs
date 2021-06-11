using Microsoft.Xna.Framework;
using System;
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
        private ActionModel() { }
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
            Pool.Push(this);
            this.disposed = true;
        }
        private static Stack<ActionModel> Pool = new Stack<ActionModel>();
        public static ActionModel Create()
        {
            if (Pool.Count == 0)
                return new ActionModel();
            return Pool.Pop();
        }
    }
}
