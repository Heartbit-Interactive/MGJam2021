using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using TheCheapsLib;
using TheCheapsServer;

namespace TheCheaps.Scenes
{
    class Screen_Lobby : Screen_MenuBase
    {
        public override string audio_name => "menu/lobby_loop";
        public override string background_name => "menu/lobby_background";        
        public override bool audio_loop => true;

        SpriteFont font;
        List<MenuOption> textual_gui = new List<MenuOption>();
        struct MenuOption
        {
            public string text;
            public bool enabled;

            public MenuOption(string v1, bool v2)
            {
                this.text = v1;
                this.enabled = v2;
            }
        }
        public Screen_Lobby()
        {
            NetworkManager.StartServer();
            textual_gui.Add(new MenuOption($"Ip: {NetworkServer.GetLocalIPAddress()}", false));
            textual_gui.Add(new MenuOption("Join", true));
            textual_gui.Add(new MenuOption("", false));
            textual_gui.Add(new MenuOption("Players", false));
            textual_gui.Add(new MenuOption("You (Host)", false));
            textual_gui.Add(new MenuOption("-", false));
            textual_gui.Add(new MenuOption("-", false));
            textual_gui.Add(new MenuOption("-", false));

        }
        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            font = content.Load<SpriteFont>("menu/lobby_font");
        }
        public override void Update(GameTime gameTime)
        {
            var kbstate = Keyboard.GetState();
            ScreenManager.Instance.ChangeScreen("game");           
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            var c = GraphicSettings.Bounds.Center;
            Vector2 position = new Vector2(c.X,c.Y/2);
            var lineHeight = 32;
            spriteBatch.Begin();
            for (int index = 0;index<textual_gui.Count;index++)
            {
                var option = textual_gui[index];
                var size = font.MeasureString(option.text);
                spriteBatch.DrawString(font, option.text, position + index * lineHeight * Vector2.UnitY, option.enabled ? Color.White : Color.DarkSlateGray,0,size/2,1,SpriteEffects.None,1f);
            }
            spriteBatch.End();
        }
    }
}
