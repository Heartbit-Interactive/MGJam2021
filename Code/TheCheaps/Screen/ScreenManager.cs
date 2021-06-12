using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheaps.Scenes
{
    /// <summary>
    /// Singleton class gamecomponent
    /// </summary>
    class ScreenManager : DrawableGameComponent
    {
        public static ScreenManager Instance;
        public Screen_Base Screen;
        ContentManager _content;
        SpriteBatch _spriteBatch;
        public ScreenManager(Game game):base(game)
        {
            if (Instance != null)
                throw new InvalidOperationException("Only one instance is allowed");
            Instance = this;
            _content = game.Content;
            _spriteBatch = game.Services.GetService<SpriteBatch>();
        }
        internal void ChangeScreen(string screen_name)
        {
            if(Screen!=null)
            Screen.Terminate(_content);
            switch (screen_name.ToLowerInvariant())
            {
                case "splash":
                    Screen = new Screen_Splash();
                    break;
                case "lobby":
                    Screen = new Screen_Lobby();
                    break;
                case "game":
                    Screen = new Screen_Game((Game1)Game);
                    break;
            }
            Screen.LoadContent(_content);
        }
        public override void Initialize()
        {
            Screen = new Screen_Splash();
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            if (Screen != null) 
                Screen.Update(gameTime);
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            if (Screen != null)
            {
                Screen.Draw(_spriteBatch);
                Screen.EndDraw(_spriteBatch);
            }
            base.Draw(gameTime);
        }
        protected override void Dispose(bool disposing)
        {
            if (Screen != null)
                Screen.Terminate(_content);
            base.Dispose(disposing);
        }
    }
}
