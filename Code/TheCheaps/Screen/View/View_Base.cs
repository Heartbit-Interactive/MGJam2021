using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using TheCheaps.Scenes;

namespace TheCheaps.Screen.View
{
    public abstract class View_Base
    {
        public Screen_Base ParentScreen { get; private set; }

        public Rectangle Rectangle;
        public Color BackgroundColor = new Color(0, 0, 0, 196);
        private Texture2D white_texture;

        protected View_Base(Screen_Base screen,Rectangle rect)
        {
            this.ParentScreen = screen;
            this.Rectangle = rect;
        }
        public virtual void LoadContent(ContentManager content)
        {
            white_texture = content.Load<Texture2D>("menu/white_square");
        }
        public abstract void Update(GameTime gameTime);
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin( SpriteSortMode.Immediate);
            spriteBatch.Draw(white_texture, this.Rectangle, this.BackgroundColor);
            spriteBatch.End();
        }
        public abstract void Terminate(ContentManager content);

        internal static void DrawString(SpriteFont font, SpriteBatch spriteBatch, string text, Vector2 pos, Color color,bool outline,bool shadow)
        {
            var size = font.MeasureString(text);
            if (shadow)
            {
                var shadowOffset = 4;
                var shadowColor = Color.Black;
                spriteBatch.DrawString(font, text, pos + Vector2.One * shadowOffset, shadowColor, 0, size / 2, 1, SpriteEffects.None, 1f);
            }
            else if (outline)
            {
                var outlineOffset = 2;
                var outlineColor = Color.Black;
                spriteBatch.DrawString(font, text, pos + new Vector2(+1, +1) * outlineOffset, outlineColor, 0, size / 2, 1, SpriteEffects.None, 1f);
                spriteBatch.DrawString(font, text, pos + new Vector2(+1, -1) * outlineOffset, outlineColor, 0, size / 2, 1, SpriteEffects.None, 1f);
                spriteBatch.DrawString(font, text, pos + new Vector2(-1, +1) * outlineOffset, outlineColor, 0, size / 2, 1, SpriteEffects.None, 1f);
                spriteBatch.DrawString(font, text, pos + new Vector2(-1, -1) * outlineOffset, outlineColor, 0, size / 2, 1, SpriteEffects.None, 1f);
            }
            spriteBatch.DrawString(font, text, pos, color, 0, size / 2, 1, SpriteEffects.None, 1f);
        }
    }
}
