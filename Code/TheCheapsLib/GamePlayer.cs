using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    //controller del player entity
    public class GamePlayer
    {
        private int id;
        private bool oldenter;
        private SimulationModel model;
        private int timer_evade = 0;
        private bool moving = true;
        private const int TIMEREVADE = 30;

        public GamePlayer(int id, SimulationModel model) 
        {
            this.id = id;
            this.model = model;
        }

        public void update_input()
        {
            //timers 
            if (timer_evade > 0)
                timer_evade--;
            var gp = model.gamepads[id];

            //base movement
            movement(1);
            //press button or keys
            if(trigger(Buttons.A) /*|| trigger(Keys.Enter)*/)
            {
                //interazione con ambiente con impulsi, reset se non raccolgo per x tempo
            }
            //schivata //B
            if ((trigger(Buttons.B) /*|| trigger(Keys.X)*/) && timer_evade == 0 && moving)
            {
                timer_evade = TIMEREVADE;
                //schivata
                movement(24);
            }
            //RT se non c'è direzione quella giusta è la direzione del giocatore 
            //sparare + analogico destro per mirare
            //potenza lancio costante
            if (trigger(Buttons.RightTrigger) && model.player_entities[id].inventory != null /*|| trigger(Keys.RightControl)*/)
            {
                var object_thrown = model.player_entities[id].inventory.last_entity();
                if (object_thrown != null)
                {
                    var vt_shoot = gp.ThumbSticks.Right;// vettore 
                    

                }

            }
            
        }

        private void movement(float speedframe)
        {
            //float speedframe = 2;
            var player = model.player_entities[id];

            var kb = model.keyboards[id];
            var gp = model.gamepads[id];

            if (gp.ThumbSticks.Left != Vector2.Zero)
            {
                //gamepad
                var speed = gp.ThumbSticks.Left * speedframe;
                speed.Y *= -1;
                player.posxy = player.posxy + speed;
            }
            else
            {
                //tastiera 
                if (kb.IsKeyDown(Keys.Left))
                {
                    player.posxy = player.posxy - speedframe * Vector2.UnitX;
                }
                if (kb.IsKeyDown(Keys.Right))
                {
                    player.posxy = player.posxy + speedframe * Vector2.UnitX;
                }
                if (kb.IsKeyDown(Keys.Up))
                {
                    player.posxy = player.posxy - speedframe * Vector2.UnitY;
                }
                if (kb.IsKeyDown(Keys.Down))
                {
                    player.posxy = player.posxy + speedframe * Vector2.UnitY;
                }
            }
        }

        public bool trigger(Buttons bt)
        {
            var gp = model.gamepads[id];
            var oldgp = model.oldGamepads[id];
            //System.Diagnostics.Debug.WriteLine("check trigger");
            if (gp.IsButtonDown(bt) && !oldgp.IsButtonDown(bt))
            {
                return true;
            }

            return false;

        }
        public bool trigger(Keys key)
        {
            var kb = model.keyboards[id];
            var oldkb = model.oldKeyboards[id];


            if (kb.IsKeyDown(key) && !oldkb.IsKeyDown(key))
            {
                return true;
            }

            return false;

        }

        //public bool release()
        //{

        //}
    }
}
