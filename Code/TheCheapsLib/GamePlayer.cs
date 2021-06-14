using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheCheapsLib
{
    //controller del player entity
    public class GamePlayer
    {
        public int id;
        private GameSimulation sim;
        private SimulationModel model;
        public PlayerEntity playerEntity;

        private Entity heap_clicked = null;

        private int click_for_interact = 0;

        private float stoppedInteractingTimer = 0;
        private Vector2 deltaxy;
        internal List<Entity> launched_items = new List<Entity>();
        public List<Recipe> recipes_associated = new List<Recipe>();

        /// <summary>
        /// PRIVATE: USA LA POOL PER CREARE
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sim"></param>
        public GamePlayer(int id, GameSimulation sim) 
        {
            this.id = id;
            this.sim = sim;
            this.model = sim.model;
            this.playerEntity = model.player_entities[id];
            //for(int i =0; i< Settings.RecipesPerPlayer; i++)
            //{
            //    generate_new_recipe(i);
            //}
        }
        public void Update(float elapsedTimeS)
        {
            deltaxy = Vector2.Zero;
            if (stunnedCounter > 0)
            {
                updateStunned(elapsedTimeS);
                return;
            }
            update_input(elapsedTimeS);
            playerEntity.update_timer_dash(elapsedTimeS);
            if (heap_clicked != null && click_for_interact > 0)
            {
                if (stoppedInteractingTimer >= Settings.MaxTimeBetwheenClicksToMineResource)
                    click_for_interact = 0;
                else
                    stoppedInteractingTimer += elapsedTimeS;
            }
            update_position(elapsedTimeS);
            update_animation_walk(elapsedTimeS);
            update_animation_dash(elapsedTimeS);


        }

        private void update_animation_dash(float elapsedTimeS)
        {
            if (performing_dash)
            {
                playerEntity.frame_index = 6;
                performing_dash = false;
            }
        }

        private void updateStunned(float elapsedTimeS)
        {
            stunnedCounter -= elapsedTimeS;
            if (stunnedCounter <= 0)
                playerEntity.frame_index = 0;
            else if (playerEntity.frame_index < 3)
                playerEntity.frame_index = 3;
            else
            {
                if (model.actions[id] != null)
                    model.actions[id].Clear();
                    frame_counter -= elapsedTimeS;
                if (frame_counter < 0)
                {
                    frame_counter = max_frame_counter;
                    step_index = (step_index + 1) % 3;
                    playerEntity.frame_index = step_index + 3;
                }
            }
        }

        int step_index = 0;
        float max_frame_counter = 1 / (float)Settings.PlayerFrameRate;
        float continue_walking_for = 0;
        private void update_animation_walk(float elapsedTime)
        {
            if (deltaxy != Vector2.Zero)
            {
                frame_counter -= elapsedTime;
                if (frame_counter < 0)
                {
                    frame_counter = max_frame_counter;
                    step_index = (step_index + 1) % 4;
                    if (step_index == 3)
                        playerEntity.frame_index = 1;
                    else
                        playerEntity.frame_index = step_index;
                }
                continue_walking_for = Settings.ContinueWalkingAnimeFor;
            }
            else if (continue_walking_for > 0)
            {
                continue_walking_for -= elapsedTime;
                frame_counter -= elapsedTime;
                if (frame_counter < 0)
                {
                    frame_counter = max_frame_counter;
                    step_index = (step_index + 1) % 4;
                    if (step_index == 3)
                        playerEntity.frame_index = 1;
                    else
                        playerEntity.frame_index = step_index;
                }
            }
            else
            {
                playerEntity.frame_index = 0;
                frame_counter = max_frame_counter;
            }
        }

        float frame_counter = 0;
        private float stunnedCounter;
        private bool performing_dash;

        public void update_input(float elapsedTime)
        {
            var current_speed = Vector2.Zero;
            if (model.actions[id] == null)
            {
                return;
            }
            while (model.actions[id].Count > 0)
            
            {
                var action = model.actions[id].LastOrDefault();                
                switch (action.type)
                {
                    case ActionModel.Type.Interact:
                        //heap interact
                        if (heap_clicked != null && player_near_entity(heap_clicked, Settings.DistanceForMiningTrash)/* playerEntity.collisionrect.Intersects(heap_clicked.collisionrect)*/)
                        {
                            if(click_for_interact >= Settings.ClicksRequiredToMineResource)
                            {
                                loot_random_material();
                                click_for_interact = 0;
                            }
                            else
                            {
                                model.special_commands.Add(new S2CActionModel() { type = S2CActionModel.Type.Shake, parameters = new int[] { heap_clicked.uniqueId, Settings.DurationShakeMsec, Settings.SpeedShake, Settings.AmplitudeShake } });
                                model.special_commands.Add(new S2CActionModel() { type = S2CActionModel.Type.SE, parameters = new int[] { id, (int)SEType.Rummage} });
                                click_for_interact++;
                                stoppedInteractingTimer = 0;

                            }
                            break;
                        }
                        else
                        {
                            heap_clicked = null;
                            click_for_interact = 0;
                        }
                        foreach(var entity in model.entities.Values)
                        {
                            if (entity.tags.Contains(Tags.CAN_TAKE_ITEM) && player_near_entity(entity,Settings.DistanceForPickup) && playerEntity.inventory.entities.Count<= Settings.InventoryMaxSize && !playerEntity.inventory.entities.Contains(entity))
                            {
                                add_entity_in_inventory(entity);
                                sim.RemEntity(entity);
                            }
                            else if (entity.tags.Contains(Tags.HEAP) && player_near_entity(entity, Settings.DistanceForMiningTrash))
                            {
                                if (click_for_interact >= Settings.ClicksRequiredToMineResource)
                                {
                                    loot_random_material();
                                    click_for_interact = 0;
                                }
                                else
                                {
                                    model.special_commands.Add(new S2CActionModel() { type = S2CActionModel.Type.Shake, parameters = new int[] { entity.uniqueId, Settings.DurationShakeMsec, Settings.SpeedShake, Settings.AmplitudeShake } });
                                    click_for_interact++;
                                    stoppedInteractingTimer = 0;
                                }
                                heap_clicked = entity;
                                break;
                            }
                            else
                            {
                                click_for_interact = 0;
                            }
                        }
                        break;
                    case ActionModel.Type.Dash:
                        {
                            if (playerEntity.dash_timer_counter <= 0)
                            {
                                playerEntity.dash_timer_counter = Settings.DashRecoilS;
                                if (float.IsNaN(action.direction.X) && float.IsNaN(action.direction.Y))
                                    action.direction = playerEntity.direction;
                                accumulateMovement(Settings.DashDistance, action.direction);
                                performing_dash = true;
                            }
                        }
                        break;
                    case ActionModel.Type.Throw:
                        if(model.player_entities[id].inventory!= null)
                        {
                            var entity = model.player_entities[id].inventory.entities.LastOrDefault();
                            if (entity != null)
                            {
                                model.special_commands.Add(new S2CActionModel() { type = S2CActionModel.Type.SE, parameters = new int[] { id, (int)SEType.Throw } });
                                model.player_entities[id].inventory.entities.Remove(entity);
                                if (float.IsNaN(action.direction.X) && float.IsNaN(action.direction.Y))
                                    entity.direction = playerEntity.direction;
                                else
                                    entity.direction = action.direction;
                                var speed_vector = Settings.ThrowSpeed * entity.direction;
                                speed_vector += current_speed;
                                entity.posz = Settings.ThrowHeight;
                                entity.speed = speed_vector.Length();
                                entity.direction = Vector2.Normalize(speed_vector);
                                entity.removeable = true;
                                entity.tags.Add(Tags.CAN_TAKE_ITEM);
                                entity.hasShadow = true;
                                launched_items.Add(entity);
                            }
                        }
                        break;
                    case ActionModel.Type.Move:
                        current_speed = Settings.MoveSpeed * Vector2.Normalize(action.direction);
                        accumulateMovement(Settings.MoveSpeed*elapsedTime,action.direction);
                        break;

                }
                action.Dispose();
                model.actions[id].RemoveAt(model.actions[id].Count - 1);
            }
        }

        /// <summary>
        /// cadono tutti gli oggetti che ha sopra e viene stunnato
        /// </summary>
        internal void stun_player()
        {
            for(int i=0; i< this.playerEntity.inventory.entities.Count; i++)
            {
                var entity = this.playerEntity.inventory.entities[i];
                var posx = (i - 1)*5;
                if (float.IsNaN(posx))
                    System.Diagnostics.Debugger.Break();
                entity.posxy += new Vector2(posx, 0);
                entity.posz = 0;
                entity.life_time = Settings.TimeOnTheFloor;
            }
            this.stunnedCounter = Settings.StunDuration;
        }

        private void loot_random_material()
        {
            var type_list = heap_clicked.tags.Where(x => x != Tags.HEAP).ToList();

            System.Random random = new System.Random();
            var index_chosen = random.Next(type_list.Count);
            var entity = model.items.Where(x => x.name == type_list[index_chosen]).FirstOrDefault();
            add_entity_in_inventory(entity);
            heap_clicked = null;
        }

        private void add_entity_in_inventory(Entity entity )
        {
            Vector2 posxy;
            if (heap_clicked != null)
                posxy = heap_clicked.posxy;
            else
                posxy = entity.posxy;
            Entity new_entity = entity.Clone();
            new_entity.through = true;
            //Se raccolgo da terra voglio che nn possa scadere
            new_entity.removeable = false;
            new_entity.life_time = Settings.TimeOnTheFloor;
            new_entity.tags.Remove(Tags.CAN_TAKE_ITEM);
            new_entity.hasShadow = false;

            if (playerEntity.inventory == null)
                playerEntity.inventory = new Inventory();
            if (playerEntity.inventory.entities.Count < Settings.InventoryMaxSize)
            {
                playerEntity.inventory.entities.Add(new_entity);
            }

            sim.AddEntity(new_entity);
        }

        private bool player_near_entity(Entity entity, int radius)
        {
            Vector2 pos_player = playerEntity.posxy;
            Vector2 pos_entity = entity.posxy ;

            if(Math.Abs(pos_entity.X - pos_player.X)<= radius + 16)
            {
                var distance_y = pos_entity.Y - pos_player.Y;
                if ( distance_y< 0)
                {
                    if (Math.Abs(distance_y) <= 25)
                        return true;
                }
                else
                {
                    if (distance_y <= radius)
                        return true;
                }
            }
            return false;
        }

        private void accumulateMovement(float movePixels, Vector2 direction)
        {
            this.playerEntity.direction = direction;
            //Movement to add to currently accumulated movement this frame (we can use it to partially roll back this move)
            var newmove_additional_deltaxy = movePixels * direction;
            if (float.IsNaN(newmove_additional_deltaxy.X))
                System.Diagnostics.Debugger.Break();
            //Store the current accumulated movement to restore it if needed
            var prevdelta = this.deltaxy;
            //Accumulate new movement
            this.deltaxy += newmove_additional_deltaxy;
            if (float.IsNaN(deltaxy.X))
                System.Diagnostics.Debugger.Break();
            //Player collision rect was not yet updated this frame, so we shift a copy of it for the accumulated movement
            var tentativePlayerCollRect = playerEntity.get_displaced_collision_rect(deltaxy);
            //Cycle all other entities
            foreach (var otherEntity in model.entities.Values)
            {
                //That have collision
                if (otherEntity.through || otherEntity.collisionrect.Width == 0)
                    continue;
                var rect = Rectangle.Intersect(otherEntity.collisionrect, tentativePlayerCollRect);
                //Collision strategies
                if (rect.Width != 0 || rect.Height != 0)
                {
                    if (rect.Height == tentativePlayerCollRect.Height) //Horz Collision?
                    {
                        if (rect.Width == tentativePlayerCollRect.Width) //Total Compenetration (Cancel move)
                        {
                            deltaxy = prevdelta;
                            if (float.IsNaN(deltaxy.X))
                                System.Diagnostics.Debugger.Break();
                            return;
                        }
                        else
                            deltaxy -= new Vector2(newmove_additional_deltaxy.X, 0); //Horz Collision!
                    }
                    else if (rect.Width == tentativePlayerCollRect.Width) //Vert Collision!
                    {
                        deltaxy -= new Vector2(0, newmove_additional_deltaxy.Y);
                    }
                    else //Ambiguous Collision!
                    {
                        //Try to cancel H move
                        deltaxy -= new Vector2(newmove_additional_deltaxy.X, 0);
                        tentativePlayerCollRect = playerEntity.get_displaced_collision_rect(deltaxy);
                        //if collides afterwards rollback and cancel V move
                        rect = Rectangle.Intersect(otherEntity.collisionrect, tentativePlayerCollRect);
                        if (rect.Width != 0 || rect.Height != 0)
                        {
                            deltaxy += new Vector2(newmove_additional_deltaxy.X, -newmove_additional_deltaxy.Y);
                        }
                    }
                    //Final collision test
                    tentativePlayerCollRect = playerEntity.get_displaced_collision_rect(deltaxy);
                    rect = Rectangle.Intersect(otherEntity.collisionrect, tentativePlayerCollRect);
                    //If after removing the component we still collide, cancel the movement completely
                    if (rect.Width != 0 || rect.Height != 0)
                    {
                        deltaxy = prevdelta;
                        if (float.IsNaN(deltaxy.X))
                            System.Diagnostics.Debugger.Break();
                        return;
                    }
                }
            }
        }

        private void update_position(float elapsedTime)
        {
            if (float.IsNaN(deltaxy.X))
                System.Diagnostics.Debugger.Break();
            this.playerEntity.posxy += deltaxy;
            playerEntity.update_collision_rect();

            for (int i = 0; i < this.playerEntity.inventory.entities.Count; i++)
            {
                var entity = this.playerEntity.inventory.entities[i];
                entity.posxy = this.playerEntity.posxy;
                entity.posz = 42 + i * (20 - 2);
                entity.hasShadow = false;
                model.updated_entities.Add(entity);
            }
        }

        public bool if_player_needs_ingredient_add(string entity_name)
        {
            for (int j = 0; j < playerEntity.inventory.list_recipes.Count; j++)
            {
                var recipe = playerEntity.inventory.list_recipes[j];
                for (int i = 0; i < recipe.ingredient_and_amount.Count; i++)
                {
                    var tupla = recipe.ingredient_and_amount[i];
                    if (tupla.Item1 == entity_name && tupla.Item2> recipe.owned[i])
                    {                        
                        recipe.owned[i]++;
                        recipe_completed(j);
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// elimina la precedente recipe
        /// </summary>
        /// <param name="recipe_index"></param>
        /// <returns></returns>
        public bool recipe_completed(int recipe_index)
        {
            var recipe = playerEntity.inventory.list_recipes[recipe_index];
            for (int i = 0; i < recipe.ingredient_and_amount.Count; i++)
            {
                var ingr = recipe.ingredient_and_amount[i];
                if (ingr.Item2 != recipe.owned[i])
                {
                    return false;
                }
            }
            playerEntity.score += recipe.score;
            sim.model.broadcasting_news = new List<int> { recipe.id, playerEntity.index };
            var baseE = sim.model.entities.Values.FirstOrDefault(x => x.name == "BASE_0" + (playerEntity.index + 1));
            if (baseE.frame_index < 3)
                baseE.frame_index++;
            model.updated_entities.Add(baseE);
            playerEntity.inventory.list_recipes.RemoveAt(recipe_index);
            generate_new_recipe(recipe_index);
            return true;
        }

        /// <summary>
        /// genera una nuova recipe
        /// </summary>
        /// <param name="index_where_add"></param>
        public void generate_new_recipe(int index_where_add)
        {
            System.Random random = new System.Random();
            var index_recipe = random.Next(recipes_associated.Count);
            var recipe_choosen = recipes_associated[index_recipe];
            var new_recipe= new Recipe(recipe_choosen.name, recipe_choosen.ingredient_and_amount, recipe_choosen.owned, recipe_choosen.score, recipe_choosen.type, recipe_choosen.sentence_to_show, recipe_choosen.character_associated);
            new_recipe.id = recipe_choosen.id;
            if (playerEntity.inventory.list_recipes.Count <= index_where_add)
                playerEntity.inventory.list_recipes.Add(new_recipe);
            else
                playerEntity.inventory.list_recipes[index_where_add] = new_recipe;
        }
    }
}
