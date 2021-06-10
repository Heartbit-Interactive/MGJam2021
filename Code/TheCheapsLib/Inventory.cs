using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public class Inventory
    {
        public List<Entity> entities = new List<Entity>();//id oggetti posseduti
        internal int size = 3;

        public Inventory() { }

        public Entity last_entity()
        {
            int lenght = entities.Count;
            if (lenght <= 0)
                return null;
            return entities[lenght - 1];
        }

        internal void update_entities_pos()
        {

        }
    }
}
