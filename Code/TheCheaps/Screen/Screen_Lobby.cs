using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
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
        int menuIndex = 2;
        bool shadow = false;
        bool outline = true;
        int extra_lineHeight = 8;
#if DEBUG
        public override string audio_name => null;
#else
        public override string audio_name => "menu/lobby_loop";
#endif
        public override string background_name => "menu/lobby_background";        
        public override bool audio_loop => true;

        SpriteFont font;
        List<MenuOption> textual_gui = new List<MenuOption>();
        private MenuOption host_option;
        private MenuOption join_option;
        private MenuOption status_option;
        private MenuOption[] player_option;
        private bool isServer;
        private bool isClient;
        private MenuOption status_option2;

        class MenuOption
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
            setDefault();
        }

        private void setDefault()
        {
            textual_gui.Clear();
            textual_gui.Add(status_option = new MenuOption("-status-", false));
            textual_gui.Add(status_option2 = new MenuOption("", false));
            textual_gui.Add(host_option = new MenuOption("Host", true));
            textual_gui.Add(join_option = new MenuOption("Join", true));
            textual_gui.Add(new MenuOption("", false));
            textual_gui.Add(new MenuOption("-Players-", false));
            player_option = new MenuOption[Settings.maxPlayers];
            for (int i = 0; i < Settings.maxPlayers; i++)
            {
                textual_gui.Add(player_option[i] = new MenuOption("-", false));
            }
            isServer = false;
            isClient = false;
        }
        private void SetClient()
        {
            isServer = false;
            isClient = true;
            host_option.text = "Disconnect";
            refresh_ClientView();
        }

        private void refresh_ClientView()
        {
            if (isServer)
                return;
            if (NetworkManager.Client == null)
            {
                join_option.text = "Joining...";
                return;
            }
            try
            {
                if (NetworkManager.Client.Status != Lidgren.Network.NetPeerStatus.Running)
                    join_option.text = $"{NetworkManager.Client.Status}...";
                else
                {
                    var pi = NetworkManager.Client.PlayerIndex;
                    if (pi < 0)
                        join_option.text = "Awaiting State";
                    else if (NetworkManager.Client.GetReady(pi))
                        join_option.text = "Ready [V]";
                    else
                        join_option.text = "Ready [ ]";
                    join_option.enabled = pi >= 0;
                }

            }
            catch(Exception e)
            {
                join_option.text = "Error...";
            }
        }

        private void SetServer()
        {
            isServer = true;
            isClient = false;
            if (NetworkManager.Client.network.model.serverState.ReadyToStart)
            {
                host_option.enabled = true;
                host_option.text = "Start!";
            }
            else
            {
                var n = NetworkManager.Client.network.model.players.Count(x => x != null && !x.Ready);
                host_option.enabled = false;
                host_option.text = $"Waiting for {n} players to be ready!";
            }
            join_option.text = "Stop";
            join_option.enabled = true;
            switch (NetworkManager.ServerStatus)
            {
                case Lidgren.Network.NetPeerStatus.NotRunning:
                    status_option.text = $"[Hosting] Not running";
                    break;
                case Lidgren.Network.NetPeerStatus.Starting:
                    status_option.text = $"[Hosting] Starting";
                    break;
                case Lidgren.Network.NetPeerStatus.Running:
                    status_option.text = $"[Hosting] Public Ip: {NetworkManager.PublicIp}:{NetworkManager.Port}";
                    status_option2.text = $"Local Ip: {NetworkManager.LocalIp}";
                    break;
                case Lidgren.Network.NetPeerStatus.ShutdownRequested:
                    status_option.text = $"[Hosting] Shutdown Requested";
                    break;
                default:
                    break;
            }
        }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content);
            font = content.Load<SpriteFont>("menu/lobby_font");
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (view_stack.Count != 0)
                return;
            if (input_sleep > 0)
            {
                input_sleep--;
                return;
            }
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
                            view.Accept += Join;
                            view.Cancel += CloseView;
                            AddView(view);
                        }
                        break;
                    case "disconnect":
                        {
                            SoundManager.PlayDecision();
                            Disconnect();
                        }
                        break;
                    case "host":
                        {
                            SoundManager.PlayDecision();
                            var view = new View_InputPort(this, new Rectangle(0, 0, 640, 256));
                            view.Center();
                            view.Accept += Host;
                            view.Cancel += CloseView;
                            AddView(view);
                        }
                        break;
                    case "start!":
                        {
                            SoundManager.PlayDecision();
                            NetworkManager.StartCountDown();
                        }
                        break;
                    case "stop":
                        {
                            Disconnect();
                            NetworkManager.StopServer();
                            setDefault();
                        }
                        break;
                    case "ready [ ]":
                        {
                            SoundManager.PlayDecision();
                            SetReady(true);
                        }
                        break;
                    case "ready [v]":
                        {
                            SoundManager.PlayDecision();
                            SetReady(false);
                        }
                        break;
                }
            }
            if (isServer)
                SetServer();
            if (NetworkManager.Client != null)
            {
                if (NetworkManager.Client.network.model.serverState.CountDown > 0)
                    SetStarting();
                if (NetworkManager.Client.network.model.serverState.GamePhase == NetworkServerState.Phase.Gameplay)
                {
                    ScreenManager.Instance.ChangeScreen("game");
                }
            }
            for (int i = 0; i < Settings.maxPlayers; i++)
            {
                if (NetworkManager.Client == null)
                {
                    player_option[i].text = "-";
                    continue;
                }
                var pl = NetworkManager.Client.network.model.players[i];
                if (pl == null)
                    player_option[i].text = "-";
                else
                    player_option[i].text = pl.ToString();
            }
        }

        private void SetStarting()
        {
                join_option.enabled = true;
                join_option.text = $"Cancel";

            host_option.enabled = false;
            host_option.text = $"Starting in {Math.Ceiling(NetworkManager.Client.network.model.serverState.CountDown)}!";
        }

        private void Disconnect()
        {
            NetworkManager.StopClient();
            setDefault();
        }

        private void SetReady(bool v)
        {
            NetworkManager.Client.SetReady(v);
            refresh_ClientView();
        }

        private void Host(object sender, EventArgs e)
        {
            var view = ((View_InputPort)sender);
            BeginHostPort(view.Port); 
            RemoveViewLayer();
        }

        private void CloseView(object sender, EventArgs e)
        {
            RemoveViewLayer();
        }

        private void Join(object sender, EventArgs e)
        {
            var view = ((View_InputIp)sender);
            SetClient();
            BeginJoinIp(view.Ip, view.Port); 
            RemoveViewLayer();
        }


        private void BeginHostPort(int port)
        {
            host_option.enabled = false;
            join_option.enabled = false;
            host_option.text = "Starting Up";
            if (NetworkManager.ServerRunning)
                NetworkManager.StopServer();
            NetworkManager.BeginHost(port);
            while (NetworkManager.ServerStatus == Lidgren.Network.NetPeerStatus.Starting)
            {
                System.Threading.Thread.Sleep(1);
            }
            WhatsMyIp.GetMyIpAsync().ContinueWith(publicIpReceived);
            NetworkManager.BeginJoin(new IPAddress(new byte[] { 127, 0, 0, 1 }), NetworkManager.Port);
            NetworkManager.Client.StateChanged += Client_StateChanged;            
        }
        private void publicIpReceived(Task<IPAddress> task)
        {
            if (!task.IsCompletedSuccessfully || task.Result == null)
            {
                NetworkManager.PublicIp = null;
                AddView(new View_Message($"[Error] Invalid State",this));
            }
            else
            {
                NetworkManager.PublicIp = task.Result;
                SetServer();
            }
        }


        private void BeginJoinIp(IPAddress ip, int port)
        {
            if (NetworkManager.ServerRunning)
                NetworkManager.StopServer();
            NetworkManager.BeginJoin(ip, port);
            NetworkManager.Client.StateChanged += Client_StateChanged;
        }

        private void Client_StateChanged(object sender, EventArgs e)
        {
            refresh_ClientView();
            if (NetworkManager.Client == null)
                return;
            if (!isServer)
                return;
            NetworkManager.Client.SetReady(true);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            var c = GraphicSettings.Bounds.Center;
            Vector2 position = new Vector2(c.X,32);
            spriteBatch.Begin();
            for (int index = 0;index<textual_gui.Count;index++)
            {
                var option = textual_gui[index];
                var text = option.text;
                if (index == menuIndex)
                    text = $"<{text}>";
                var pos = position + index * (extra_lineHeight + 44) * Vector2.UnitY;
                View_Base.DrawString(font,spriteBatch,  text, pos, option.enabled ? Color.White : Color.Gray,outline,false);
            }
            spriteBatch.End();
        }
    }
}
