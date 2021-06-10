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
        public int TIMER_DASH = 40;
        public int dash_timer_counter;
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
        public void update_timer_dash()
        {
            if (dash_timer_counter > 0)
                dash_timer_counter--;
        }

    }
}
