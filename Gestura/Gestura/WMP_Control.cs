#region Library Files
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
#endregion

namespace Gestura
{
    class WMP_Control
    {
        #region Variables and Initializations
        MethodInvoker simpleDelegate = new MethodInvoker(make_a_beep);
        #endregion

        #region Activate an application window
        [DllImport("USER32.DLL")]
        private static extern IntPtr FindWindow(string lpClassName,
            string lpWindowName);

        // Activate an application window.

        [DllImport("USER32.DLL")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        #endregion

        #region Control Function
        static private void make_a_beep()
        {
            Console.Beep(659, 200);
        }

        public void ControlMediaPlayer(string button)
        {
            IntPtr mediaPlayerHandle =
                   FindWindow("WMPlayerApp", "Windows Media Player");

            simpleDelegate.BeginInvoke(null, null);
            // Verify that WMP is a running process.
            if (mediaPlayerHandle == IntPtr.Zero)
            {
                return;
            }

            switch (button)
            {
                case "PreviousSong":
                    SetForegroundWindow(mediaPlayerHandle);
                    SendKeys.SendWait("^b");
                    break;

                case "NextSong":
                    SetForegroundWindow(mediaPlayerHandle);
                    SendKeys.SendWait("^f");
                    break;

                case "Play/Pause":
                    SetForegroundWindow(mediaPlayerHandle);
                    SendKeys.SendWait("^p");
                    break;

                case "Stop":
                    SetForegroundWindow(mediaPlayerHandle);
                    SendKeys.SendWait("^s");
                    break;

                case "Increase_Vol":
                    SetForegroundWindow(mediaPlayerHandle);
                    SendKeys.SendWait("{F9}");
                    break;

                case "Decrease_Vol":
                    SetForegroundWindow(mediaPlayerHandle);
                    SendKeys.SendWait("{F8}");
                    break;

                case "Repeat":
                    SetForegroundWindow(mediaPlayerHandle);
                    SendKeys.SendWait("^t");
                    break;

                case "Shuffle":
                    SetForegroundWindow(mediaPlayerHandle);
                    SendKeys.SendWait("^h");
                    break;
            }
        }
        #endregion
    }
}