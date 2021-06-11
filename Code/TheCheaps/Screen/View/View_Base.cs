using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;
using TheCheaps.Scenes;
using TheCheapsLib;

namespace TheCheaps.Screen.View
{
    public abstract class View_Base
    {
        public Screen_Base ParentScreen { get; private set; }

        public Rectangle Rectangle;
        public Color BackgroundColor = new Color(0, 0, 0, 196);
        private Texture2D white_texture;
        public event EventHandler Accept;
        public event EventHandler Cancel;

        protected View_Base(Screen_Base screen,Rectangle rect)
        {
            this.ParentScreen = screen;
            this.Rectangle = rect;
        }
        public virtual void LoadContent(ContentManager content)
        {
            white_texture = content.Load<Texture2D>("menu/white_square");
        }
        public abstract void Update(GameTime gameTime);
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin( SpriteSortMode.Immediate);
            spriteBatch.Draw(white_texture, this.Rectangle, this.BackgroundColor);
            spriteBatch.End();
        }
        public abstract void Terminate(ContentManager content);
        protected void OnCancel()
        {
            if (Cancel != null)
                Cancel.Invoke(this, null);
        }

        protected void OnAccept()
        {
            if (Accept != null)
                Accept.Invoke(this, null);
        }
        internal void Center()
        {
            this.Rectangle.X = GraphicSettings.Bounds.Center.X - this.Rectangle.Width / 2;
            this.Rectangle.Y = GraphicSettings.Bounds.Center.Y - this.Rectangle.Height / 2;
        }
    }
}
