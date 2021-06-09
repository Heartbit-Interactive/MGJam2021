using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TheCheapsLib
{
    public class GameInput
    {
        private GamePadState oldGpState = new GamePadState();
        private KeyboardState oldKbState = new KeyboardState();
        public SimulationModel model;
        private GamePadState gpState;
        private KeyboardState kbState;

        public GameInput(SimulationModel model)
        {
            this.model = model;
        }
        public FrameActionState getActionState()
        {
            gpState = GamePad.GetState(0);
            kbState = Keyboard.GetState();
            FrameActionState actionList = new FrameActionState();
            Vector2 dir = Vector2.Zero;
            Vector2 dir2 = Vector2.Zero;
            if (gpState.IsConnected)
            {
                dir = gpState.ThumbSticks.Left;
                dir.Y *= -1;
                dir2 = gpState.ThumbSticks.Right;
                dir2.Y *= -1;
            }
            if (Press(Keys.Left))
            {
                dir += -Vector2.UnitX;
                dir2 += -Vector2.UnitX;
            }
            if (Press(Keys.Right))
            {
                dir += Vector2.UnitX;
                dir2 += -Vector2.UnitX;
            }
            if (Press(Keys.Up))
            {
                dir += -Vector2.UnitY;
                dir2 += -Vector2.UnitX;
            }
            if (Press(Keys.Down))
            {
                dir += Vector2.UnitY;
                dir2 += -Vector2.UnitX;
            }
            if (Trigger(Buttons.A) || Trigger(Keys.Z))
                actionList.Add(ActionModel.Type.Interact, dir);
            if (Trigger(Buttons.RightTrigger) || Trigger(Keys.C))
                actionList.Add(ActionModel.Type.Throw, dir2);
            if (Trigger(Buttons.B) || Trigger(Keys.X))
                actionList.Add(ActionModel.Type.Dash, dir);
            else if(dir.Length()>=0.25f)
                actionList.Add(ActionModel.Type.Move, dir);
            oldGpState = gpState;
            oldKbState = kbState;
            return actionList;
        }
        public bool Trigger(Keys key)
        {
            return kbState.IsKeyDown(key) && oldKbState.IsKeyUp(key);
        }
        public bool Trigger(Buttons button)
        {
            return gpState.IsButtonDown(button) && oldGpState.IsButtonUp(button);
        }
        public bool Release(Keys key)
        {
            return kbState.IsKeyUp(key) && oldKbState.IsKeyDown(key);
        }
        public bool Release(Buttons button)
        {
            return gpState.IsButtonUp(button) && oldGpState.IsButtonDown(button);
        }
        public bool Press(Keys key)
        {
            return kbState.IsKeyDown(key);
        }
        public bool Press(Buttons button)
        {
            return gpState.IsButtonDown(button);
        }

        public void SetActionState(FrameActionState actionState, int playerIndex)
        {
            if (model.actions[playerIndex] == null)
                model.actions[playerIndex] = new List<ActionModel>();
            model.actions[playerIndex].AddRange(actionState.List);
        }
    }
}
