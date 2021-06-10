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
        private DateTime last_time;

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
            foreach(var player in model.player_entities)
            {
                player.inventory = new Inventory();
            }
            players = new GamePlayer[Settings.maxPlayers];
            for (int i = 0; i < players.Length; i++)
                players[i] = new GamePlayer(i, model);

            var jsontextitems = File.ReadAllText("Items.json");
            model.items = JsonConvert.DeserializeObject<List<Entity>>(jsontextitems);
         
        }


        public void Step()
        {
            var now = DateTime.UtcNow;
            var elapsedTime = last_time - now;
            foreach(var player in players)
            {
                player.Update(elapsedTime);
            }
            for (int i = model.entities.Count-1; i>=0; i--)
                update_entity(elapsedTime,model.entities[i]);
            last_time = now;
        }

        private void update_entity(TimeSpan elapsedTime,Entity entity)
        {
            if(entity.speed > 0)
            {
                entity.posxy += entity.direction * entity.speed;
                if(entity.posz>0)
                {
                    entity.posz -= Settings.fall_speed;
                    if (entity.posz <= 0)
                    {
                        entity.posz = 0;
                        entity.speed = 0;
                    }
                }

            }
            if (entity.speed == 0 && entity.removeable)
            {
                if (entity.life_time <= 0)
                {
                    entity.life_time = Settings.TIME_ON_THE_FLOOR;
                    model.entities.Remove(entity);
                }
                else
                    entity.life_time--;
            }

            
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