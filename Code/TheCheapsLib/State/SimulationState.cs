using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TheCheapsLib
{
    public class SimulationState:State
    {
        public List<Entity> entities { get; private set; }
        public List<PlayerEntity> player_entities { get; private set; }
        public List<int> added_entities { get; private set; }
        public List<int> removed_entities { get; private set; }
        public SimulationState()
        {

        }
        public SimulationState(SimulationModel model)
        {
            entities = model.entities.Values.ToList() ;
            player_entities = model.player_entities;
            added_entities = model.added_entities.Select(x=>x.uniqueId).ToList();
            removed_entities = model.removed_entities;
        }
        public override void BinaryRead(BinaryReader br)
        {
            base.BinaryRead(br);
            var count = br.ReadInt32();
            entities = new List<Entity>(count);
            for (int i = 0; i < count; i++)
            {
                var entity = Entity.Create();
                entity.BinaryRead(br);
                entities.Add(entity);
            }
            count = br.ReadInt32();
            player_entities = new List<PlayerEntity>(count);
            for (int i = 0; i < count; i++)
            {
                var entity = PlayerEntity.Create();
                entity.BinaryRead(br);
                player_entities.Add(entity);
            }
            count = br.ReadInt32();
            added_entities = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                added_entities.Add(br.ReadInt32());
            }
            count = br.ReadInt32();
            removed_entities = new List<int>(count);
            for (int i = 0; i < count; i++)
            {
                removed_entities.Add(br.ReadInt32());
            }

        }
        public override void BinaryWrite(BinaryWriter bw)
        {
            base.BinaryWrite(bw);
            bw.Write(entities.Count);
            foreach (var entity in entities)
                entity.BinaryWrite(bw);

            bw.Write(player_entities.Count);
            foreach (var entity in player_entities)
                entity.BinaryWrite(bw);

            bw.Write(added_entities.Count);
            foreach (var id in added_entities)
                bw.Write(id);
            bw.Write(removed_entities.Count);
            foreach (var id in removed_entities)
                bw.Write(id);
        }
    }
}
