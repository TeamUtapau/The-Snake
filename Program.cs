using System;
using System.Windows.Forms;
using System.Media;
using System.Threading;

namespace Snake
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            new Thread(() =>
            {
                System.Media.SoundPlayer sp = new SoundPlayer(@"110_dirty-offbeat-funk-synthbass.wav");
                sp.PlayLooping();
            }).Start();
        


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
