using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public class PlayerEntity: Entity
    {
        public int index;
        public Inventory inventory;
        public PlayerEntity():base() 
        {
            inventory = new Inventory();
        }




    }
}
