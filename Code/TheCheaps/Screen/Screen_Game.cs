#define TEST
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TheCheapsLib;
using TheCheapsServer;

namespace TheCheaps.Scenes
{
    class Screen_Game : Screen_Base
    {
        private Game1 game;
        public Screen_Game(Game1 game)
        {
            this.game = game;
            if (NetworkManager.Client == null)
            {
                NetworkManager.StartServer(false);
                NetworkManager.BeginJoin(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), 12345,false);
                NetworkManager.Client.StateChanged += Client_StateChanged_Debug;
            }
            NetworkManager.Client.simulation.EntityAdded += Simulation_EntityAdded;
        }

        private void Client_StateChanged_Debug(object sender, EventArgs e)
        {
            NetworkManager.Client.SetReady(true);
        }
        private void Simulation_EntityAdded(object sender, EventArgs e)
        {
            var entity = (Entity)sender;
            if (entity.texture == null)
                entity.LoadTexture(Content);
        }

        private List<PlayerEntity> gui_entities;
        private ContentManager Content;
        public override void LoadContent(ContentManager content)
        {
            this.Content = content;
            var jsontextgui = File.ReadAllText("GUI.json");
            gui_entities = JsonConvert.DeserializeObject<List<PlayerEntity>>(jsontextgui);
            foreach (var entity in gui_entities.ToArray())
            {
                var m = Regex.Match(entity.name, @"FACE_ACTOR_(\d\d)");
                if (m.Success)
                {
                    if (int.TryParse(m.Groups[1].Captures[0].Value, out int id))
                    {
                        id--;
                        if (id != NetworkManager.Client.PlayerIndex)
                            gui_entities.Remove(entity);
                    }
                }
                else if (entity.name.StartsWith("HUD_Ricetta"))
                {
                    hud_ricetta = entity;
                    gui_entities.Remove(entity);
                }
                else if (entity.name.StartsWith("HUD_BarraTelegiornale"))
                {
                    barra_tg = entity;
                    gui_entities.Remove(barra_tg);
                }
            }
            GraphicSettings.DebugSquare = Content.Load<Texture2D>("menu/white_square");
            //Carico le textures degli oggetti già in sim
            foreach (var entity in NetworkManager.Client.simulation.model.entities.Values)
                Simulation_EntityAdded(entity, null);
        }
        public override void Update(GameTime gameTime)
        {
#if TEST
            if (NetworkManager.Client.network.model.serverState.GamePhase != TheCheapsLib.NetworkServerState.Phase.Gameplay)
            {
                var pls = NetworkManager.Client.network.model.players;
                if (pls.Any(x => x != null) && pls.All(p => p == null || p.Ready))
                    NetworkManager.Server.StartMatch();
            }
#endif
            player_index = NetworkManager.Client.PlayerIndex;
            UpdateGUI();
        }

        private void UpdateGUI()
        {
        }

        Color backgroundColor = new Color(191 ,149 ,77);
        private int player_index;
        private PlayerEntity hud_ricetta;
        private PlayerEntity barra_tg;
        private PlayerEntity player { get { return sim.player_entities[NetworkManager.Client.PlayerIndex]; } }

        SimulationModel sim { get { return NetworkManager.Client.simulation.model; } }
        public override void Draw(SpriteBatch spriteBatch)
        {
            refresh_entity_textures();
            game.GraphicsDevice.Clear(backgroundColor);
            if (player_index < 0)
                return;
            var pl = sim.player_entities.ElementAtOrDefault(player_index);
            if (pl == null)
                return;
            Matrix translation_matrix = MakeCameraMatrix(pl);
            spriteBatch.Begin(SpriteSortMode.FrontToBack, null, null, null, null, null, translation_matrix);

            foreach (var entity in sim.entities.Values)
            {
                entity.Draw(spriteBatch);
            }
            foreach (var entity in sim.player_entities)
            {
                entity.Draw(spriteBatch);
            }
            spriteBatch.End();
            spriteBatch.Begin();
            foreach (var entity in gui_entities)
            {                
                entity.Draw(spriteBatch);
            }
            draw_hud(spriteBatch);
            spriteBatch.End();
        }

        private void draw_hud(SpriteBatch spriteBatch)
        {
            hud_ricetta.Draw(spriteBatch);
            //spriteBatch.DrawString(spriteFont18, player, hud_ricetta.posxy, Color.White, true, false);
            //spriteBatch.DrawString(spriteFont18, "Bla bla", hud_ricetta.posxy, Color.White, true, false);
        }

        private static Matrix MakeCameraMatrix(PlayerEntity pl)
        {
            var posplayer = pl.posxy;
            var shift_x = Mathf.Clamp(posplayer.X - GraphicSettings.Bounds.Width / 2, 0, Settings.LevelWidth - GraphicSettings.Bounds.Width);
            var shift_y = Mathf.Clamp(posplayer.Y - GraphicSettings.Bounds.Height / 2, 0, Settings.LevelHeight - GraphicSettings.Bounds.Height);
            var translation_matrix = Matrix.CreateTranslation(new Vector3(-shift_x, -shift_y, 0));
            return translation_matrix;
        }

        public override void EndDraw(SpriteBatch spriteBatch)
        {
        }
        private void refresh_entity_textures()
        {
            foreach (var entity in gui_entities)
            {
                if (entity.texture == null)
                    entity.LoadTexture(Content);
            }
            foreach (var entity in NetworkManager.Client.simulation.model.player_entities)
            {
                if (entity.texture == null)
                    entity.LoadTexture(Content);
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
