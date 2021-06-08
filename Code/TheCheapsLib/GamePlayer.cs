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
        private bool oldenter;
        private SimulationModel model;
        private int timer_dash = 0;
        private bool moving = true;
        private const int TIMERDASH = 30;

        public GamePlayer(int id, SimulationModel model) 
        {
            this.id = id;
            this.model = model;
        }

        public void update_input()
        {
            //timers 
            if (timer_dash > 0)
                timer_dash--;
            //var gp = model.gamepads[id];
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
                        break;
                    case ActionModel.Type.Dash:
                        timer_dash = TIMERDASH;
                        //schivata
                        movement(24,action.direction);
                        break;
                    case ActionModel.Type.Throw:
                        var object_thrown = model.player_entities[id].inventory.last_entity();
                        if (object_thrown != null)
                        {
                            var vt_shoot =action.direction;// vettore 


                        }
                        break;
                    case ActionModel.Type.Move:
                        movement(1,action.direction);
                        break;

                }

                model.actions[id].RemoveAt(model.actions[id].Count - 1);
            }
        }

        private void movement(float speedframe, Vector2 vector)
        {
            var player = model.player_entities[id];

            player.posxy = player.posxy + speedframe * vector;

        }

        //public bool trigger(Buttons bt)
        //{
        //    var gp = model.gamepads[id];
        //    var oldgp = model.oldGamepads[id];
        //    //System.Diagnostics.Debug.WriteLine("check trigger");
        //    if (gp.IsButtonDown(bt) && !oldgp.IsButtonDown(bt))
        //    {
        //        return true;
        //    }

        //    return false;

        //}
        //public bool trigger(Keys key)
        //{
        //    var kb = model.keyboards[id];
        //    var oldkb = model.oldKeyboards[id];


        //    if (kb.IsKeyDown(key) && !oldkb.IsKeyDown(key))
        //    {
        //        return true;
        //    }

        //    return false;

        //}

        //public bool release()
        //{

        //}
    }
}
