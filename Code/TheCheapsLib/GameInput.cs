using System;
using System.Collections.Generic;
using System.IO;
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
        public byte[] serializeActionState()
        {
            gpState = GamePad.GetState(0);
            kbState = Keyboard.GetState();
            var actionList = new List<ActionModel>();
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
                dir = -Vector2.UnitX;
                dir2 = -Vector2.UnitX;
            }
            else if (Press(Keys.Right))
            {
                dir = Vector2.UnitX;
                dir2 = -Vector2.UnitX;
            }
            else if (Press(Keys.Up))
            {
                dir = -Vector2.UnitY;
                dir2 = -Vector2.UnitX;
            }
            else if (Press(Keys.Down))
            {
                dir = Vector2.UnitY;
                dir2 = -Vector2.UnitX;
            }
            if (Trigger(Buttons.A) || Trigger(Keys.Z))
                actionList.Add(new ActionModel() { type = ActionModel.Type.Interact, direction = dir });
            if (Trigger(Buttons.RightTrigger) || Trigger(Keys.C))
                actionList.Add(new ActionModel() { type = ActionModel.Type.Throw, direction = dir2 });
            if (Trigger(Buttons.B) || Trigger(Keys.X))
                actionList.Add(new ActionModel() { type = ActionModel.Type.Dash, direction = dir });
            else if(dir.Length()>=0.25f)
                actionList.Add(new ActionModel() { type = ActionModel.Type.Move, direction = dir });
            oldGpState = gpState;
            oldKbState = kbState;

            using (var memstream = new MemoryStream(32 * 1024))
            {
                using (var bw = new BinaryWriter(memstream))
                {
                    bw.Write(actionList.Count);
                    foreach (var action in actionList)
                        action.binary_write(bw);
                }
                return memstream.ToArray();
            }
        }
        public void deserializeActionState(byte[] buffer, int playerIndex)
        {
            using (var memstream = new MemoryStream(buffer))
            {
                using (var br = new BinaryReader(memstream))
                {
                    var count = br.ReadInt32();
                    var list = new List<ActionModel>(count);
                    for (int i = 0; i < count; i++)
                    {
                        var action = new ActionModel();
                        action.binary_read(br);
                        list.Add(action);
                    }
                    if (model.actions[playerIndex] == null)
                        model.actions[playerIndex] = new List<ActionModel>();
                    model.actions[playerIndex].AddRange(list);
                }
            }
        }
#if TRUE
        public byte[] serializeInputState()
        {
            //System.Diagnostics.Debug.WriteLine("write gamepad state");
            var gpState = GamePad.GetState(0);
            var kbState = Keyboard.GetState();
            using (var memstream = new MemoryStream(32 * 1024))
            {
                using (var bw = new BinaryWriter(memstream))
                {
                    writeGamePadState(gpState, bw);
                    writeGamePadState(oldGpState, bw);
                    writeKeyboardState(kbState, bw);
                    writeKeyboardState(oldKbState, bw);
                }
                oldGpState = gpState;
                oldKbState = kbState;
                return memstream.ToArray();
            }
        }

        private void writeGamePadState(GamePadState gpState, BinaryWriter bw)
        {
            bw.Write(gpState.ThumbSticks.Left.X);
            bw.Write(gpState.ThumbSticks.Left.Y);

            bw.Write(gpState.ThumbSticks.Right.X);
            bw.Write(gpState.ThumbSticks.Right.Y);

            bw.Write(gpState.Triggers.Left);
            bw.Write(gpState.Triggers.Right);

            bw.Write(gpState.Buttons.A == ButtonState.Pressed);
            bw.Write(gpState.Buttons.B == ButtonState.Pressed);
            bw.Write(gpState.Buttons.X == ButtonState.Pressed);
            bw.Write(gpState.Buttons.Y == ButtonState.Pressed);
            bw.Write(gpState.Buttons.LeftShoulder == ButtonState.Pressed);
            bw.Write(gpState.Buttons.RightShoulder == ButtonState.Pressed);
        }
        private void writeKeyboardState(KeyboardState kbState, BinaryWriter bw)
        {
            var keys = kbState.GetPressedKeys();
            bw.Write(keys.Length);
            for (int i = 0; i < keys.Length; i++)
                bw.Write((int)keys[i]);
        }

        public void deserializeInputState(byte[] buffer,int playerIndex)
        {
            using (var memstream = new MemoryStream(buffer))
            {
                GamePadState gamePadState,oldGamePadState;
                KeyboardState kbState, oldKbState;
                using (var br = new BinaryReader(memstream))
                {
                    //System.Diagnostics.Debug.WriteLine("read gamepad state");
                    gamePadState = readGamePadState(br);
                    oldGamePadState = readGamePadState(br);
                    kbState = readKeyboardState(br);
                    oldKbState = readKeyboardState(br);
                }
                model.gamepads[playerIndex] = gamePadState;
                model.oldGamepads[playerIndex] = oldGamePadState;
                model.keyboards[playerIndex] = kbState;
                model.oldKeyboards[playerIndex] = oldKbState;
            }
        }
        private GamePadState readGamePadState(BinaryReader br)
        {
            GamePadState gamePadState;
            var left = Vector2.Zero;
            left.X = br.ReadSingle();
            left.Y = br.ReadSingle();
            var right = Vector2.Zero;
            right.X = br.ReadSingle();
            right.Y = br.ReadSingle();
            var trigger_left = br.ReadSingle();
            var trigger_right = br.ReadSingle();

            var buttonA = br.ReadBoolean();
            var buttonB = br.ReadBoolean();
            var buttonX = br.ReadBoolean();
            var buttonY = br.ReadBoolean(); 
            var buttonLS = br.ReadBoolean();
            var buttonRS = br.ReadBoolean();
            var button_list = new List<Buttons>();
            if (buttonA)
                button_list.Add(Buttons.A);
            if (buttonB)
                button_list.Add(Buttons.B);
            if (buttonX)
                button_list.Add(Buttons.X);
            if (buttonY)
                button_list.Add(Buttons.Y);
            if (buttonLS)
                button_list.Add(Buttons.LeftShoulder);
            if (buttonRS)
                button_list.Add(Buttons.RightShoulder);

            gamePadState = new GamePadState(left, right, trigger_left, trigger_right, button_list.ToArray());
            return gamePadState;
        }

        private KeyboardState readKeyboardState(BinaryReader br)
        {
            var l = br.ReadInt32();
            var keys = new Keys[l];
            for (int i = 0; i < l; i++)
                keys[i] = (Keys)br.ReadInt32();
            return new KeyboardState(keys);
        }
#endif
        public bool Trigger(Keys key)
        {
            return kbState.IsKeyDown(key) && oldKbState.IsKeyUp(key);
        }
        public bool Trigger(Buttons button)
        {
            return gpState.IsButtonDown(button) && gpState.IsButtonUp(button);
        }
        public bool Release(Keys key)
        {
            return kbState.IsKeyUp(key) && oldKbState.IsKeyDown(key);
        }
        public bool Release(Buttons button)
        {
            return gpState.IsButtonUp(button) && gpState.IsButtonDown(button);
        }
        public bool Press(Keys key)
        {
            return kbState.IsKeyDown(key);
        }
        public bool Press(Buttons button)
        {
            return gpState.IsButtonDown(button);
        }
    }
}
