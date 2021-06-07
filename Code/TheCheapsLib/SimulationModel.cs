using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public static class SimulationModel
    {
        public static List<Entity> entities = new List<Entity>();
        public static GamePadState[] gamepads = new GamePadState[Settings.maxPlayers];
        public static GamePadState[] oldGamepads = new GamePadState[Settings.maxPlayers];
        public static KeyboardState[] keyboards = new KeyboardState[Settings.maxPlayers];
        public static KeyboardState[] oldKeyboards = new KeyboardState[Settings.maxPlayers];
    }
}
