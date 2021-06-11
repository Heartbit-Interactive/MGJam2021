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
        internal const float fall_speed = (1.2f)*60f;
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
        /// Distance in pixels that the dash move allow to traverse in a single use
        /// </summary>
        internal static float DashDistance = 48;
        /// <summary>
        /// Pixels/Second traversed when moving
        /// </summary>
        internal static float MoveSpeed = 3*60f;

        /// <summary>
        /// Exit Speed in pixel per second of a thrown item
        /// </summary>
        public static float ThrowSpeed = 4*60f;
        /// <summary>
        /// Number of refeshes per second of the game server
        /// </summary>
        public static float ServerTicksPerSecond { get { return 90; } }
        /// <summary>
        /// Number of hits required to mine a resource from a pile
        /// </summary>
        public const int ClicksRequiredToMineResource = 5;
        /// <summary>
        /// Number of seconds after which the clicked counter is resed due to inactivity
        /// </summary>
        public const float MaxTimeBetwheenClicksToMineResource = 30/60f;
        /// <summary>
        /// Number of seconds bethween dash moves
        /// </summary>
        public const float DashRecoilS = 40/60f;

    }
}
