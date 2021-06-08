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

        SpriteFont font;
        List<string> textual_gui = new List<string>();
        public Screen_Lobby() { }
        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            font = content.Load<SpriteFont>("menu/lobby_font");
        }
        public override void Update(GameTime gameTime)
        {
            ScreenManager.Instance.ChangeScreen("game");
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.Begin();
            foreach (var line in textual_gui)
            { 
                //var size =  
            }
            spriteBatch.End();
        }
    }
}
