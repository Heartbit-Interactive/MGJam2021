using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TheCheapsLib
{
    public static class GameInput
    {
        private static GamePadState oldGpState = new GamePadState();
        private static KeyboardState oldKbState = new KeyboardState();

        public static byte[] serializeInputState()
        {
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

        private static void writeGamePadState(GamePadState gpState, BinaryWriter bw)
        {
            bw.Write(gpState.ThumbSticks.Left.X);
            bw.Write(gpState.ThumbSticks.Left.Y);
            bw.Write(gpState.Buttons.A == ButtonState.Pressed);
            bw.Write(gpState.Buttons.B == ButtonState.Pressed);
        }
        private static void writeKeyboardState(KeyboardState kbState, BinaryWriter bw)
        {
            var keys = kbState.GetPressedKeys();
            bw.Write(keys.Length);
            for (int i = 0; i < keys.Length; i++)
                bw.Write((int)keys[i]);
        }

        private static KeyboardState readKeyboardState(BinaryReader br)
        {
            var l = br.ReadInt32();
            var keys = new Keys[l];
            for (int i = 0; i < l; i++)
                keys[i] = (Keys)br.ReadInt32();
            return new KeyboardState(keys);
        }

        public static void deserializeInputState(byte[] buffer,int playerIndex)
        {
            using (var memstream = new MemoryStream(buffer))
            {
                GamePadState gamePadState,oldGamePadState;
                KeyboardState kbState, oldKbState;
                using (var br = new BinaryReader(memstream))
                {
                    gamePadState = readGamePadState(br);
                    oldGamePadState = readGamePadState(br);
                    kbState = readKeyboardState(br);
                    oldKbState = readKeyboardState(br);
                }
                SimulationModel.gamepads[playerIndex] = gamePadState;
                SimulationModel.oldGamepads[playerIndex] = oldGamePadState;
            }
        }

        private static GamePadState readGamePadState(BinaryReader br)
        {
            GamePadState gamePadState;
            var left = Vector2.Zero;
            left.X = br.ReadSingle();
            left.Y = br.ReadSingle();
            var buttonA = br.ReadBoolean();
            var buttonB = br.ReadBoolean();
            var button_list = new List<Buttons>();
            if (buttonA)
                button_list.Add(Buttons.A);
            if (buttonB)
                button_list.Add(Buttons.B);
            gamePadState = new GamePadState(left, Vector2.Zero, 0, 0, button_list.ToArray());
            return gamePadState;
        }
    }
}
