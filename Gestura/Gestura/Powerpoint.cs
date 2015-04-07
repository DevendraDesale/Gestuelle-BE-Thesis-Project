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
    class Powerpoint
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

        public void ControlPowerpoint(string button)
        {
            IntPtr powerpointHandle = FindWindow("PP12FrameClass", "Microsoft Powerpoint - [presentation1]");

            simpleDelegate.BeginInvoke(null, null);
            // Verify that  is a running process.
            if (powerpointHandle == IntPtr.Zero)
            {
                return;
            }

            switch (button)
            {
                case "Slideshow_Begin":
                    SetForegroundWindow(powerpointHandle);
                    SendKeys.SendWait("{F5}");
                    break;

                case "Open":
                    SetForegroundWindow(powerpointHandle);
                    SendKeys.SendWait("^o");
                    break;

                case "Print":
                    SetForegroundWindow(powerpointHandle);
                    SendKeys.SendWait("^p");
                    break;

                case "Save":
                    SetForegroundWindow(powerpointHandle);
                    SendKeys.SendWait("^s");
                    break;

                case "Add_Slide":
                    SetForegroundWindow(powerpointHandle);
                    SendKeys.SendWait("^m");
                    break;

                case "Slideshow_Next":
                    SetForegroundWindow(powerpointHandle);
                    SendKeys.SendWait("n");
                    break;

                case "Slideshow_Previous":
                    SetForegroundWindow(powerpointHandle);
                    SendKeys.SendWait("p");
                    break;

                case "Slideshow_End":
                    SetForegroundWindow(powerpointHandle);
                    SendKeys.SendWait("{ESC}");
                    break;
            }
        }
        #endregion
    }
}