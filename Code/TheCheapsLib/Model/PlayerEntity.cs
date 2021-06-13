using Microsoft.Extensions.ObjectPool;
using Microsoft.Xna.Framework.Content;
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
        public float dash_timer_counter;

        public int score = 0;
        private bool walking = false;
        public int step = 0;

        public PlayerEntity():base() 
        {
            inventory = new Inventory();

        }
        public override void BinaryRead(BinaryReader br)
        {
            base.BinaryRead(br);
            BinaryReadPlayer(br);
        }
        public override void BinaryReadDelta(BinaryReader br)
        {
            base.BinaryReadDelta(br);
            BinaryReadPlayer(br);
        }

        private void BinaryReadPlayer(BinaryReader br)
        {
            score = br.ReadInt32();
            var lcount = br.ReadInt32();
            inventory.temp_list_deltas = new List<int[]>();
            for (int i = 0; i < lcount; i++)
            {
                var list_id = br.ReadInt32();
                var list = new int[br.ReadInt32() + 1];
                list[0] = list_id;
                for (int j = 1; j < list.Length; j++)
                    list[j] = br.ReadInt32();
                inventory.temp_list_deltas.Add(list);
            }
        }

        public override void BinaryWrite(BinaryWriter bw)
        {
            base.BinaryWrite(bw);
            BinaryWritePlayer(bw);
        }
        public override void BinaryWriteDelta(BinaryWriter bw)
        {
            base.BinaryWriteDelta(bw);
            BinaryWritePlayer(bw);
        }
        public override void LoadTexture(ContentManager Content)
        {
            System.Diagnostics.Debug.WriteLine($"Player texture loaded {texture_path}");
            base.LoadTexture(Content);
        }
        private void BinaryWritePlayer(BinaryWriter bw)
        {
            bw.Write(score);
            bw.Write(this.inventory.list_recipes.Count);
            for (int i = 0; i < this.inventory.list_recipes.Count; i++)
            {
                var list = this.inventory.list_recipes[i];
                bw.Write(list.id);
                bw.Write(list.owned.Length);
                for (int j = 0; j < list.owned.Length; j++)
                    bw.Write(list.owned[j]);
            }
        }

        public void update_timer_dash(float elapsedTime)
        {
            if (dash_timer_counter > 0)
                dash_timer_counter -= elapsedTime;
        }
        internal override void CopyDelta(Entity other)
        {
            this.walking = other.posxy != this.posxy;
            base.CopyDelta(other);
            var othp = (PlayerEntity)other;
            this.score = othp.score;
            //this.inventory.temp_list_deltas = othp.inventory.temp_list_deltas;
        }
        public override void Dispose()
        {
            if (this.disposed)
                return;
            Pool.Return(this);
            this.disposed = true;
        }
        private static ObjectPool<PlayerEntity> Pool = ObjectPool.Create<PlayerEntity>();

        public static new PlayerEntity Create()
        {
            return Pool.Get();
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
