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
            players = new GamePlayer[Settings.maxPlayers];
            for (int i = 0; i < players.Length; i++)
                players[i] = new GamePlayer(i, model);

            var jsontextitems = File.ReadAllText("Items.json");
            model.items = JsonConvert.DeserializeObject<List<Entity>>(jsontextitems);
        }


        public void Step()
        {
            var now = DateTime.Now;
            foreach(var player in players)
            {
                player.update();
            }
            foreach (var entity in updateable_entities)
                update_entity(entity);
        }

        private void update_entity(Entity entity)
        {
        }

        private IEnumerable<Entity> updateable_entities { get { return model.entities; } }

        public SimulationState GetState()
        {
            return new SimulationState(model);
        }

        public void Stop()
        {
            
        }

        public void Dispose()
        {
        }

        public void SetState(SimulationState simulationState)
        {
            model.entities = simulationState.entities;
            model.player_entities = simulationState.player_entities;

        }
    }
}