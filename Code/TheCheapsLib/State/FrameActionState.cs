using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheCheapsLib
{
    public class FrameActionState : State
    {
        List<ActionModel> actionList = new List<ActionModel>();
        public IEnumerable<ActionModel> List { get { return actionList; } }
        public int Count { get { return actionList.Count; } }
        public FrameActionState() { }
        public override void BinaryRead(BinaryReader br)
        {
            base.BinaryRead(br);
            var count = br.ReadInt32();
            actionList = new List<ActionModel>(count);
            for (int i = 0; i < count; i++)
            {
                var action = ActionModel.Create();
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

        internal void Clear()
        {
            foreach (var action in actionList)
                action.Dispose();
            actionList.Clear();
        }

        internal void Add(ActionModel.Type type, Vector2 dir)
        {
            if (actionList == null)
                actionList = new List<ActionModel>();
            var item = ActionModel.Create();
            item.type = type;
            item.direction = dir;
            actionList.Add(item);
        }
    }
}
