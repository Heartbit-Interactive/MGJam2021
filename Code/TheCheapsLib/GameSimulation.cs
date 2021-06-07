using System;
using System.Collections.Generic;
using System.IO;

namespace TheCheapsLib
{
    public static class GameSimulation
    {
        public static void Start()
        {

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
    }
}