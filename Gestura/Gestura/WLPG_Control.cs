#region Library Files
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
#endregion

namespace Gestura
{
    class WLPG_Control
    {
        #region Variables and Initializations
        MethodInvoker simpleDelegate = new MethodInvoker(make_a_beep);
        Process p = new Process();
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

        public void ControlPictureViewer(string button)
        {
            IntPtr pictureViewerHandle = FindWindow(null, "Windows Live Photo Gallery");
            

            simpleDelegate.BeginInvoke(null, null);
            // Verify that WPFV is a running process.
            if (pictureViewerHandle == IntPtr.Zero)
            {
                return;
            }

            switch (button)
            {
                case "LeftImage":
                    SetForegroundWindow(pictureViewerHandle);
                    SendKeys.SendWait("{LEFT}");
                    break;

                case "RightImage":
                    SetForegroundWindow(pictureViewerHandle);
                    SendKeys.SendWait("{RIGHT}");
                    break;

                case "SlideShow":
                    SetForegroundWindow(pictureViewerHandle);
                    SendKeys.SendWait("{F12}");
                    break;
                    
                case "Zoom_In":
                    SetForegroundWindow(pictureViewerHandle);
                    SendKeys.SendWait("{+}");
                    break;

                case "Zoom_Out":
                    SetForegroundWindow(pictureViewerHandle);
                    SendKeys.SendWait("{-}");
                    break;

                case "Print":
                    SetForegroundWindow(pictureViewerHandle);
                    SendKeys.SendWait("^p");
                    break;

                case "Rotate_Clock":
                    SetForegroundWindow(pictureViewerHandle);
                    SendKeys.SendWait("^.");
                    break;

                case "Rotate_Anti":
                    SetForegroundWindow(pictureViewerHandle);
                    SendKeys.SendWait("^,");
                    break;
            }
        }
        #endregion
    }
}