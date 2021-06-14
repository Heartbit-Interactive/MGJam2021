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
        private DateTime last_time = DateTime.MinValue;
        private List<int> currentle_broadcasting;
        private float currentle_broadcasting_timer;
        public int run;
        public bool ready;

        public GameSimulation()
        {
            model = new SimulationModel();
        }
        /// <summary>
        /// This is run on the server
        /// </summary>
        public void StartServer()
        {
            var jsontext = File.ReadAllText("Level.json");
            var entities = JsonConvert.DeserializeObject<List<Entity>>(jsontext);

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
                model.player_entities[i].index = i;
            }
            foreach (var recipe in model.recipes)
            {
                if (recipe.character_associated != null)
                    players.FirstOrDefault(x => x.playerEntity.name == recipe.character_associated).recipes_associated.Add(recipe);
            }
            for (int i = 0; i < players.Length; i++)
            {
                players[i].generate_new_recipe(0);
            }
                InitializeEntityIdentifiers(entities);

        }
        /// <summary>
        /// This is run on the client and on the server
        /// </summary>
        public void StartCommon()
        {
            ready = true;
            var jsontextitems = File.ReadAllText("Items.json");
            model.items = JsonConvert.DeserializeObject<List<Entity>>(jsontextitems);

            var jsontextrecipe = File.ReadAllText("Recipes.json");
            model.recipes = JsonConvert.DeserializeObject<List<Recipe>>(jsontextrecipe);
            for (int i = 1; i < model.recipes.Count; i++)
            {
                model.recipes[i].id = i;
            }
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

        public void StepClient()
        {
            var now = DateTime.UtcNow;
            if (last_time == DateTime.MinValue)
                last_time = now;
            var elapsedSecondsf = (float)(now - last_time).TotalSeconds;
            foreach (var entity in model.entities.Values)
                entity.Update(elapsedSecondsf) ;
            last_time = now;
        }
        public void Step()
        {
            model.updated_entities.Clear();
            model.removed_entities.Clear();
            model.added_entities.Clear();
            model.special_commands.Clear();
            var now = DateTime.UtcNow;
            if (last_time == DateTime.MinValue)
                last_time = now;
            var elapsedSecondsf = (float)(now- last_time).TotalSeconds;
            if (float.IsNaN(elapsedSecondsf))
                System.Diagnostics.Debugger.Break();
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
            model.timer -= elapsedSecondsf;
            if (model.broadcasting_news.Count > 0)
            {
                if (currentle_broadcasting != model.broadcasting_news)
                {
                    currentle_broadcasting = model.broadcasting_news;
                    currentle_broadcasting_timer = (float)Settings.BroadCastDuration;
                }
                currentle_broadcasting_timer -= elapsedSecondsf;
                if (currentle_broadcasting_timer < 0)
                {
                    model.broadcasting_news.Clear();
                    currentle_broadcasting.Clear();
                    currentle_broadcasting = null;
                }
            }
            if (model.timer <= 0)
                OnGameEndedServer();
            last_time = now;
        }

        public event EventHandler GameEndedServer;
        private void OnGameEndedServer()
        {
            if (GameEndedServer != null)
                GameEndedServer.Invoke(this, null);
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
            if (entity.tags.Contains(Tags.CAN_TAKE_ITEM) && entity.posz>0)
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
                        entity.posxy -= entity.direction * 24;
                        entity.update_collision_rect();
                        entity.posz = 0;
                        entity.speed = 0;
                        model.updated_entities.Add(entity);
                        player.stun_player();
                        model.special_commands.Add(new S2CActionModel() { type = S2CActionModel.Type.SE, parameters = new int[] { player.id, (int)SEType.Stun } });

                    }
                    else if (player.launched_items.Contains(entity))
                        player.launched_items.Remove(entity);
                }
                //Controllo colpisce una base
                foreach (var en in model.entities.Values)
                {
                    if (en.tags.Contains(Tags.BASE))
                    {
                        if (Rectangle.Intersect(en.collisionrect, entity.collisionrect).Width != 0)
                        {
                            var a = Int32.Parse(en.tags.Where(x => x != Tags.BASE).FirstOrDefault());
                            var player = players[a];
                            var score_pre = players[a].playerEntity.score;
                            //oggetto aggiunto alle recipe in questa funzione
                            if (player.if_player_needs_ingredient_add(entity.name))
                            {
                                if (score_pre != players[a].playerEntity.score)
                                {
                                    model.special_commands.Add(new S2CActionModel() { type = S2CActionModel.Type.Popup, parameters = new int[] { en.uniqueId, players[a].playerEntity.score- score_pre} });
                                }
                                entity.removeable = true;                                
                                model.special_commands.Add(new S2CActionModel() { type = S2CActionModel.Type.SE, parameters = new int[] { a, (int)SEType.Recipe } });
                            }
                            else
                            {
                                //oggetto venduto si ottengono punti
                                player.playerEntity.score += Settings.scoreSellItem;
                                model.special_commands.Add(new S2CActionModel() { type = S2CActionModel.Type.SE, parameters = new int[] { a, (int)SEType.Sold } });
                                model.special_commands.Add(new S2CActionModel() { type = S2CActionModel.Type.Popup, parameters = new int[] { en.uniqueId, Settings.scoreSellItem } });
                            }
                            entity.speed = 0;
                            entity.life_time = 0;
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

        public void Reset()
        {
            model.Clear();
            last_time = DateTime.MinValue;
            ready = false;
        }

        public SimulationState GetState()
        {
            var state = new SimulationState();
            state.entities = model.entities.Values.ToList();
            state.player_entities = model.player_entities;
            state.added_entities = model.added_entities.Select(x => x.uniqueId).ToList();
            state.removed_entities = model.removed_entities;
            state.broadcasting_news = model.broadcasting_news;
            state.timer = (int)model.timer;
            return state;
        }

        public SimulationDelta GetDelta()
        {
            var delta = new SimulationDelta();
            delta.player_entities = model.player_entities;
            delta.added_entities = model.added_entities;
            delta.removed_entities = model.removed_entities;
            delta.updated_entities = model.updated_entities;
            delta.broadcasting_news = model.broadcasting_news;
            delta.special_commands = model.special_commands;
            delta.timer = (int)model.timer;
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
            model.timer = simulationState.timer;
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
                    OnEntityAdded(simulationState.player_entities[i]);
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
                    {
                        inv.list_recipes.Clear();
                        inv.list_recipes.Add(recipe);
                    }
                    recipe.owned = new int[list_delta.Length - 1];
                    Array.Copy(list_delta, 1, recipe.owned, 0, list_delta.Length - 1);
                }
        }

        public void ApplyDelta(SimulationDelta delta)
        {
            model.timer = delta.timer;
            model.broadcasting_news = delta.broadcasting_news;
            if (delta.special_commands == null)
                delta.special_commands = new List<S2CActionModel>();
            foreach (var s2cCommand in delta.special_commands)
                processS2CCommand(s2cCommand);
            foreach (var added_entity in delta.added_entities)
            {                
                model.entities.Add(added_entity.uniqueId, added_entity);
                OnEntityAdded(added_entity);
            }
            foreach (var freshEntity in delta.updated_entities)
            {
                if (!model.entities.TryGetValue(freshEntity.uniqueId, out var existingEntity))
                {
                    //throw new Exception("invalid id on update");
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

        private void processS2CCommand(S2CActionModel s2cCommand)
        {
            switch (s2cCommand.type)
            {
                case S2CActionModel.Type.SE:
                    {
                        SoundManager.PlaySE((SEType)s2cCommand.parameters[1]);
                    }
                    break;
                case S2CActionModel.Type.Shake:
                    {
                        if (model.entities.TryGetValue(s2cCommand.parameters[0], out var ent))
                        {
                            ent.StartShake(s2cCommand.parameters[1], s2cCommand.parameters[2], s2cCommand.parameters[3]);
                        }
                    }
                    break;
                case S2CActionModel.Type.Popup:
                    throw new NotImplementedException();
                default:
                    break;
            }
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