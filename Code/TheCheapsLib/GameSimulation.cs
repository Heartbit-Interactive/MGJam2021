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
            var entities = JsonConvert.DeserializeObject<List<Entity>>(jsontext);

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

            InitializeEntityIdentifiers(entities);
            foreach(var item in model.items)
            {
                item.tags.Add(Tags.CAN_TAKE_ITEM);
            }

            var jsontextrecipe = File.ReadAllText("Recipes.json");
            model.recipes = JsonConvert.DeserializeObject<List<Recipe>>(jsontextrecipe);

        }

        private void InitializeEntityIdentifiers(List<Entity> entities)
        {
            Entity.UniqueCounter = 0;
            foreach (var entity in entities)
            {
                entity.InitializeServer(0.02f);
                model.entities[entity.uniqueId] = entity;
            }
            foreach (var entity in model.player_entities)
                entity.InitializeServer(0.04f);
        }

        public void Step()
        {
            model.updated_entities.Clear();
            model.removed_entities.Clear();
            model.added_entities.Clear();
            var now = DateTime.UtcNow;
            var elapsedTime = last_time - now;
            foreach(var player in players)
            {
                player.Update(elapsedTime);
            }
            foreach (var entity in model.entities.Values)
                update_entity(elapsedTime,entity);
            foreach (var entity in model.added_entities)
                model.entities[entity.uniqueId] = entity;
            foreach (var id in model.removed_entities)
                model.entities.Remove(id);
            last_time = now;
        }

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
                model.updated_entities.Add(entity);
            }
            if (entity.speed == 0 && entity.removeable)
            {
                if (entity.life_time <= 0)
                {
                    entity.life_time = Settings.TIME_ON_THE_FLOOR;
                    RemEntity(entity);
                }
                else
                {
                    entity.life_time--;
                    model.updated_entities.Add(entity);
                }
            }
        }
        
        public SimulationState GetState()
        {
            return new SimulationState(model);
        }

        public SimulationDelta GetDelta()
        {
            return new SimulationDelta(model);
        }

        public void Stop()
        {
            
        }

        public void Dispose()
        {
        }

        public void SetState(SimulationState simulationState)
        {
            foreach (var freshEntity in simulationState.entities)
            {
                if (!model.entities.TryGetValue(freshEntity.uniqueId, out var existingEntity))
                {
                    model.entities[freshEntity.uniqueId] = freshEntity;
                    OnEntityAdded(freshEntity);
                }
                else
                {
                    existingEntity.CopyChanges(freshEntity);
                    freshEntity.Dispose();
                }
            }
            foreach (var id in simulationState.removed_entities)
                model.entities.Remove(id);

            for (int i = 0; i < simulationState.player_entities.Count; i++)
            {
                if (model.player_entities.Count <= i)
                    model.player_entities.Add(simulationState.player_entities[i]);
                else
                {
                    model.player_entities[i].CopyChanges(simulationState.player_entities[i]);
                    model.player_entities[i].update_collision_rect();
                    simulationState.player_entities[i].Dispose();
                }
            }

            OnStateUpdated();
        }
        public void ApplyDelta(SimulationDelta delta)
        {
            foreach (var freshEntity in delta.updated_entities)
            {
                if (!model.entities.TryGetValue(freshEntity.uniqueId, out var existingEntity))
                {
                    model.entities[freshEntity.uniqueId] = freshEntity;
                    OnEntityAdded(freshEntity);
                }
                else
                {
                    existingEntity.CopyChanges(freshEntity);
                    freshEntity.Dispose();
                }
            }
            foreach (var id in delta.removed_entities)
                model.entities.Remove(id);

            for (int i = 0; i < delta.player_entities.Count; i++)
            {
                if (model.player_entities.Count <= i)
                    model.player_entities.Add(delta.player_entities[i]);
                else
                {
                    model.player_entities[i].CopyChanges(delta.player_entities[i]);
                    model.player_entities[i].update_collision_rect();
                    delta.player_entities[i].Dispose();
                }
            }

            OnStateUpdated();
        }
        public event EventHandler StateUpdated;
        private void OnStateUpdated()
        {
            if (StateUpdated != null)
                StateUpdated.Invoke(this, null);
        }

        public event EventHandler EntityAdded;
        private void OnEntityAdded(Entity freshEntity)
        {
            if (EntityAdded != null)
                EntityAdded.Invoke(freshEntity, null);
        }
        internal void AddEntity(Entity entity)
        {
            model.added_entities.Add(entity);
        }
        internal void RemEntity(Entity entity)
        {
            model.removed_entities.Add(entity.uniqueId);
            //model.entities.Remove(entity.uniqueId);
        }
    }
}