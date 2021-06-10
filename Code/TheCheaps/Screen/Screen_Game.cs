#define TEST
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheCheapsLib;
using TheCheapsServer;

namespace TheCheaps.Scenes
{
    class Screen_Game : Screen_Base
    {
        private Game1 game;
        public Screen_Game(Game1 game)
        {
            this.game = game;
#if TEST
            NetworkManager.StartServer();
            NetworkManager.BeginJoin(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), 12345);
            NetworkManager.Client.StateChanged += Client_StateChanged;
        }

        private void Client_StateChanged(object sender, EventArgs e)
        {
            NetworkManager.Client.SetReady(true);
        }

#else
        }
#endif
        private List<PlayerEntity> gui_entities;
        private ContentManager Content;
        public override void LoadContent(ContentManager content)
        {
            var jsontextgui = File.ReadAllText("GUI.json");
            gui_entities = JsonConvert.DeserializeObject<List<PlayerEntity>>(jsontextgui);
            this.Content = content;
        }
        public override void Update(GameTime gameTime)
        {
#if TEST
            if (NetworkManager.Client.network.model.serverState.GamePhase != TheCheapsLib.NetworkServerState.Phase.Gameplay)
            {
                var pls = NetworkManager.Client.network.model.players;
                if (pls.Any(x => x != null) && pls.All(p => p == null || p.Ready))
                    NetworkManager.Server.StartMatch();
            }
#endif
        }

        Color backgroundColor = new Color(191 ,149 ,77);
        public override void Draw(SpriteBatch spriteBatch)
        {
            refresh_entity_textures();
            game.GraphicsDevice.Clear(backgroundColor);
            spriteBatch.Begin();
            foreach (var entity in NetworkManager.Client.simulation.model.entities)
            {
                entity.Draw(spriteBatch);
            }
            foreach (var entity in NetworkManager.Client.simulation.model.player_entities)
            {
                entity.Draw(spriteBatch);
            }
            foreach (var entity in gui_entities)
            {
                entity.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
        public override void EndDraw(SpriteBatch spriteBatch)
        {
        }
        private void refresh_entity_textures()
        {
            foreach (var entity in NetworkManager.Client.simulation.model.entities)
            {
                if (entity.texture == null)
                {
                    var tex = Content.Load<Texture2D>(entity.texture_path);
                    entity.texture = tex;
                    entity.origin = new Vector2(tex.Width / 2, tex.Height);
                }
            }
            foreach (var entity in gui_entities)
            {
                if (entity.texture == null)
                {
                    var tex = Content.Load<Texture2D>(entity.texture_path);
                    entity.texture = tex;
                    entity.origin = new Vector2(tex.Width / 2, tex.Height);
                }
            }
            foreach (var entity in NetworkManager.Client.simulation.model.player_entities)
            {
                if (entity.texture == null)
                {
                    var tex = Content.Load<Texture2D>(entity.texture_path);
                    entity.texture = tex;
                    entity.origin = new Vector2(tex.Width / 2, tex.Height);
                }
            }
        }
        public override void Terminate(ContentManager content)
        {
#if TEST
            NetworkManager.StopServer();
#endif
        }
    }
}
