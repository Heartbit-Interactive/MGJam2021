using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public static class GraphicSettings
    {
        /// <summary>
        /// Screen Bounds used for reference when drawing
        /// </summary>
        public static Rectangle Bounds = new Rectangle(0, 0, 0, 0);
        /// <summary>
        /// White 2x2 Texture to fill screen using spritebatch
        /// </summary>
        public static Texture2D DebugSquare;
        /// <summary>
        /// Color for collider rects if ShowCollisions is true
        /// </summary>
        public static Color CollisorColor = new Color(0, 96, 0, 96);
        /// <summary>
        /// Color for non-collider rects if ShowCollisions is true
        /// </summary>
        public static Color NonCollidingColor = new Color(0, 0, 96, 96);        
        /// <summary>
        /// Whether to show or not collision rects below the entities
        /// </summary>
        internal static bool ShowCollisions;
    }
}
