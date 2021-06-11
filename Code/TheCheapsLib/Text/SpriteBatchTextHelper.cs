using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public static class SpriteBatchTextHelper
    {
        public static void DrawString(this SpriteBatch spriteBatch,SpriteFont font, string text, Vector2 pos, Color color, bool outline, bool shadow,float scale = 1)
        {
            var size = font.MeasureString(text);
            if (shadow)
            {
                var shadowOffset = 4;
                var shadowColor = Color.Black;
                spriteBatch.DrawString(font, text, pos + Vector2.One * shadowOffset, shadowColor, 0, size / 2, scale, SpriteEffects.None, 0.5f);
            }
            else if (outline)
            {
                var outlineOffset = 2;
                var outlineColor = Color.Black;
                spriteBatch.DrawString(font, text, pos + new Vector2(+1, +1) * outlineOffset, outlineColor, 0, size / 2, scale, SpriteEffects.None, 0.7f);
                spriteBatch.DrawString(font, text, pos + new Vector2(+1, -1) * outlineOffset, outlineColor, 0, size / 2, scale, SpriteEffects.None, 0.7f);
                spriteBatch.DrawString(font, text, pos + new Vector2(-1, +1) * outlineOffset, outlineColor, 0, size / 2, scale, SpriteEffects.None, 0.7f);
                spriteBatch.DrawString(font, text, pos + new Vector2(-1, -1) * outlineOffset, outlineColor, 0, size / 2, scale, SpriteEffects.None, 0.7f);
            }
            spriteBatch.DrawString(font, text, pos, color, 0, size / 2, scale, SpriteEffects.None, 1f);
        }
    }
}
