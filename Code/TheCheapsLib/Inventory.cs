using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public class Inventory
    {
        public List<Entity> entities = new List<Entity>();//id oggetti posseduti
        internal int size = 3;
        public List<Recipe> list_recipes = new List<Recipe>();
        public Inventory() { }

        public bool if_player_needs_ingredient_add(string entity_name)
        {
            foreach (var recipe in list_recipes)
            {
               for(int i =0; i< recipe.ingredient_and_amount.Count; i++)
                {
                    if (recipe.ingredient_and_amount[i].Item1 == entity_name)
                    {
                        recipe.owned[i]++;
                        return true;

                    }
                }
            }
            return false;
        }
        internal void update_entities_pos()
        {

        }
    }
}
