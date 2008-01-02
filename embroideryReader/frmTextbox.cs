using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace embroideryReader
{
    public partial class frmTextbox : Form
    {
        public frmTextbox()
        {
            InitializeComponent();
        }

        public void showText(string text)
        {
            textBox1.Text = text;
            this.Show();
        }
    }
}