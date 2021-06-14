using System;
using System.Windows;
using System.Windows.Forms;

namespace TheCheaps
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            ////COMMENTA DA QUI--->
            if (System.Diagnostics.Debugger.IsAttached)
            {
                using (Game1 game = new Game1())
                {
                    game.Run();
                }
            }
            else
            ////A QUI<--
            {
                try
                {
                    // Add the event handler for handling non-UI thread exceptions to the event. 
                    AppDomain.CurrentDomain.UnhandledException += new
                    UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                    using (Game1 game = new Game1())
                    {
                        game.Run();
                    }
                }
                catch (Exception e)
                {
                    WriteLog(e);
                }
            }
        }
        private static string WriteException(Exception e)
        {
            if (!System.IO.Directory.Exists("Logs"))
                System.IO.Directory.CreateDirectory("Logs");
            var fname = $"Logs/error_{DateTime.Now.Ticks}.txt";
            using (var tw = System.IO.File.CreateText(fname))
            {
                Exception inner_ex = e;
                while (inner_ex != null)
                {
                    tw.WriteLine(inner_ex.ToString());
                    inner_ex = inner_ex.InnerException;
                    if (inner_ex != null)
                        tw.WriteLine("Inner Exception:");
                }
            }
            return fname;
        }
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            WriteLog((Exception)e.ExceptionObject);
        }

        private static void WriteLog(Exception e)
        {
            if (e is Microsoft.Xna.Framework.Audio.NoAudioHardwareException) //ERRORE NOTO CHE IGNORIAMO COSCENTEMENTE, L'UTENTE HA PREMUTO ABORT SULLA SEGNALAZIONE DI ERRORE DI HARDWARE AUDIO MANCANTE
                return;

            string path_error = null;
            try
            {
                path_error = WriteException(e);
            }
            catch { }

            var subj = $"E: {e.GetType().Name}";

            //ESTRAE LA DESCRIZIONE COMPLETA DELL'ECCEZIONE, INCLUSE LE INNER EXCEPTIONS
            Exception inner_ex = e;
            var sw = new System.Text.StringBuilder();
            while (inner_ex != null)
            {
                sw.AppendLine(inner_ex.ToString());
                inner_ex = inner_ex.InnerException;
                if (inner_ex != null)
                    sw.AppendLine("Inner Exception:");
            }

            try
            {
                var errormessage = MessageBox.Show(e.ToString(), $"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch { }
        }
    }
}
