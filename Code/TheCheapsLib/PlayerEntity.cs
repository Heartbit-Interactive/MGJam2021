using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheCheapsLib
{
    public class PlayerEntity: Entity
    {
        public int index;
        public Inventory inventory;
        public int TIMER_DASH = 40;
        public int dash_timer_counter;

        private PlayerEntity():base() 
        {
            inventory = new Inventory();
        }
        public override void BinaryRead(BinaryReader br)
        {
            base.BinaryRead(br);            
        }
        public override void BinaryWrite(BinaryWriter bw)
        {
            base.BinaryWrite(bw);
        }
        public void update_timer_dash()
        {
            if (dash_timer_counter > 0)
                dash_timer_counter--;
        }
        internal override void CopyDelta(Entity other)
        {
            base.CopyDelta(other);
        }
        public override void Dispose()
        {
            if (this.disposed)
                return;
            Pool.Push(this);
            this.disposed = true;
        }
        private static Stack<PlayerEntity> Pool = new Stack<PlayerEntity>();
        public static new PlayerEntity Create()
        {
            if (Pool.Count == 0)
                return new PlayerEntity();
            return Pool.Pop();
        }
        public override void update_collision_rect()
        {
            base.update_collision_rect();
            for (int i = 0; i < inventory.entities.Count; i++)
            {
                var entity = inventory.entities[i];
                var offx = (int)(entity.posxy.X - entity.collisionrect.Width / 2);
                var offy = (int)(entity.posxy.Y - entity.collisionrect.Height);
                entity.collisionrect.X = offx;
                entity.collisionrect.Y = offy;
            }
        }

    }
}
