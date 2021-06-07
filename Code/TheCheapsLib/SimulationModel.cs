﻿using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public class SimulationModel
    {
        public List<Entity> entities = new List<Entity>();
        public List<PlayerEntity> player_entities = new List<PlayerEntity>();
        public GamePadState[] gamepads = new GamePadState[Settings.maxPlayers];
        public GamePadState[] oldGamepads = new GamePadState[Settings.maxPlayers];
        public KeyboardState[] keyboards = new KeyboardState[Settings.maxPlayers];
        public KeyboardState[] oldKeyboards = new KeyboardState[Settings.maxPlayers];
    }
}
