#define TEST
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TheCheapsServer;

namespace TheCheaps.Scenes
{
    class Screen_Game : Screen_Base
    {
        private NetworkClient client;
        private NetworkServer server;
        private Game1 game;
        public Screen_Game(Game1 game)
        {
            this.game = game;
#if TEST
            start_server();
#endif
            client = new NetworkClient(game);
        }
        private ContentManager Content;
        public override void LoadContent(ContentManager content)
        {
            this.Content = content;
        }
        public override void Update(GameTime gameTime)
        {
            client.Update(gameTime);
        }

        Color backgroundColor = new Color(191 ,149 ,77);
        public override void Draw(SpriteBatch spriteBatch)
        {
            refresh_entity_textures();
            game.GraphicsDevice.Clear(backgroundColor);
            spriteBatch.Begin();
            foreach (var entity in client.simulation.model.entities)
            {
                if (entity.sourcerect.Width == 0)
                    spriteBatch.Draw(entity.texture, entity.posxy, null, Color.White, 0, entity.origin, 1, SpriteEffects.None, entity.z);
                else
                    spriteBatch.Draw(entity.texture, entity.posxy, entity.sourcerect, Color.White, 0, entity.origin, 1, SpriteEffects.None, entity.z);
            }
            foreach (var entity in client.simulation.model.player_entities)
            {
                if (entity.sourcerect.Width == 0)
                    spriteBatch.Draw(entity.texture, entity.posxy, null, Color.White, 0, entity.origin, 1, SpriteEffects.None, entity.z);
                else
                    spriteBatch.Draw(entity.texture, entity.posxy, entity.sourcerect, Color.White, 0, entity.origin, 1, SpriteEffects.None, entity.z);
            }
            spriteBatch.End();
        }

        private void refresh_entity_textures()
        {
            foreach (var entity in client.simulation.model.entities)
            {
                if (entity.texture == null)
                {
                    var tex = Content.Load<Texture2D>(entity.texture_path);
                    entity.texture = tex;
                    entity.origin = new Vector2(tex.Width / 2, tex.Height);
                }
            }
            foreach (var entity in client.simulation.model.player_entities)
            {
                if (entity.texture == null)
                    entity.texture = Content.Load<Texture2D>(entity.texture_path);
            }
        }
        public override void Terminate(ContentManager content)
        {
#if TEST
            serverCancellation.Cancel();
#endif
        }

#if TEST
        private CancellationTokenSource serverCancellation;
        private void start_server()
        {
            serverCancellation = new CancellationTokenSource();
            var ct = serverCancellation.Token;
            System.Threading.Tasks.Task.Factory.StartNew(()=>runServer(serverCancellation.Token), ct);
            while (server == null || !server.Started)
            {
                System.Threading.Thread.Yield();
                System.Threading.Thread.Sleep(1);
            }
        }
        private void runServer(CancellationToken ctoken)
        {
            if (ctoken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }
            server = new TheCheapsServer.NetworkServer();
            server.Start();
            var msperstep = 8;
            while (true)
            {
                //GESTIONE DEL CLOCK BASILARE
                var ms = DateTime.Now.Ticks / 10000;
                server.Tick();
                var newms = DateTime.Now.Ticks / 10000;
                var elapsedms = newms - ms;
                if (ctoken.IsCancellationRequested)
                {
                    server.Stop("Task Cancelled");
                    server.Dispose();
                    throw new TaskCanceledException();
                }
                while (elapsedms < msperstep)
                {
                    System.Threading.Thread.Yield();
                    System.Threading.Thread.Sleep(1);
                    if (ctoken.IsCancellationRequested)
                    {
                        server.Stop("Task Cancelled");
                        server.Dispose();
                        throw new TaskCanceledException();
                    }
                    newms = DateTime.Now.Ticks / 10000;
                    elapsedms = newms - ms;
                }
            }
        }
#endif
    }
}
