using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TheCheapsLib;

namespace TheCheaps
{
    public class Game1 : Game
    {
        private Task serverTask;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private NetworkClient client;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            serverTask = System.Threading.Tasks.Task.Factory.StartNew(runServer);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        private void runServer()
        {
            var server = new TheCheapsServer.NetworkServer();
            server.Start();
            var msperstep = 8;
            while (true)
            {
                //GESTIONE DEL CLOCK BASILARE
                var ms = DateTime.Now.Ticks / 10000;
                server.Tick();
                var newms = DateTime.Now.Ticks / 10000;
                var elapsedms = newms - ms;
                while (elapsedms < msperstep)
                {
                    System.Threading.Thread.Yield();
                    System.Threading.Thread.Sleep(1);
                    newms = DateTime.Now.Ticks / 10000;
                    elapsedms = newms - ms;
                }
            }
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            client = new NetworkClient(this);
            Components.Add(client);
            base.Initialize();
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            _spriteBatch.Begin();
            foreach(var entity in client.simulation.model.entities)
            {
                if(entity.sourcerect.Width == 0)
                    _spriteBatch.Draw(entity.texture, entity.posxy, null, Color.White, 0, entity.origin, 1, SpriteEffects.None, entity.z);
                else
                    _spriteBatch.Draw(entity.texture, entity.posxy, entity.sourcerect, Color.White, 0, entity.origin, 1, SpriteEffects.None, entity.z);
            }
            foreach (var entity in client.simulation.model.player_entities)
            {
                if (entity.sourcerect.Width == 0)
                    _spriteBatch.Draw(entity.texture, entity.posxy, null, Color.White, 0, entity.origin, 1, SpriteEffects.None, entity.z);
                else
                    _spriteBatch.Draw(entity.texture, entity.posxy, entity.sourcerect, Color.White, 0, entity.origin, 1, SpriteEffects.None, entity.z);
            }
            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        protected override void OnExiting(object sender, EventArgs args)
        {            
            base.OnExiting(sender, args);
        }
    }
}
