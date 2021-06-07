﻿#define _TEST﻿
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using TheCheapsLib;

namespace TheCheaps
{
    public class Game1 : Game
    {
        private Process server_process;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
#if !TEST
        private NetworkClient client;
#endif
        private KeyboardState oldstate;
        public Game1()
        {
            server_process = Process.Start(@"TheCheapsServer.exe");
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
#if !TEST
            // TODO: Add your initialization logic here
            client = new NetworkClient(this);
            Components.Add(client);
#endif
            base.Initialize();
        }

        protected override void LoadContent()
        {

            load_entities();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        private void load_entities()
        {
#if TEST
            GameSimulation.Stop();
            GameSimulation.Start();
            foreach (var entity in SimulationModel.entities)
            {
                entity.texture = Content.Load<Texture2D>(entity.texture_path);
            }
#endif
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            // TODO: Add your update logic here

            var state = Keyboard.GetState();
            if (state.IsKeyUp(Keys.F5) && oldstate.IsKeyDown(Keys.F5))
                load_entities();

            base.Update(gameTime);
            foreach (var entity in SimulationModel.entities)
            {
                if (entity.texture == null)
                {
                    var tex = Content.Load<Texture2D>(entity.texture_path);
                    entity.texture = tex;
                    entity.origin = new Vector2(tex.Width / 2, tex.Height);
                }
            }
            oldstate = state;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            _spriteBatch.Begin();
            foreach(var entity in SimulationModel.entities)
            {
                if(entity.sourcerect.Width == 0)
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
            server_process.Kill(true);
            base.OnExiting(sender, args);
        }
    }
}
