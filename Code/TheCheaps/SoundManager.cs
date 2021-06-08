using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheaps
{
    static class SoundManager
    {
        private static SoundEffect buzzer;
        private static SoundEffect accept;
        private static SoundEffect cancel;
        private static SoundEffect cursor;
        private static float se_volume = 0.75f;

        public static void LoadContent(ContentManager content)
        {
            buzzer = content.Load<SoundEffect>("menu/buzzer");
            accept = content.Load<SoundEffect>("menu/decision");
            cancel = content.Load<SoundEffect>("menu/cancel");
            cursor = content.Load<SoundEffect>("menu/cursor");
        }
        internal static void PlayBuzzer()
        {
            buzzer.Play(se_volume, 0, 0);
        }
        internal static void PlayDecision()
        {
            accept.Play(se_volume, 0, 0);
        }
        internal static void PlayCancel()
        {
            cancel.Play(se_volume, 0, 0);
        }
        internal static void PlayCursors()
        {
            cursor.Play(se_volume, 0, 0);
        }
    }
}
