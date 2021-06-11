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
        private int id;
        private GameSimulation sim;
        private SimulationModel model;
        public PlayerEntity playerEntity;
        private bool moving = true;

        private Entity heap_clicked = null;

        private int click_for_interact = 0;
        private const int CLICK_INTERACT_NEEDS = 5;

        private int timer_interact = 0;
        private Vector2 deltaxy;
        private const int INTERACT_LOST_AFTER = 30;//dopo quanti frame il contatore click_for_interact viene azzerato perchè ho smesso di cliccare

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
        }
        public void Update(TimeSpan elapsedTime)
        {
            deltaxy = Vector2.Zero;
            update_input();
            playerEntity.update_timer_dash();
            if(heap_clicked!= null && click_for_interact > 0)
            {
                if (timer_interact >= INTERACT_LOST_AFTER)
                    click_for_interact = 0;
                else
                    timer_interact++;
            }
            update_position(elapsedTime);
        }
        public void update_input()
        {
            if(model.actions[id] == null)
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
                        if (heap_clicked != null && player_near_entity(heap_clicked, 64)/* playerEntity.collisionrect.Intersects(heap_clicked.collisionrect)*/)
                        {
                            if(click_for_interact >= CLICK_INTERACT_NEEDS)
                            {
                                loot_random_material();
                                click_for_interact = 0;
                            }
                            else
                            {
                                click_for_interact++;
                                timer_interact = 0;

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
                            if (player_near_entity(entity,12) && entity.tags.Contains(Tags.CAN_TAKE_ITEM) && playerEntity.inventory.entities.Count<= playerEntity.inventory.size && !playerEntity.inventory.entities.Contains(entity))
                            {
                                add_entity_in_inventory(entity);
                                sim.RemEntity(entity);
                            }
                            else if (player_near_entity(entity, 64)/*playerEntity.collisionrect.Intersects(entity.collisionrect)*/ && entity.tags.Contains(Tags.HEAP))
                            {
                                if (click_for_interact >= CLICK_INTERACT_NEEDS)
                                {
                                    loot_random_material();
                                    click_for_interact = 0;
                                }
                                else
                                {
                                    click_for_interact++;
                                    timer_interact = 0;
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
                                playerEntity.dash_timer_counter = playerEntity.TIMER_DASH;
                                movement(48, action.direction);
                            }
                        }
                        break;
                    case ActionModel.Type.Throw:
                        if(model.player_entities[id].inventory!= null)
                        {
                            var entity = model.player_entities[id].inventory.entities.LastOrDefault();
                            if (entity != null)
                            {
                                model.player_entities[id].inventory.entities.Remove(entity);
                                if (float.IsNaN(action.direction.X) && float.IsNaN(action.direction.Y))
                                    entity.direction = playerEntity.direction;
                                else
                                    entity.direction = action.direction;
                                entity.speed = 3;
                                entity.removeable = true;
                            }
                        }
                        break;
                    case ActionModel.Type.Move:
                        movement(3,action.direction);
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
                entity.posxy += new Vector2(posx, 0);
                entity.posz = 0;
                entity.life_time = Settings.TimeOnTheFloor;
            }

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
            if (playerEntity.inventory == null)
                playerEntity.inventory = new Inventory();
            if (playerEntity.inventory.entities.Count < playerEntity.inventory.size)
            {
                playerEntity.inventory.entities.Add(new_entity);
            }

            sim.AddEntity(new_entity);
        }

        private bool player_near_entity(Entity entity, int radius)
        {
            Vector2 pos_player = playerEntity.posxy;
            Vector2 pos_entity = entity.posxy ;

            if(Math.Abs(pos_entity.X - pos_player.X)<= radius && Math.Abs(pos_entity.Y - pos_player.Y) <= radius)
            {
                return true;
            }
            return false;
        }

        private void movement(float speedframe, Vector2 vector)
        {
           
            this.deltaxy += speedframe * vector;
            this.playerEntity.direction = vector;

            int deltax = (int)deltaxy.X;
            int deltay = (int)deltaxy.Y;
            playerEntity.collisionrect.Offset(deltax, deltay);
            foreach (var entity in model.entities.Values)
            {
                if (entity.through)
                    continue;
                var rect = Rectangle.Intersect(entity.collisionrect, playerEntity.collisionrect);
                if (rect.Width != 0 || rect.Height != 0)
                {
                    deltaxy = Vector2.Zero;
                    break;
                }
                //{
                ////    if (rect == playerEntity.collisionrect)
                ////        deltaxy = Vector2.Zero;
                //    else
                //    {
                //        bool with_less_height = rect.Width <= rect.Height;
                //        if (with_less_height)
                //        {
                //            deltaxy -= new Vector2(deltaxy.X, 0);

                //        }
                //        else
                //        {
                //            deltaxy -= new Vector2(0, deltaxy.Y);
                //        }
                //    }

                //}
            }
            playerEntity.collisionrect.Offset(-deltax, -deltay);

        }

        private void update_position(TimeSpan elapsedTime)
        {
            this.playerEntity.posxy += deltaxy;
            playerEntity.update_collision_rect();

            for (int i = 0; i < this.playerEntity.inventory.entities.Count; i++)
            {
                var entity = this.playerEntity.inventory.entities[i];
                entity.posxy = this.playerEntity.posxy;
                entity.posz = 48 + (i * 24 - 2);
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
                    if (recipe.ingredient_and_amount[i].Item1 == entity_name)
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
            playerEntity.inventory.list_recipes.RemoveAt(recipe_index);
            generate_new_recipe(recipe_index);
            return true;
        }

        /// <summary>
        /// genera una nuova recipe
        /// </summary>
        /// <param name="index_where_add"></param>
        private void generate_new_recipe(int index_where_add)
        {
            System.Random random = new System.Random();
            var index_recipe = random.Next(model.recipes.Count);
            var recipe_choosen = model.recipes[index_recipe];
            playerEntity.inventory.list_recipes[index_where_add] = new Recipe(recipe_choosen.name, recipe_choosen.ingredient_and_amount, recipe_choosen.owned, recipe_choosen.score, recipe_choosen.type, recipe_choosen.sentence_to_show, recipe_choosen.character_associated);
        }
    }
}
