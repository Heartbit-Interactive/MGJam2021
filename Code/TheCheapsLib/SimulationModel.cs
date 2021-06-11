using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public class SimulationModel
    {
        public Dictionary<int,Entity> entities = new Dictionary<int, Entity>();
        public List<PlayerEntity> player_entities = new List<PlayerEntity>();
        internal List<ActionModel>[] actions = new List<ActionModel>[Settings.maxPlayers];
        internal List<Entity> items = new List<Entity>();

        public List<int> updated_entities = new List<int>();
        public List<int> removed_entities = new List<int>();
        public List<Entity> added_entities = new List<Entity>();
    }
}
