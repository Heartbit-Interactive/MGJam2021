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
            foreach (var player in model.player_entities)
            {
                player.inventory = new Inventory();
            }
            players = new GamePlayer[Settings.maxPlayers];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new GamePlayer(i, this);
            }

            var jsontextitems = File.ReadAllText("Items.json");
            model.items = JsonConvert.DeserializeObject<List<Entity>>(jsontextitems);

            InitializeEntityIdentifiers();
            foreach(var item in model.items)
            {
                item.tags.Add(Tags.CAN_TAKE_ITEM);
            }
        }

        private void InitializeEntityIdentifiers()
        {
            Entity.UniqueCounter = 0;
            foreach (var entity in model.entities)
                entity.InitializeServer(0.02f);
            foreach (var entity in model.player_entities)
                entity.InitializeServer(0.04f);
        }

        public void Step()
        {
            updated_entities.Clear();
            removed_entities.Clear();
            added_entities.Clear();
            var now = DateTime.UtcNow;
            var elapsedTime = last_time - now;
            foreach(var player in players)
            {
                player.Update(elapsedTime);
                updated_entities.Add(player.playerEntity);
            }
            for (int i = model.entities.Count-1; i>=0; i--)
                update_entity(elapsedTime,model.entities[i]);
            last_time = now;
        }
        public List<Entity> updated_entities = new List<Entity>();
        public List<Entity> removed_entities = new List<Entity>();
        public List<Entity> added_entities = new List<Entity>();
        private void update_entity(TimeSpan elapsedTime,Entity entity)
        {
            if(entity.speed > 0)
            {
                entity.posxy += entity.direction * entity.speed;
                entity.update_collision_rect();

                if (entity.posz>0)
                {
                    entity.posz -= Settings.fall_speed;
                    if (entity.posz <= 0)
                    {
                        entity.posz = 0;
                        entity.speed = 0;
                    }
                }
                updated_entities.Add(entity);
            }
            if (entity.speed == 0 && entity.removeable)
            {
                if (entity.life_time <= 0)
                {
                    entity.life_time = Settings.TIME_ON_THE_FLOOR;
                    removed_entities.Add(entity);
                    model.entities.Remove(entity);
                }
                else
                {
                    entity.life_time--;
                    updated_entities.Add(entity);
                }
            }
        }
        
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

        internal void AddEntity(Entity entity)
        {
            added_entities.Add(entity);
            model.entities.Add(entity);
        }
        internal void RemEntity(Entity entity)
        {
            removed_entities.Add(entity);
            model.entities.Remove(entity);
        }
    }
}