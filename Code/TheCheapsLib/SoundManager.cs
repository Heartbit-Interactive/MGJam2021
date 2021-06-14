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
        Recipe,
        Dash
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
                        se = content.Load<SoundEffect>("SE/Hit_Dig1");
                        break;
                    case SEType.Throw:
                        se = content.Load<SoundEffect>("SE/Hit_Bush2_edit");
                        break;
                    case SEType.Stun:
                        se = content.Load<SoundEffect>("SE/Blow5");
                        break;
                    case SEType.Sold:
                        se = content.Load<SoundEffect>("SE/Hit_Interact1");
                        break;
                    case SEType.Recipe:
                        se = content.Load<SoundEffect>("SE/Title_Risucchio01");
                        break;
                    case SEType.Dash:
                        se = content.Load<SoundEffect>("SE/Wind7");
                        break;
                    default:
                        break;
                }
            }
            catch { }
            if (se != null)
                se.Play(0.55f, 0, 0);
        }
        public static void PlayBGM(string name)
        {
            var song = content.Load<Song>(name);
            current_song_name = name;
            MediaPlayer.Volume = 0.45f;
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
        }
        public static void StopBGM(string name)
        {
            if (current_song_name == name)
                MediaPlayer.Stop();
        }
        private static SoundEffect buzzer;
        private static SoundEffect accept;
        private static SoundEffect cancel;
        private static SoundEffect cursor;
        private static float se_volume = 0.75f;
        private static string current_song_name;

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
