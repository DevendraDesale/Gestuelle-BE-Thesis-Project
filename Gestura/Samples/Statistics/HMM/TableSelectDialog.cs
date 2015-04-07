#region Library Function
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
#endregion

namespace Components
{
    public partial class TableSelectDialog : Form
    {
        #region Methods
        public TableSelectDialog(string[] tables)
        {
            InitializeComponent();

            this.listBox1.DataSource = tables;
        }

        public string Selection
        {
            get { return this.listBox1.SelectedItem as string; }
        }
        #endregion
    }
}