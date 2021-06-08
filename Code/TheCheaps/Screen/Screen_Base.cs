using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
    }
}
