using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TheCheaps.Screen.View;
using TheCheapsLib;
using TheCheapsServer;

namespace TheCheaps.Scenes
{
    class Screen_Lobby : Screen_MenuBase
    {
        int menuIndex = 4;
        bool shadow = false;
        bool outline = true;
        int extra_lineHeight = 8;
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
            //textual_gui.Add(new MenuOption($"Local Ip: {NetworkServer.GetLocalIPAddress()}", false));
            //textual_gui.Add(new MenuOption($"STATUS", false));
            //textual_gui.Add(new MenuOption($"Port: {NetworkManager.Port}", true));
            textual_gui.Add(new MenuOption("Host", true));
            textual_gui.Add(new MenuOption("Join", true));
            textual_gui.Add(new MenuOption("Start", false));
            textual_gui.Add(new MenuOption("", false));
            textual_gui.Add(new MenuOption("Players", false));
            textual_gui.Add(new MenuOption("You (Host)", false));
            for (int i = 1; i < Settings.maxPlayers; i++)
            {
                textual_gui.Add(new MenuOption("-", false));
            }
            NetworkManager.StartServer();
        }

        private void publicIpReceived(Task<IPAddress> arg1)
        {
            var gui = textual_gui[1];
            if (!arg1.IsCompletedSuccessfully || arg1.Result == null)
            {
                NetworkManager.PublicIp = null;
                gui.text = $"[Error] Invalid State";
            }
            else
            {
                NetworkManager.PublicIp = arg1.Result;
                gui.text = $"[Hosting] Public Ip: {arg1.Result}"; 
            }
            textual_gui[1] = gui;
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            font = content.Load<SpriteFont>("menu/lobby_font");
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (view_stack.Count == 0)
            {
                if (Trigger(Buttons.LeftThumbstickDown) || Trigger(Buttons.DPadDown) || Trigger(Keys.Down))
                {
                    menuIndex = (textual_gui.Count + menuIndex + 1) % textual_gui.Count;
                    SoundManager.PlayCursors();
                }
                if (Trigger(Buttons.LeftThumbstickUp) || Trigger(Buttons.DPadUp) || Trigger(Keys.Up))
                {
                    menuIndex = (textual_gui.Count + menuIndex - 1) % textual_gui.Count;
                    SoundManager.PlayCursors();
                }
                if (Trigger(Buttons.A) || Trigger(Keys.Enter))
                {
                    var command = textual_gui[menuIndex];
                    if (!command.enabled)
                        SoundManager.PlayBuzzer();
                    switch (command.text.ToLowerInvariant())
                    {
                        case "join":
                            {
                                SoundManager.PlayDecision();
                                var view = new View_InputIp(this, new Rectangle(0, 0, 640, 256));
                                view.Center();
                                view.Accept += (s, a) => { BeginJoinIp(view.Ip, view.Port); RemoveViewLayer(); };
                                view.Cancel += (s, a) => { RemoveViewLayer(); };
                                AddView(view);
                            }
                            break;
                        case "host":
                            {
                                SoundManager.PlayDecision();
                                var view = new View_InputPort(this, new Rectangle(0, 0, 640, 256));
                                view.Center();
                                view.Accept += (s, a) => { BeginHostPort(view.Port); RemoveViewLayer(); };
                                view.Cancel += (s, a) => { RemoveViewLayer(); };
                                AddView(view);
                            }
                            break;
                    }
                }
            }
        }

        private void BeginHostPort(int port)
        {
            if (NetworkManager.ServerRunning)
                NetworkManager.StopServer();
            NetworkManager.BeginHost(port);
        }

        private void BeginJoinIp(IPAddress ip, int port)
        {
            if (NetworkManager.ServerRunning)
                NetworkManager.StopServer();
            NetworkManager.BeginJoin(ip, port);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            var c = GraphicSettings.Bounds.Center;
            Vector2 position = new Vector2(c.X,c.Y/2);
            spriteBatch.Begin();
            for (int index = 0;index<textual_gui.Count;index++)
            {
                var option = textual_gui[index];
                var text = option.text;
                if (index == menuIndex)
                    text = $"<{text}>";
                var pos = position + index * (extra_lineHeight + 36) * Vector2.UnitY;
                View_Base.DrawString(font,spriteBatch,  text, pos, option.enabled ? Color.White : Color.Gray,outline,false);
            }
            spriteBatch.End();
        }
    }
}
