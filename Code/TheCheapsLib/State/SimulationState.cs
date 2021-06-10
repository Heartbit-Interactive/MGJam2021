﻿using System;
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
        public SimulationState()
        {

        }
        public SimulationState(SimulationModel model)
        {
            entities = model.entities.Values.ToList() ;
            player_entities = model.player_entities;
        }
        public override void BinaryRead(BinaryReader br)
        {
            base.BinaryRead(br);
            var count = br.ReadInt32();
            entities = new List<Entity>(count);
            for (int i = 0; i < count; i++)
            {
                var entity = new Entity();
                entity.BinaryRead(br);
                entities.Add(entity);
            }
            count = br.ReadInt32();
            player_entities = new List<PlayerEntity>(count);
            for (int i = 0; i < count; i++)
            {
                var entity = new PlayerEntity();
                entity.BinaryRead(br);
                player_entities.Add(entity);
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
        }
    }
}
