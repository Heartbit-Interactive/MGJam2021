using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public class SimulationModel
    {
        //STATE
        public float timer = Settings.TotalGameTimeS;
        public Dictionary<int,Entity> entities = new Dictionary<int, Entity>();
        public List<PlayerEntity> player_entities = new List<PlayerEntity>();
        internal List<ActionModel>[] actions = new List<ActionModel>[Settings.maxPlayers];
        //DELTA
        public HashSet<Entity> updated_entities = new HashSet<Entity>();
        public List<int> removed_entities = new List<int>();
        public List<Entity> added_entities = new List<Entity>();
        /// <summary>
        /// Contains the informations required to show the TV-styled news bar on all player screens
        /// </summary>
        public List<int> broadcasting_news=new List<int>(); 

        //UNCHANGEABLE/NON BROADCASTED (client has these loaded in the game_screen for referencing)
        internal List<Entity> items = new List<Entity>();
        internal List<Recipe> recipes = new List<Recipe>();
    }
}
