#region Library Files
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
#endregion
namespace Gestura
{
    public partial class Tutorial : Form
    {
        #region Variables and Initializations
        public int flag = 1; //flag=> 1: play 2: pause 3: stop
        public int collapse = 1;
        #endregion

        #region Constructor
        public Tutorial()
        {
            InitializeComponent();
        }
        #endregion

        #region UI Functions
        private void Tutorial_Load(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.URL = @"d:\demo.avi";
            axWindowsMediaPlayer1.uiMode = "none";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (flag == 1)
            {
                axWindowsMediaPlayer1.Ctlcontrols.pause();
                flag = 2;
                button1.Text = "Play";
            }
            else if (flag == 2)
            {
                axWindowsMediaPlayer1.Ctlcontrols.play();
                flag = 1;
                button1.Text = "Pause";
            }
            else
            {
                axWindowsMediaPlayer1.URL = @"d:\demo.avi";
                flag = 1;
                button1.Text = "Play";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.Ctlcontrols.stop();
            flag = 3;
            button1.Text = "Play";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(flag != 3)
                axWindowsMediaPlayer1.Ctlcontrols.stop();
            Application.OpenForms[0].Show();
            this.Close();
        }

        private void axWindowsMediaPlayer1_MouseDownEvent(object sender, AxWMPLib._WMPOCXEvents_MouseDownEvent e)
        {
            if (collapse == 1)
            {
                for (int i = 374; i <= 414; i ++)
                {
                    //Thread.Sleep(25);
                    //Application.OpenForms[1].Height += 5;
                    this.Height++;
                }
                button1.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
                collapse = 0;
            }
            else
            {
                for (int i = 414; i >= 374; i --)
                {
                    //Thread.Sleep(25);
                    //Application.OpenForms[1].Height += 5;
                    this.Height--;
                }
                button1.Visible = false;
                button2.Visible = false;
                button3.Visible = false;
                collapse = 1;
            }
        }
        #endregion
    }
}
