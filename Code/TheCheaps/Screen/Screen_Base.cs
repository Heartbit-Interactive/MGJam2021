using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheaps.Scenes
{
    public abstract class Screen_Base
    {
        public abstract void LoadContent(ContentManager content);
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Terminate(ContentManager content);

        protected void RefreshInputState()
        {
            oldKbState = kbState;
            oldGpState = gpState;
            kbState = Keyboard.GetState();
            gpState = GamePad.GetState(0);
        }

        KeyboardState kbState, oldKbState;
        GamePadState gpState, oldGpState;
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
