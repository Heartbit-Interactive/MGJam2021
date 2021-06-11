﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public static class Settings
    {
        public const int maxPlayers = 4;
        internal const float fall_speed = 2/3f;

        public const int TimeOnTheFloor = 180;

        public const int scoreSellItem = 5;
#if DEBUG
        public static float StartGameCountDownMax = 3;
#else
        public static float StartGameCountDownMax = 5;
#endif
        public static int LevelWidth = 1920;
        public static int LevelHeight = 1080;

        public static float ServerTicksPerSecond { get { return 90; } }
    }
}
