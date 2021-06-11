using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        /// <summary>
        /// This is run on the server
        /// </summary>
        public void StartServer()
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
            StartCommon();

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

            InitializeEntityIdentifiers(entities);

        }
        /// <summary>
        /// This is run on the client and on the server
        /// </summary>
        public void StartCommon()
        {
            var jsontextitems = File.ReadAllText("Items.json");
            model.items = JsonConvert.DeserializeObject<List<Entity>>(jsontextitems);

            var jsontextrecipe = File.ReadAllText("Recipes.json");
            model.recipes = JsonConvert.DeserializeObject<List<Recipe>>(jsontextrecipe);
            for (int i = 0; i < model.recipes.Count; i++)
                model.recipes[i].id = i;
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
            var elapsedSecondsf = (float)(now- last_time).TotalSeconds;
            foreach (var player in players)
            {
                player.Update(elapsedSecondsf);
            }
            foreach (var entity in model.entities.Values)
                update_entity(elapsedSecondsf, entity);
            foreach (var entity in model.added_entities)
                model.entities.Add(entity.uniqueId,entity);
            foreach (var id in model.removed_entities)
                model.entities.Remove(id);
            last_time = now;
        }

        private void update_entity(float elapsedTimeSeconds,Entity entity)
        {
            if(entity.speed > 0)
            {
                entity.posxy += entity.direction * entity.speed * elapsedTimeSeconds;
                entity.update_collision_rect();
                if (entity.posz>0)
                {
                    entity.posz -= Settings.fall_speed* elapsedTimeSeconds;
                    if (entity.posz <= 0)
                    {
                        entity.posz = 0;
                        entity.speed = 0;
                    }
                }
                model.updated_entities.Add(entity);
            }
            if (entity.tags.Contains(Tags.CAN_TAKE_ITEM))
            {
                //Se colpisce un player
                foreach (var player in players)
                {
                    if (player.playerEntity.inventory.entities.Contains(entity))
                        continue;
                    if (Rectangle.Intersect(player.playerEntity.collisionrect, entity.collisionrect).Width != 0)
                    {
                        if (player.launched_items.Contains(entity))
                            continue;
                        entity.posz = 0;
                        entity.speed = 0;
                        player.stun_player();
                    }
                    else if (player.launched_items.Contains(entity))
                        player.launched_items.Remove(entity);
                }
                //Controllo colpisce una base
                foreach (var en in model.entities)
                {
                    if (en.Value.tags.Contains(Tags.BASE))
                    {
                        if (Rectangle.Intersect(en.Value.collisionrect, entity.collisionrect).Width != 0)
                        {
                            var a = Int32.Parse(en.Value.tags.Where(x => x != Tags.BASE).FirstOrDefault());
                            var player = players[a];
                            //oggetto aggiunto alle recipe in questa funzione
                            if (player.if_player_needs_ingredient_add(entity.name))
                            {
                                entity.removeable = true;
                                entity.speed = 0;
                                entity.life_time = 0;
                            }
                            else
                            {
                                //oggetto venduto si ottengono punti
                                player.playerEntity.score += Settings.scoreSellItem;
                            }
                        }
                    }
                }
            }
            if (entity.speed == 0 && entity.removeable)
            {
                if (entity.life_time <= 0)
                {
                    entity.life_time = Settings.TimeOnTheFloor;
                    RemEntity(entity);
                }
                else
                {
                    entity.life_time -= elapsedTimeSeconds;
                    model.updated_entities.Add(entity);
                }
            }
        }
        
        public SimulationState GetState()
        {
            var state = new SimulationState();
            state.entities = model.entities.Values.ToList();
            state.player_entities = model.player_entities;
            state.added_entities = model.added_entities.Select(x => x.uniqueId).ToList();
            state.removed_entities = model.removed_entities;
            return state;
        }

        public SimulationDelta GetDelta()
        {
            var delta = new SimulationDelta();
            delta.player_entities = model.player_entities;
            delta.added_entities = model.added_entities;
            delta.removed_entities = model.removed_entities;
            delta.updated_entities = model.updated_entities;
            return delta;
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
                    existingEntity.CopyDelta(freshEntity);
                    freshEntity.Dispose();
                }
            }
            foreach (var id in simulationState.removed_entities)
                model.entities.Remove(id);

            for (int i = 0; i < simulationState.player_entities.Count; i++)
            {
                if (model.player_entities.Count <= i)
                {
                    model.player_entities.Add(simulationState.player_entities[i]);
                    applyRecipesToPlayer(simulationState.player_entities[i].inventory.temp_list_deltas, i);
                }
                else
                {
                    model.player_entities[i].CopyDelta(simulationState.player_entities[i]);
                    model.player_entities[i].update_collision_rect();
                    applyRecipesToPlayer(simulationState.player_entities[i].inventory.temp_list_deltas, i);
                    simulationState.player_entities[i].Dispose();
                }
            }

            OnStateUpdated();
        }

        private void applyRecipesToPlayer(List<int[]> list_deltas, int i)
        {
            var inv = model.player_entities[i].inventory;            
            //Converte i dati delle liste da liste di interi a liste 
            if (list_deltas != null && list_deltas.Count > 0)
                foreach (var list_delta in list_deltas)
                {
                    var recipe = model.recipes[list_delta[0]];
                    if (!inv.list_recipes.Contains(recipe))
                        inv.list_recipes.Add(recipe);
                    recipe.owned = new int[list_delta.Length - 1];
                    Array.Copy(list_delta, 1, recipe.owned, 0, list_delta.Length - 1);
                }
        }

        public void ApplyDelta(SimulationDelta delta)
        {
            foreach (var added_entity in delta.added_entities)
            {
                model.entities.Add(added_entity.uniqueId, added_entity);
                OnEntityAdded(added_entity);
            }
            foreach (var freshEntity in delta.updated_entities)
            {
                if (!model.entities.TryGetValue(freshEntity.uniqueId, out var existingEntity))
                {
                    throw new Exception("invalid id on update");
                }
                else
                {
                    existingEntity.CopyDelta(freshEntity);
                    freshEntity.Dispose();
                }
            }
            foreach (var id in delta.removed_entities)
                model.entities.Remove(id);

            for (int i = 0; i < delta.player_entities.Count; i++)
            {
                if (model.player_entities.Count <= i)
                {
                    model.player_entities.Add(delta.player_entities[i]);
                    applyRecipesToPlayer(delta.player_entities[i].inventory.temp_list_deltas, i);
                }
                else
                {
                    model.player_entities[i].CopyDelta(delta.player_entities[i]);
                    model.player_entities[i].update_collision_rect();
                    applyRecipesToPlayer(delta.player_entities[i].inventory.temp_list_deltas, i);
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