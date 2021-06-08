using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public static class Random
    {
        static System.Random _rng;
        public static System.Random rng { get { return _rng; } }
        static Random()
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
