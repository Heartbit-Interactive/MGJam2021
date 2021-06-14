using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheCheapsLib
{
    public enum SEType {
        Rummage,
        Throw,
        Stun,
        Sold,
        Recipe
    }
    public static class SoundManager
    {
        public static ContentManager content;
        public static void LoadContent(ContentManager incontent)
        {
            content = incontent;
            buzzer = content.Load<SoundEffect>("menu/buzzer");
            accept = content.Load<SoundEffect>("menu/decision");
            cancel = content.Load<SoundEffect>("menu/cancel");
            cursor = content.Load<SoundEffect>("menu/cursor");
        }
        internal static void PlaySE(SEType sEType)
        {
            SoundEffect se = null;
            try
            {
                switch (sEType)
                {
                    case SEType.Rummage:
                        se = content.Load<SoundEffect>("");
                        break;
                    case SEType.Throw:
                        se = content.Load<SoundEffect>("");
                        break;
                    case SEType.Stun:
                        se = content.Load<SoundEffect>("");
                        break;
                    case SEType.Sold:
                        se = content.Load<SoundEffect>("");
                        break;
                    case SEType.Recipe:
                        se = content.Load<SoundEffect>("");
                        break;
                    default:
                        break;
                }
            }
            catch { }
            if (se != null)
                se.Play(0.5f, 0, 0);
        }
        internal static void PlayBGM(string name)
        {
            var song = content.Load<Song>(name);
            MediaPlayer.Play(song);
        }
        internal static void StopBGM()
        {
            MediaPlayer.Stop();
        }
        private static SoundEffect buzzer;
        private static SoundEffect accept;
        private static SoundEffect cancel;
        private static SoundEffect cursor;
        private static float se_volume = 0.75f;

        public static void PlayBuzzer()
        {
            buzzer.Play(se_volume, 0, 0);
        }
        public static void PlayDecision()
        {
            accept.Play(se_volume, 0, 0);
        }
        public static void PlayCancel()
        {
            cancel.Play(se_volume, 0, 0);
        }
        public static void PlayCursors()
        {
            cursor.Play(se_volume, 0, 0);
        }
    }
}
