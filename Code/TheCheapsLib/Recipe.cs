using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public class Recipe
    {
        public string name;

        public List<Tuple<string, int>> ingredient_and_amount = new List<Tuple<string, int>>();
        public int[] owned;
        public int score;
        public string type; //A o B
        public string sentence_to_show;
        public string character_associated;

        public Recipe() 
        {
            if(ingredient_and_amount.Count>0)
                owned = new int[ingredient_and_amount.Count];
        }

        public Recipe(string name, List<Tuple<string, int>> ingredient_and_amount, int[] owned, int score, string type, string sentence_to_show, string character_associated)
        {
            this.name = name;
            this.ingredient_and_amount = ingredient_and_amount;
            this.owned = new int[ingredient_and_amount.Count];
            this.score = score;
            this.type = type;
            this.sentence_to_show = sentence_to_show;
            this.character_associated = character_associated;
        }
    }
}
