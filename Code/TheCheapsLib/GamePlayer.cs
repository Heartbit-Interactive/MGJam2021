﻿using Microsoft.Xna.Framework;
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
        private bool oldenter;
        private SimulationModel model;
        private PlayerEntity playerEntity;
        private bool moving = true;

        private Entity entity_clicked = null;

        private int click_for_interact = 0;
        private const int CLICK_INTERACT_NEEDS = 5;

        private int timer_interact = 0;
        private const int INTERACT_LOST_AFTER = 30;//dopo quanti frame il contatore click_for_interact viene azzerato perchè ho smesso di cliccare


        public GamePlayer(int id, SimulationModel model) 
        {
            this.id = id;
            this.model = model;
            this.playerEntity = model.player_entities[id];
        }
        public void update()
        {
            update_input();
            if(entity_clicked!= null && click_for_interact > 0)
            {
                if (timer_interact >= INTERACT_LOST_AFTER)
                    click_for_interact = 0;
                else
                    timer_interact++;
            }
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
                        if (entity_clicked != null && playerEntity.collisionrect.Intersects(entity_clicked.collisionrect))
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
                            entity_clicked = null;
                            click_for_interact = 0;
                        }
                        foreach (var entity in model.entities)
                        {
                            
                            if (playerEntity.collisionrect.Intersects(entity.collisionrect))
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
                                entity_clicked = entity;
                                //raccogli
                                break;
                            }
                            else
                            {
                                click_for_interact = 0;
                            }
                        }
                        break;
                    case ActionModel.Type.Dash:
                        
                            //schivata
                            movement(24, action.direction);                        
                        break;
                    case ActionModel.Type.Throw:
                        if(model.player_entities[id].inventory!= null)
                        {
                            var object_thrown = model.player_entities[id].inventory.last_entity();
                            if (object_thrown != null)
                            {
                                var vt_shoot = action.direction;// vettore 

                            }
                        }
                       
                        break;
                    case ActionModel.Type.Move:
                        movement(1,action.direction);
                        break;

                }

                model.actions[id].RemoveAt(model.actions[id].Count - 1);
            }
        }

        private void loot_random_material()
        {

        }

        private bool player_near_entity(Entity entity)
        {
            Vector2 pos_player = playerEntity.posxy;
            Vector2 pos_entity = entity.posxy;
            if(Math.Abs(pos_entity.X - pos_player.X)<= 2 && Math.Abs(pos_entity.Y - pos_player.Y) <= 2)
            {
                return true;
            }
            return false;
        }

        private void movement(float speedframe, Vector2 vector)
        {
            var player = model.player_entities[id];

            player.posxy = player.posxy + speedframe * vector;

        }
    }
}
