using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public static class SimulationModel
    {
        public static List<Entity> entities = new List<Entity>();
        public static List<PlayerEntity> player_entities = new List<PlayerEntity>();
        public static List<GamePadState> gamepads = new List<GamePadState>();
    }
}
