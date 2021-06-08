using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace TheCheapsLib
{
    public class GameSimulation
    {
        public SimulationModel model;
        public GamePlayer[] players;
        public GameSimulation()
        {
            model = new SimulationModel();
            players = new GamePlayer[Settings.maxPlayers];
            for (int i = 0; i < players.Length; i++)
                players[i] = new GamePlayer(i, model);
                
        }
        public void Start()
        {
            //SimulationModel.entities = new List<Entity>();
            //SimulationModel.entities.Add(new Entity());
            //SimulationModel.entities.Add(new Entity() { posxy = new Microsoft.Xna.Framework.Vector2(0, 0.5f), texture_path = "123", direction = new Microsoft.Xna.Framework.Vector2(0, 1), });
            //var text = JsonConvert.SerializeObject(SimulationModel.entities, Formatting.Indented);
            //File.WriteAllText("Level.json", text);
            var jsontext = File.ReadAllText("Level.json");
            model.entities = JsonConvert.DeserializeObject<List<Entity>>(jsontext);

            //SimulationModel.player_entities = new List<PlayerEntity>();
            //SimulationModel.player_entities.Add(new PlayerEntity());
            //SimulationModel.player_entities.Add(new PlayerEntity() { posxy = new Microsoft.Xna.Framework.Vector2(0, 0.5f), texture_path = "123", direction = new Microsoft.Xna.Framework.Vector2(0, 1), });
            //var text = JsonConvert.SerializeObject(SimulationModel.player_entities, Formatting.Indented);
            //File.WriteAllText("Players.json", text);
            var jsontextplayer = File.ReadAllText("Players.json");
            model.player_entities = JsonConvert.DeserializeObject<List<PlayerEntity>>(jsontextplayer);
        }


        public void Step()
        {
            var now = DateTime.Now;
            foreach(var player in players)
            {
                player.update_input();
            }
            foreach (var entity in updateable_entities)
                update_entity(entity);
        }

        private void update_entity(Entity entity)
        {
        }

        private IEnumerable<Entity> updateable_entities { get { return model.entities; } }

        public byte[] GetSerializedState()
        {
            using (var memstream = new MemoryStream(128 * 1024))
            {
                var bw = new BinaryWriter(memstream);
                bw.Write(model.entities.Count);
                foreach (var entity in model.entities)
                    entity.binarywrite(bw);

                bw.Write(model.player_entities.Count);
                foreach (var entity in model.player_entities)
                    entity.binarywrite(bw);
                return memstream.ToArray();
            }
        }

        public void Stop()
        {
            
        }

        public void DeserializeState(byte[] content)
        {
            using (var memstream = new MemoryStream(content))
            {
                var br = new BinaryReader(memstream);
                var count = br.ReadInt32();
                model.entities = new List<Entity>(count);
                for (int i = 0; i < count; i++)
                {
                    var entity = new Entity();
                    entity.binaryread(br);
                    model.entities.Add(entity);
                }
                count = br.ReadInt32();
                model.player_entities = new List<PlayerEntity>(count);
                for (int i = 0; i < count; i++)
                {
                    var entity = new PlayerEntity();
                    entity.binaryread(br);
                    model.player_entities.Add(entity);
                }
            }
        }
    }
}