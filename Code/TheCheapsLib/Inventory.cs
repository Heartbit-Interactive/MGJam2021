using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public class Inventory
    {
        public Entity[] entities = new Entity[3];//id oggetti posseduti

        public Inventory() { }

        public Entity last_entity()
        {
            int lenght = entities.Length;
            if (lenght <= 0)
                return null;
            return entities[lenght - 1];
        }

    }
}
