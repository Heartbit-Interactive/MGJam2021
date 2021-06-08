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
        public GamePlayer(int id) 
        {
            this.id = id;
        }

        public void update_input(SimulationModel model)
        {
            float speedframe = 1;
            var player = model.player_entities[id];
            var kb = model.keyboards[id];
            var gp = model.gamepads[id];
            var speed = gp.ThumbSticks.Left * speedframe;
            speed.Y *= -1;
            player.posxy = player.posxy + speed;
            //tastiera direzioni
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
}
