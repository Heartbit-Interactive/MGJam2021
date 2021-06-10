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
        private SimulationModel model;
        public PlayerEntity playerEntity;
        private bool moving = true;

        private Entity heap_clicked = null;

        private int click_for_interact = 0;
        private const int CLICK_INTERACT_NEEDS = 5;

        private int timer_interact = 0;
        private Vector2 deltaxy;
        private const int INTERACT_LOST_AFTER = 30;//dopo quanti frame il contatore click_for_interact viene azzerato perchè ho smesso di cliccare


        public GamePlayer(int id, SimulationModel model) 
        {
            this.id = id;
            this.model = model;
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
                        for(int i =  model.entities.Count-1; i>=0; i--)
                        {
                            var entity = model.entities[i];
                            if (player_near_entity(entity,12) && entity.tags.Contains(Tags.CAN_TAKE_ITEM) && playerEntity.inventory.entities.Count<= playerEntity.inventory.size && !playerEntity.inventory.entities.Contains(entity))
                            {
                                add_entity_in_inventory(entity);
                                model.entities.Remove(entity);
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
                                //Entity object_thrown = new Entity(entity.texture_path, entity.name, entity.posxy, entity.z, entity.sourcerect, entity.direction, entity.through, entity.speed, entity.tags, entity.collisionrect, entity.texture, entity.origin, entity.posz);
                                model.player_entities[id].inventory.entities.Remove(entity);
                                if (float.IsNaN(action.direction.X) && float.IsNaN(action.direction.Y))
                                    entity.direction = playerEntity.direction;
                                else
                                    entity.direction = action.direction;
                                entity.speed = 3;
                                entity.removeable = true;
                                model.entities.Add(entity);
                            }
                        }
                       
                        break;
                    case ActionModel.Type.Move:
                        movement(3,action.direction);
                        break;

                }

                model.actions[id].RemoveAt(model.actions[id].Count - 1);
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
            Entity new_entity = new Entity(entity.texture_path, entity.name, posxy, entity.z, entity.sourcerect, entity.direction, entity.through, entity.speed, entity.tags, entity.collisionrect, entity.texture, entity.origin, entity.posz,false);
            if (playerEntity.inventory == null)
                playerEntity.inventory = new Inventory();
            if (playerEntity.inventory.entities.Count < playerEntity.inventory.size)
            {
                playerEntity.inventory.entities.Add(new_entity);
            }

            model.entities.Add(new_entity);
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
        }

        private void update_position(TimeSpan elapsedTime)
        {
            this.playerEntity.posxy += deltaxy;

            for(int i =0; i <  this.playerEntity.inventory.entities.Count; i++)
            {
                var entity = this.playerEntity.inventory.entities[i];
                entity.posxy = this.playerEntity.posxy;
                entity.posz = 48 + (i * 24 - 2);
            }
        }
    }
}
