using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheCheapsLib
{
    public class FrameActionState : State
    {
        List<ActionModel> actionList;

        public IEnumerable<ActionModel> List { get { return actionList; } }
        public FrameActionState() { }
        public override void BinaryRead(BinaryReader br)
        {
            base.BinaryRead(br);
            var count = br.ReadInt32();
            actionList = new List<ActionModel>(count);
            for (int i = 0; i < count; i++)
            {
                var action = new ActionModel();
                action.binary_read(br);
                actionList.Add(action);
            }            
        }

        public override void BinaryWrite(BinaryWriter bw)
        {
            base.BinaryWrite(bw);
            if (actionList == null)
            {
                bw.Write(0); return;
            }

            bw.Write(actionList.Count);
            foreach (var action in actionList)
                action.binary_write(bw);
        }

        internal void Add(ActionModel.Type type, Vector2 dir)
        {
            if (actionList == null)
                actionList = new List<ActionModel>();
            actionList.Add(new ActionModel() { type = type, direction = dir });
        }
    }
}
