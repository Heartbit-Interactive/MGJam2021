using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheCheapsLib
{
    public static partial class Mathf
    {
        public static float Clamp(float value, float min, float max)
        {
            return Math.Min(Math.Max(value, min), max);
        }
        public static int Clamp(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }
        public static byte Clamp(byte value, byte min, byte max)
        {
            return Math.Min(Math.Max(value, min), max);
        }
        public static int Wrap0(int value, int exmax)
        {
            return ((value % exmax) + exmax) % exmax;
        }
        internal static int min(int p1, int p2)
        {
            return Math.Min(p1, p2);
        }
        internal static int max(int p1, int p2)
        {
            return Math.Max(p1, p2);
        }

        public static float xydistance2(float x1, float y1, float x2, float y2)
        {
            var dx = x1 - x2;
            var dy = y1 - y2;
            return dx * dx + dy * dy;
        }
    }
}
