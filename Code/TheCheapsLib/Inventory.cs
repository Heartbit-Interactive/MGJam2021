using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public class Inventory
    {
        public List<Entity> entities = new List<Entity>();//id oggetti posseduti

        public Inventory() { }

        public Entity last_entity()
        {
            int lenght = entities.Count;
            if (lenght <= 0)
                return null;
            return entities[lenght - 1];
        }

    }
}
