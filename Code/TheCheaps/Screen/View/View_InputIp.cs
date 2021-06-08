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
    class View_InputIp : View_Base
    {
        private SpriteFont font;
        public int[] numbers = new int[5];
        public int[] number_limits = new int[5] { 255, 255, 255, 255, 999999 };
        public string[] texts = new string[2]{
            "Please enter Host Ip:",
            "" };

        public View_InputIp(Screen_Base screen, Rectangle rect) : base(screen,rect)
        {
        }
        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            font = content.Load<SpriteFont>("menu/lobby_font");
        }
        int menu_index = 0;

        public int Port { get { return numbers[4]; } }
        public IPAddress Ip { get { return new IPAddress(numbers.Take(4).Select(x=>(byte)x).ToArray()); } }

        public override void Update(GameTime gameTime)
        {
            if (ParentScreen.Trigger(Keys.Left) || ParentScreen.Trigger(Buttons.LeftThumbstickLeft) || ParentScreen.Trigger(Buttons.DPadLeft))
                menu_index = (5 + menu_index - 1) % 5;
            else if (ParentScreen.Trigger(Keys.Right) || ParentScreen.Trigger(Buttons.LeftThumbstickRight) || ParentScreen.Trigger(Buttons.DPadRight))
                menu_index = (5 + menu_index + 1) % 5;

            else if (ParentScreen.Press(Keys.Down) || ParentScreen.Press(Buttons.LeftThumbstickDown) || ParentScreen.Press(Buttons.DPadDown))
                numbers[menu_index] = (number_limits[menu_index] + numbers[menu_index] - 1) % number_limits[menu_index];
            else if (ParentScreen.Press(Keys.Up) || ParentScreen.Press(Buttons.LeftThumbstickUp) || ParentScreen.Press(Buttons.DPadUp))
                numbers[menu_index] = (number_limits[menu_index] + numbers[menu_index] + 1) % number_limits[menu_index];

            else if (ParentScreen.Press(Buttons.A) || ParentScreen.Press(Keys.Enter))
                OnAccept();
            else if (ParentScreen.Press(Buttons.B) || ParentScreen.Press(Keys.Back))
                OnCancel();

            if (menu_index == 0)
                texts[1] = $"<{numbers[0]}>.{numbers[1]}.{numbers[2]}.{numbers[3]}:{numbers[4]}";
            else if (menu_index == 1)
                texts[1] = $"{numbers[0]}.<{numbers[1]}>.{numbers[2]}.{numbers[3]}:{numbers[4]}";
            else if (menu_index == 2)
                texts[1] = $"{numbers[0]}.{numbers[1]}.<{numbers[2]}>.{numbers[3]}:{numbers[4]}";
            else if (menu_index == 3)
                texts[1] = $"{numbers[0]}.{numbers[1]}.{numbers[2]}.<{numbers[3]}>:{numbers[4]}";
            else if (menu_index == 4)
                texts[1] = $"{numbers[0]}.{numbers[1]}.{numbers[2]}.{numbers[3]}:<{numbers[4]}>";
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            Vector2 position = new Vector2(this.Rectangle.Center.X, this.Rectangle.Y );
            spriteBatch.Begin();
            for (int index = 0; index < texts.Length; index++)
            {
                var pos = position + Vector2.UnitY * 64 * (index+1);
                View_Base.DrawString(font, spriteBatch, texts[index], pos, Color.Gray, true, false);
            }
            spriteBatch.End();
        }
        public override void Terminate(ContentManager content)
        {
        }
    }
}
