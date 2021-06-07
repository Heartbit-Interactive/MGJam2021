using Newtonsoft.Json;
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
            //SimulationModel.entities.Add(new Entity() { posxy = new Microsoft.Xna.Framework.Vector2(0,0.5f), texture_path="123"});
            //var text = JsonConvert.SerializeObject(SimulationModel.entities, Formatting.Indented);
            //File.WriteAllText("Level.json", text);
            var jsontext = File.ReadAllText("Level.json");
            SimulationModel.entities = JsonConvert.DeserializeObject<List<Entity>>(jsontext);
        }

        public static void Step()
        {
            var now = DateTime.Now;
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
    }
}