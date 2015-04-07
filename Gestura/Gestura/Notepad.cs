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
    class Notepad
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
        
        public void ControlNotepad(string button)
        {
            IntPtr notepadHandle = FindWindow("Notepad", "Untitled - Notepad");

            simpleDelegate.BeginInvoke(null, null);
            // Verify that Powerpoint is a running process.
            if (notepadHandle == IntPtr.Zero)
            {
                return;
            }

            switch (button)
            {
                case "Save":
                    SetForegroundWindow(notepadHandle);
                    SendKeys.SendWait("^s");
                    break;

                case "Open":
                    SetForegroundWindow(notepadHandle);
                    SendKeys.SendWait("^o");
                    break;

                case "Print":
                    SetForegroundWindow(notepadHandle);
                    SendKeys.SendWait("^p");
                    break;

                case "Time_Date":
                    SetForegroundWindow(notepadHandle);
                    SendKeys.SendWait("{F5}");
                    break;

                case "Highlight":
                    SetForegroundWindow(notepadHandle);
                    SendKeys.SendWait("{F10}");
                    break;

                case "Replace":
                    SetForegroundWindow(notepadHandle);
                    SendKeys.SendWait("^h");
                    break;

                case "Find":
                    SetForegroundWindow(notepadHandle);
                    SendKeys.SendWait("^f");
                    break;

                case "Selectall":
                    SetForegroundWindow(notepadHandle);
                    SendKeys.SendWait("^a");
                    break;
            }
        }
        #endregion
    }
}