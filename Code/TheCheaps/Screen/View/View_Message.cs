using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using TheCheaps.Scenes;
using TheCheapsLib;

namespace TheCheaps.Screen.View
{
    class View_Message : View_Base
    {
        private SpriteFont font;
        public string[] texts = new string[] { };

        public View_Message(string text, Screen_Base screen) : base(screen, new Rectangle(0, 0, 640, 256))
        {
            this.texts = new string[] { text };
        }
        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            font = content.Load<SpriteFont>("menu/lobby_font");
        }
        public override void Update(GameTime gameTime)
        {
            if (ParentScreen.Trigger(Buttons.A) || ParentScreen.Trigger(Keys.Enter))
                OnAccept();
            else if (ParentScreen.Trigger(Buttons.B) || ParentScreen.Trigger(Keys.Back))
                OnCancel();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            Vector2 position = new Vector2(this.Rectangle.Center.X, this.Rectangle.Y );
            spriteBatch.Begin();
            for (int index = 0; index < texts.Length; index++)
            {
                var pos = position + Vector2.UnitY * 64 * (index+1);
                spriteBatch.DrawString(font, texts[index], pos, Color.Gray, true, false);
            }
            spriteBatch.End();
        }
        public override void Terminate(ContentManager content)
        {
        }
    }
}
