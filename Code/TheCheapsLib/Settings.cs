using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public static class Settings
    {
        public const int maxPlayers = 4;
        /// <summary>
        /// Fall speed when throwing an item, in pixel per second (converted from frames)
        /// </summary>
        internal const float fall_speed = (2/3f)*60f;
        /// <summary>
        /// Time in second before an item on the floor disappears
        /// </summary>
        public const float TimeOnTheFloor = 3f;
        /// <summary>
        /// Score received when selling an item that is not in the current recipe
        /// </summary>
        public const int scoreSellItem = 5;
        /// <summary>
        /// Time before the start of a match
        /// </summary>
#if DEBUG
        public static float StartGameCountDownMax = 3;
#else
        public static float StartGameCountDownMax = 5;
#endif
        /// <summary>
        /// Total level size in pixels
        /// </summary>
        public static int LevelWidth = 1920;
        /// <summary>
        /// Total level size in pixels
        /// </summary>
        public static int LevelHeight = 1080;

        /// <summary>
        /// Number of refeshes per second of the game server
        /// </summary>
        public static float ServerTicksPerSecond { get { return 90; } }
    }
}
