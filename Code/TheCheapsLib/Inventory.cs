using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public class Inventory
    {
        public List<Entity> entities = new List<Entity>();//id oggetti posseduti
        internal int size = 3;
        public List<Recipe> list_recipes = new List<Recipe>();
        public Inventory() { }

        internal void update_entities_pos()
        {

        }
    }
}
