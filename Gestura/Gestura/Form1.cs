#region Library Files
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Speech.Synthesis;
#endregion

namespace Gestura
{
    public partial class Form1 : Form
    {
        #region Variables and Initializations
        public static int flag,flag2;
        public SpeechSynthesizer speaker = new SpeechSynthesizer();
        #endregion

        #region Constructors And destructors
        public Form1()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
            //SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //this.BackColor = Color.Transparent;
            this.TransparencyKey = Color.Turquoise;
            this.BackColor = Color.Turquoise;
        }
        #endregion

        #region UI Functions
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Opacity = 0.0;
            //Application.OpenForms[0].Opacity = 0;
            flag = 1;
            timer1.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Gestura new_form = new Gestura();
            new_form.Show();
            //Application.OpenForms[0].Opacity = 0.89;

        }

        #endregion

        #region Timer Functions
        private void tick(object sender, EventArgs e)
        {
            if (flag==1)
            {
                if (this.Opacity < 1)
                {
                    this.Opacity += 0.02;
                }
                else
                {
                    timer1.Stop();
                    flag = 0;
                    flag2 = 1;

                    /*speaker.Rate = 1;
                    speaker.Volume = 100;
                    speaker.Speak("Welcome to Gestuelle.");*/

                    timer2.Start();
                }
            }
            else
            {
                timer2.Stop();
                timer1.Start();
                if (this.Opacity > 0)
                {
                    this.Opacity -= 0.04;
                }
                else
                {
                    timer1.Stop();
                    //this.Close();
                    Gestura new_form = new Gestura();
                    new_form.Show();
                    this.Hide();
                }
            }
        }
        #endregion

    }
}
