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
        public GamePlayer(int id, SimulationModel model) 
        {
            this.id = id;
            this.model = model;
        }

        public void update_input()
        {
            float speedframe = 1;
            var player = model.player_entities[id];
            var kb = model.keyboards[id];
            var gp = model.gamepads[id];
            //MOVIMENTO DIREZIONALE
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
            if(trigger(Buttons.A) || trigger(Keys.Enter))
            {

            }
            //if (trigger(Buttons.B) || trigger(Keys.Back))
            //{

            //}

        }

        public bool trigger(Buttons bt)
        {
            var gp = model.gamepads[id];
            var oldgp = model.oldGamepads[id];


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
