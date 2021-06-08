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

        KeyboardState kbState, oldKbState;
        GamePadState gpState, oldGpState;
        public bool Triggered(Keys key)
        {
            return kbState.IsKeyDown(key) && oldKbState.IsKeyUp(key);
        }
        public bool Triggered(Buttons button)
        {
            return gpState.IsButtonDown(button) && gpState.IsButtonUp(button);
        }
        public bool Released(Keys key)
        {
            return kbState.IsKeyUp(key) && oldKbState.IsKeyDown(key);
        }
        public bool Released(Buttons button)
        {
            return gpState.IsButtonUp(button) && gpState.IsButtonDown(button);
        }
        public bool Pressed(Keys key)
        {
            return kbState.IsKeyDown(key);
        }
        public bool Pressed(Buttons button)
        {
            return gpState.IsButtonDown(button);
        }
    }
}
