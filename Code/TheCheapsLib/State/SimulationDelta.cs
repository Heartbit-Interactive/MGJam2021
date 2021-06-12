using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TheCheapsLib
{
    public class SimulationDelta:State
    {
        public int timer;
        public HashSet<Entity> updated_entities   ;
        public List<PlayerEntity> player_entities;
        public List<Entity> added_entities        ;
        public List<int> removed_entities;
        public List<int> broadcasting_news;
        public SimulationDelta()
        {

        }
        public override void BinaryRead(BinaryReader br)
        {
            base.BinaryRead(br);
            var count = br.ReadInt32();
            if (updated_entities == null)
                updated_entities = new HashSet<Entity>();
            else
                updated_entities.Clear();
            for (int i = 0; i < count; i++)
            {
                var entity = Entity.Create();
                entity.BinaryReadDelta(br);
                updated_entities.Add(entity);
            }
            count = br.ReadInt32();
            player_entities = new List<PlayerEntity>(count);
            for (int i = 0; i < count; i++)
            {
                var entity = PlayerEntity.Create();
                entity.index = i;
                entity.BinaryReadDelta(br);
                player_entities.Add(entity);
            }
            count = br.ReadInt32();
            if (added_entities == null)
                added_entities = new List<Entity>();
            else
                added_entities.Clear();
            for (int i = 0; i < count; i++)
            {
                var entity = Entity.Create();
                entity.BinaryRead(br);
                added_entities.Add(entity);
            }
            count = br.ReadInt32();
            removed_entities = new List<int>(count);
            for (int i = 0; i < count; i++)
                removed_entities.Add(br.ReadInt32());

            count = br.ReadInt32();
            broadcasting_news = new List<int>(count);
            for (int i = 0; i < count; i++)
                broadcasting_news.Add(br.ReadInt32());
            timer = br.ReadInt32();
        }
        public override void BinaryWrite(BinaryWriter bw)
        {
            base.BinaryWrite(bw);
            bw.Write(updated_entities.Count);
            foreach (var entity in updated_entities)
                entity.BinaryWriteDelta(bw);

            bw.Write(player_entities.Count);
            foreach (var entity in player_entities)
                entity.BinaryWriteDelta(bw);

            bw.Write(added_entities.Count);
            foreach (var entity in added_entities)
                entity.BinaryWrite(bw);
            bw.Write(removed_entities.Count);
            foreach (var id in removed_entities)
                bw.Write(id);
            bw.Write(broadcasting_news.Count);
            foreach (var id in broadcasting_news)
                bw.Write(id);
            bw.Write(timer);
        }
    }
}
