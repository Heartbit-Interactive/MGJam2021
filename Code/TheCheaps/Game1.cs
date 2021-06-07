#define TEST﻿
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TheCheapsLib;

namespace TheCheaps
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D test_texture;
        private NetworkClient client;
        private KeyboardState oldstate;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            client = new NetworkClient(this);
            Components.Add(client);
            base.Initialize();
        }

        protected override void LoadContent()
        {

            load_entities();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            test_texture = Content.Load<Texture2D>("test");

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
            oldstate = state;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            _spriteBatch.Begin();
            foreach(var entity in SimulationModel.entities)
            {
                _spriteBatch.Draw(entity.texture, entity.posxy, entity.sourcerect, Color.White, 0, entity.origin, 1, SpriteEffects.None, entity.z);
            }
            _spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
