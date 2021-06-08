using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheaps.Scenes
{
    class Screen_Lobby : Screen_MenuBase
    {
        public override string audio_name => "menu/lobby_loop";
        public override string background_name => "menu/lobby_background";        
        public override bool audio_loop => true;

        public Screen_Lobby() { }
        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
