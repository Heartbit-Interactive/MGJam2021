using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheaps.Scenes
{
    /// <summary>
    /// Singleton class gamecomponent
    /// </summary>
    class ScreenManager : GameComponent
    {
        public static ScreenManager Instance;
        public Screen_Base screen;
        public ScreenManager(Game game):base(game)
        {
            if (Instance != null)
                throw new InvalidOperationException("Only one instance is allowed");
            Instance = this;
        }

        internal void ChangeScreen(string v)
        {
            throw new NotImplementedException();
        }

        public override void Initialize()
        {
            screen = new Screen_Title();
            base.Initialize();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnEnabledChanged(object sender, EventArgs args)
        {
            base.OnEnabledChanged(sender, args);
        }

        protected override void OnUpdateOrderChanged(object sender, EventArgs args)
        {
            base.OnUpdateOrderChanged(sender, args);
        }
    }
}
