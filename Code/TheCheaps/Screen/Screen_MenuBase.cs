using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using TheCheaps.Screen.View;
using TheCheapsLib;

namespace TheCheaps.Scenes
{
    abstract class Screen_MenuBase : Screen_Base
    {
        public abstract string audio_name { get; }
        public abstract string background_name { get; }
        public abstract bool audio_loop { get; }

        SoundEffectInstance audio;
        Texture2D background;
        public override void LoadContent(ContentManager content)
        {
            var se = content.Load<SoundEffect>(audio_name);
            audio = se.CreateInstance();
            audio.Volume = 0.75f;
            audio.IsLooped = audio_loop;
            audio.Play();
            background = content.Load<Texture2D>(background_name);
        }
        public Stack<View_Base[]> view_stack = new Stack<View_Base[]>();
        public void AddView(params View_Base[] views)
        {
            foreach (var view in views)
                view.LoadContent(ScreenManager.Instance.Game.Content);
            view_stack.Push(views);
        }
        public void RemoveViewLayer()
        {
            var views = view_stack.Pop();
            foreach (var view in views)
                view.Terminate(ScreenManager.Instance.Game.Content);
        }

        public override void Update(GameTime gameTime)
        {
            RefreshInputState();
            if (view_stack.Count > 0)
            {
                var views = view_stack.Peek();
                foreach (var view in views)
                    view.Update(gameTime);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(background, GraphicSettings.Bounds, Color.White);
            spriteBatch.End();
        }

        public override void EndDraw(SpriteBatch spriteBatch)
        {
            if (view_stack.Count > 0)
            {
                var views = view_stack.Peek();
                foreach (var view in views)
                    view.Draw(spriteBatch);
            }
        }
        public override void Terminate(ContentManager content)
        {
            audio.Stop();
            audio = null;
            background = null;
        }
    }
}
