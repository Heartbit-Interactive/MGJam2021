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
                NetworkManager.Client.simulation.EntityAdded += Simulation_EntityAdded;
                NetworkManager.Client.StateChanged += Client_StateChanged_Debug;
            }
            else
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

        private List<Entity> gui_entities;
        private ContentManager Content;
        private bool contentLoaded;
        private SpriteFont font12;
        private SpriteFont font14;
        private SpriteFont font18;
        private SpriteFont font20;
        private SpriteFont font28;
        private Entity[] win_screens = new Entity[Settings.maxPlayers];
        public override void LoadContent(ContentManager content)
        {
            this.Content = content;
            UpdateLoadContent();
        }

        private void UpdateLoadContent()
        {
            if (contentLoaded || player_index < 0)
                return;
            contentLoaded = true;
            font12 = Content.Load<SpriteFont>("Font12");
            font14 = Content.Load<SpriteFont>("Font14");
            font18 = Content.Load<SpriteFont>("Font18");
            font20 = Content.Load<SpriteFont>("Font20");
            font28 = Content.Load<SpriteFont>("Font28");
            var jsontextgui = File.ReadAllText("Items.json");
            item_entities = JsonConvert.DeserializeObject<List<Entity>>(jsontextgui);
            jsontextgui = File.ReadAllText("Recipes.json");
            global_recipe_list = JsonConvert.DeserializeObject<List<Recipe>>(jsontextgui);
            jsontextgui = File.ReadAllText("GUI.json");
            gui_entities = JsonConvert.DeserializeObject<List<Entity>>(jsontextgui);
            foreach (var entity in gui_entities.Concat(item_entities).ToArray())
            {
                var m = Regex.Match(entity.name, @"FACE_ACTOR_(\d\d)");
                var m2 = Regex.Match(entity.name, @"WIN_(\d\d)");
                if (m.Success)
                {
                    if (int.TryParse(m.Groups[1].Captures[0].Value, out int id))
                    {
                        id--;
                        if (id != NetworkManager.Client.PlayerIndex)
                        {
                            gui_entities.Remove(entity);
                            continue;
                        }
                    }
                }
                else if (m2.Success)
                {
                    if (int.TryParse(m2.Groups[1].Captures[0].Value, out int id))
                    {
                        win_screens[id-1] = entity;
                        gui_entities.Remove(entity);
                    }
                }
                else if (entity.name.StartsWith("HUD_Ricetta"))
                {
                    hud_recipe = entity;
                    gui_entities.Remove(entity);
                }
                else if (entity.name.StartsWith("HUD_BarraTelegiornale"))
                {
                    hud_tg_bar = entity;
                    gui_entities.Remove(hud_tg_bar);
                }
                if (entity.texture == null)
                {
                    entity.LoadTexture(Content);
                    entity.origin = Vector2.Zero;
                    entity.hasShadow = false;
                }
            }
            GraphicSettings.DebugSquare = Content.Load<Texture2D>("menu/white_square");
            Entity.shadow = Content.Load<Texture2D>("Shadow");
            Entity.shadowOrigin = new Vector2(Entity.shadow.Width / 2, Entity.shadow.Height / 2);
            score_icon = item_entities.First(x => x.name == "Score");
            var icon_names = new[] { "UpDown++", "UpDown+", "UpDown-", "UpDown--" };
            arrow_icons = icon_names.Select(x => item_entities.First(y => y.name == x)).ToArray();
            //Carico le textures degli oggetti già in sim
            foreach (var entity in NetworkManager.Client.simulation.model.entities.Values)
                Simulation_EntityAdded(entity, null);
        }

        public override void Update(GameTime gameTime)
        {
            if (NetworkManager.Client.network.model.serverState.GamePhase != TheCheapsLib.NetworkServerState.Phase.Gameplay)
            {
                var pls = NetworkManager.Client.network.model.players;
                if (pls.Any(x => x != null) && pls.All(p => p == null || p.Ready))
                    NetworkManager.Server.StartMatch();
            }
            player_index = NetworkManager.Client.PlayerIndex;
            if (player_index < 0)
                return;
            if (sim.player_entities.Count == 0)
                return;
            UpdateLoadContent();
            UpdateGUI();
        }
        List<int> recipe_items = new List<int>();
        private void UpdateGUI()
        {
            if (last_recipe != player.inventory.list_recipes.FirstOrDefault()) //If recipe changed
            {
                last_recipe = player.inventory.list_recipes.FirstOrDefault();
                recipe_items.Clear();
                if (last_recipe != null)
                {
                    foreach (var item in last_recipe.ingredient_and_amount)
                        recipe_items.Add(item_entities.FindIndex(x => x.name.ToLowerInvariant() == item.Item1.ToLowerInvariant()));
                }
            }
        }

        Color backgroundColor = new Color(191 ,149 ,77);
        private int player_index = -1;
        private Entity hud_recipe;
        private Entity hud_tg_bar;
        private List<Entity> item_entities;
        private Recipe last_recipe;
        private List<Recipe> global_recipe_list;
        private Entity score_icon;
        private Entity[] arrow_icons;

        private PlayerEntity player { get { return sim.player_entities[player_index]; } }

        SimulationModel sim { get { return NetworkManager.Client.simulation.model; } }
        public override void Draw(SpriteBatch spriteBatch)
        {
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
            if (sim.timer <= 0)
            {
                var entity = win_screens[sim.player_entities.OrderByDescending(x=>x.score).First().index];
                entity.Draw(spriteBatch);
                return;
            }            
            if (player.inventory.list_recipes.Count > 0)
            {
                hud_recipe.Draw(spriteBatch);
                var recipe = player.inventory.list_recipes[0];
                var posxy = hud_recipe.posxy + new Vector2(8, 4);
                spriteBatch.DrawString(font12, recipe.name, posxy, Color.White);
                posxy.X += 8;
                posxy.Y += 22;
                Entity icon;
                for (int i=0;i<recipe.ingredient_and_amount.Count;i++)
                {
                    icon = item_entities[recipe_items[i]];
                    icon.posxy = posxy;
                    icon.hasShadow = false;
                    icon.Draw(spriteBatch);
                    spriteBatch.DrawString(font12, $"{recipe.owned[i]}/{recipe.ingredient_and_amount[i].Item2}", posxy + new Vector2(icon.sourcerect.Width/2,(icon.sourcerect.Height + 12)), Color.White, false, false);
                    posxy.X += 24+16;
                }
                posxy = hud_recipe.posxy+new Vector2(hud_recipe.sourcerect.Width-70,26);
                score_icon.posxy = posxy;
                score_icon.hasShadow = false;
                score_icon.Draw(spriteBatch);
                spriteBatch.DrawString(font12, $"+{recipe.score}", posxy + new Vector2(score_icon.sourcerect.Width+22, (score_icon.sourcerect.Height/2+2)), Color.White, false, false);
            }
            if (sim.broadcasting_news.Count > 0)
            {
                hud_tg_bar.Draw(spriteBatch);
                var posxy = hud_tg_bar.posxy + new Vector2(8, 12);
                var recipe_completed = global_recipe_list[sim.broadcasting_news[0]];
                var name = sim.player_entities[sim.broadcasting_news[1]].name;
                var mes = font20.MeasureString(name);
                spriteBatch.DrawString(font20, name, posxy, Color.Red);
                spriteBatch.DrawString(font20, recipe_completed.sentence_to_show, posxy+ Vector2.UnitX*(mes.X+10), Color.Black);
                posxy = hud_tg_bar.posxy + new Vector2(hud_tg_bar.sourcerect.Width-96, 4);
                var timer = (int)Math.Round(sim.timer);
                spriteBatch.DrawString(font28, $"{timer/60}:{timer%60:00}", posxy, Color.White);
                posxy = hud_tg_bar.posxy + new Vector2(24, hud_tg_bar.sourcerect.Height-26);
                var sorted_pl = sim.player_entities.OrderByDescending(x=>x.score);
                int i = 0;
                foreach(var pl in sorted_pl)
                {
                    arrow_icons[i].posxy = posxy;
                    arrow_icons[i].Draw(spriteBatch);
                    spriteBatch.DrawString(font12, $"{pl.name}:{(int)pl.score:+0}", posxy+new Vector2(arrow_icons[i].sourcerect.Width + 4,8), Color.White);
                    posxy.X += 236;
                    i++;
                }

            }
        }

        private static Matrix MakeCameraMatrix(PlayerEntity pl)
        {
            var posplayer = pl.posxy-Vector2.One*16;
            var shift_x = Mathf.Clamp(posplayer.X - GraphicSettings.Bounds.Width / 2, 0, Settings.LevelWidth - GraphicSettings.Bounds.Width);
            var shift_y = Mathf.Clamp(posplayer.Y - GraphicSettings.Bounds.Height / 2, 0, Settings.LevelHeight - GraphicSettings.Bounds.Height);
            var translation_matrix = Matrix.CreateTranslation(new Vector3(-shift_x, -shift_y, 0));
            return translation_matrix;
        }

        public override void EndDraw(SpriteBatch spriteBatch)
        {
        }
        public override void Terminate(ContentManager content)
        {
            NetworkManager.StopServer();
            NetworkManager.StopClient();
        }
    }
}
