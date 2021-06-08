using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheaps.Scenes
{
    class Screen_Splash : Screen_MenuBase
    {
        public double duration_seconds = 3;
        public override string audio_name => "menu/splash_intro";
        public override string background_name => "menu/splash_background";
        public override bool audio_loop => false;

        public override void Update(GameTime gameTime)
        {
            duration_seconds -= gameTime.ElapsedGameTime.TotalSeconds;
            if (duration_seconds < 0)
                ScreenManager.Instance.ChangeScreen("Lobby");
        }
    }
}
