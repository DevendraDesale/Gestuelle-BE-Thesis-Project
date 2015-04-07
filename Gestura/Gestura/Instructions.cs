#region Library Files
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
#endregion

namespace Gestura
{
    public partial class Instructions : Form
    {
        #region Constructor And Destructors
        public Instructions()
        {
            InitializeComponent();
        }
        #endregion

        #region UI Functions
        private void Instructions_Load(object sender, EventArgs e)
        {

        }

        private void Instructions_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Application.OpenForms[0].Show();
            this.Close();
        }
        #endregion
    }
}
