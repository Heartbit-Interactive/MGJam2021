using System;
using System.Collections.Generic;
using System.IO;
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
        public override void BinaryRead(BinaryReader br)
        {
            base.BinaryRead(br);            
        }
        public override void BinaryWrite(BinaryWriter bw)
        {
            base.BinaryWrite(bw);
        }


    }
}
