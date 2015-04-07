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
    class Prezi
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

        public void ControlPrezi(string button)
        {
            IntPtr preziHandle = FindWindow("", "Adobe Flash Player 10");

            simpleDelegate.BeginInvoke(null, null);
            // Verify that Powerpoint is a running process.
            if (preziHandle == IntPtr.Zero)
            {
                return;
            }

            switch (button)
            {
                case "Left":
                    SetForegroundWindow(preziHandle);
                    SendKeys.SendWait("^{LEFT}");
                    break;

                case "Right":
                    SetForegroundWindow(preziHandle);
                    SendKeys.SendWait("^{RIGHT}");
                    break;
            }
        }
        #endregion
    }
}