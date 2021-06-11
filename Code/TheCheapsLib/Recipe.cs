﻿using System;
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
    }
}
