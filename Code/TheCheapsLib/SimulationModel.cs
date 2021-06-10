using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public class SimulationModel
    {
        public List<Entity> entities = new List<Entity>();
        public List<PlayerEntity> player_entities = new List<PlayerEntity>();
        internal List<ActionModel>[] actions = new List<ActionModel>[Settings.maxPlayers];
        internal List<Entity> items = new List<Entity>();
    }
}
