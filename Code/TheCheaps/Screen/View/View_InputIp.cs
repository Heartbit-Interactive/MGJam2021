using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using TheCheaps.Scenes;
using TheCheapsLib;

namespace TheCheaps.Screen.View
{
    class View_InputIp : View_Base
    {
        private SpriteFont font;
        public int[] numbers = new int[4];
        public string[] texts = new string[2]{
            "Please enter Host Ip:",
            "" };
        public View_InputIp(Screen_Base screen, Rectangle rect) : base(screen,rect)
        {
        }
        public override void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("menu/lobby_font");
        }
        int menu_index = 0;
        public override void Update(GameTime gameTime)
        {
            if (ParentScreen.Trigger(Keys.Left) || ParentScreen.Trigger(Buttons.LeftThumbstickLeft) || ParentScreen.Trigger(Buttons.DPadLeft))
                menu_index = (4 + menu_index - 1) % 4;
            else if (ParentScreen.Trigger(Keys.Right) || ParentScreen.Trigger(Buttons.LeftThumbstickRight) || ParentScreen.Trigger(Buttons.DPadRight))
                menu_index = (4 + menu_index + 1) % 4;
            if (menu_index == 0)
                texts[1] = $"<{numbers[0]}>.{numbers[1]}.{numbers[2]}.{numbers[3]}";
            else if (menu_index == 1)
                texts[1] = $"{numbers[0]}.<{numbers[1]}>.{numbers[2]}.{numbers[3]}";
            else if (menu_index == 2)
                texts[1] = $"{numbers[0]}.{numbers[1]}.<{numbers[2]}>.{numbers[3]}";
            else if (menu_index == 3)
                texts[1] = $"{numbers[0]}.{numbers[1]}.{numbers[2]}.<{numbers[3]}>";
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            var c = GraphicSettings.Bounds.Center;
            Vector2 position = new Vector2(c.X, c.Y / 2);
            spriteBatch.Begin();
            for (int index = 0; index < texts.Length; index++)
            {
                var pos = position + Vector2.UnitY * 36 * index;
                View_Base.DrawString(font, spriteBatch, texts[index], pos, Color.Gray, true, false);
            }
            spriteBatch.End();
        }
        public override void Terminate(ContentManager content)
        {
            throw new NotImplementedException();
        }
    }
}
