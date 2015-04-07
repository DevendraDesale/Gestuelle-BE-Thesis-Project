#region Library Files
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DirectX.Capture;
using Microsoft.VisualBasic;
using System.Xml;
using NUnit.Framework;
using System.Collections.Generic;
#endregion

namespace Gestura
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Gestura());
            Application.Run(new Form1());

            AppDomain currentDomain = AppDomain.CurrentDomain;
            //Application.Run(new Gestura());
            Application.Run(new Form1());
        }
    }
}