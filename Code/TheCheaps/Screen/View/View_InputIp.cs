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
        public int[] numbers = new int[5] { 127, 0, 0, 1, 12345 };
        public float[] float_numbers = new float[5] { 127, 0, 0, 1, 12345 };
        float speed, start_speed = 0.05f;
        public int[] number_limits = new int[5] { 256, 256, 256, 256, 1000000 };
        public string[] texts = new string[2]{
            "Please enter Host Ip and Port:",
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

            else if (ParentScreen.Trigger(Keys.Down) || ParentScreen.Trigger(Buttons.LeftThumbstickDown) || ParentScreen.Trigger(Buttons.DPadDown))
            {
                change_value(menu_index, -1);
                float_numbers[menu_index] = numbers[menu_index];
            }
            else if (ParentScreen.Trigger(Keys.Up) || ParentScreen.Trigger(Buttons.LeftThumbstickUp) || ParentScreen.Trigger(Buttons.DPadUp))
            {
                change_value(menu_index, +1);
                float_numbers[menu_index] = numbers[menu_index];
            }
            else if (ParentScreen.Press(Keys.Down) || ParentScreen.Press(Buttons.LeftThumbstickDown) || ParentScreen.Press(Buttons.DPadDown))
            {
                change_value(menu_index, -speed);
                speed = Math.Min(speed + Math.Max(0.01f, speed / 10f), menu_index == 4 ? 100:1) ;
            }
            else if (ParentScreen.Press(Keys.Up) || ParentScreen.Press(Buttons.LeftThumbstickUp) || ParentScreen.Press(Buttons.DPadUp))
            {
                change_value(menu_index, +speed);
                speed = Math.Min(speed + Math.Max(0.01f,speed/10f), menu_index == 4 ? 100 : 1);
            }
            else
                speed = start_speed;

            if (ParentScreen.Trigger(Buttons.A) || ParentScreen.Trigger(Keys.Enter) || ParentScreen.Trigger(Keys.Z))
                OnAccept();
            else if (ParentScreen.Trigger(Buttons.B) || ParentScreen.Trigger(Keys.Escape) || ParentScreen.Trigger(Keys.X))
                OnCancel();
            if (ParentScreen.Press(Keys.LeftControl) && ParentScreen.Trigger(Keys.V))
            {
                string pasted_text = new TextCopy.Clipboard().GetText();
                var split = pasted_text.Split('.', ':');
                if (split.Length == 5)
                {
                    for (int i = 0; i < 5; i++)
                        int.TryParse(split[i],out numbers[i]);
                }
            }

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

        private void change_value(int menu_index, float speed)
        {
            float_numbers[menu_index] = (number_limits[menu_index] + float_numbers[menu_index] + speed+0.001f) % number_limits[menu_index];
            numbers[menu_index] = (int)Math.Round(float_numbers[menu_index]);
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
