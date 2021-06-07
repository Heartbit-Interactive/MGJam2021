﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace TheCheapsLib
{
    public static class GameSimulation
    {
        public static void Start()
        {
            //SimulationModel.entities = new List<Entity>();
            //SimulationModel.entities.Add(new Entity());
            //SimulationModel.entities.Add(new Entity() { posxy = new Microsoft.Xna.Framework.Vector2(0, 0.5f), texture_path = "123", direction = new Microsoft.Xna.Framework.Vector2(0, 1), });
            //var text = JsonConvert.SerializeObject(SimulationModel.entities, Formatting.Indented);
            //File.WriteAllText("Level.json", text);
            var jsontext = File.ReadAllText("Level.json");
            SimulationModel.entities = JsonConvert.DeserializeObject<List<Entity>>(jsontext);
        }

        public static void Step()
        {
            var now = DateTime.Now;
            float speedframe = 1;
            if(SimulationModel.gamepads.Count>0)
                SimulationModel.entities[0].posxy = SimulationModel.entities[0].posxy + SimulationModel.gamepads[0].ThumbSticks.Left * speedframe;
            foreach (var entity in updateable_entities)
                update_entity(entity);
        }

        private static void update_entity(Entity entity)
        {
        }

        private static IEnumerable<Entity> updateable_entities { get { return SimulationModel.entities; } }

        public static byte[] GetSerializedState()
        {
            using (var memstream = new MemoryStream(128 * 1024))
            {
                var bw = new BinaryWriter(memstream);
                bw.Write(SimulationModel.entities.Count);
                foreach (var entity in SimulationModel.entities)
                    entity.binarywrite(bw);
                return memstream.ToArray();
            }
        }

        public static void Stop()
        {
            
        }

        public static void DeserializeState(byte[] content)
        {
            using (var memstream = new MemoryStream(content))
            {
                var br = new BinaryReader(memstream);
                var count = br.ReadInt32();
                SimulationModel.entities = new List<Entity>(count);
                for (int i = 0; i < count; i++)
                {
                    var entity = new Entity();
                    entity.binaryread(br);
                    SimulationModel.entities.Add(entity);
                }
            }
        }
    }
}