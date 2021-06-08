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
        private Game1 game;
        public Screen_Game(Game1 game)
        {
            this.game = game;
#if TEST
            NetworkManager.StartServer();
            NetworkManager.BeginJoin(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), 12345);
#endif
        }
        private ContentManager Content;
        public override void LoadContent(ContentManager content)
        {
            this.Content = content;
        }
        public override void Update(GameTime gameTime)
        {
            NetworkManager.Client.Update(gameTime);
        }

        Color backgroundColor = new Color(191 ,149 ,77);
        public override void Draw(SpriteBatch spriteBatch)
        {
            refresh_entity_textures();
            game.GraphicsDevice.Clear(backgroundColor);
            spriteBatch.Begin();
            foreach (var entity in NetworkManager.Client.simulation.model.entities)
            {
                if (entity.sourcerect.Width == 0)
                    spriteBatch.Draw(entity.texture, entity.posxy, null, Color.White, 0, entity.origin, 1, SpriteEffects.None, entity.z);
                else
                    spriteBatch.Draw(entity.texture, entity.posxy, entity.sourcerect, Color.White, 0, entity.origin, 1, SpriteEffects.None, entity.z);
            }
            foreach (var entity in NetworkManager.Client.simulation.model.player_entities)
            {
                if (entity.sourcerect.Width == 0)
                    spriteBatch.Draw(entity.texture, entity.posxy, null, Color.White, 0, entity.origin, 1, SpriteEffects.None, entity.z);
                else
                    spriteBatch.Draw(entity.texture, entity.posxy, entity.sourcerect, Color.White, 0, entity.origin, 1, SpriteEffects.None, entity.z);
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
            foreach (var entity in NetworkManager.Client.simulation.model.player_entities)
            {
                if (entity.texture == null)
                    entity.texture = Content.Load<Texture2D>(entity.texture_path);
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
