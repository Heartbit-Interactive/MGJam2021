using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public static class Rand
    {
        static System.Random _rng;
        public static string GeneratePlayerName()
        {
            return $"Funny Random Name {Generator.Next(1000)}";
        }

        public static System.Random Generator { get { return _rng; } }
        static Rand()
        {
            _rng = new System.Random(DateTime.Now.Millisecond);
        }
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = _rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
