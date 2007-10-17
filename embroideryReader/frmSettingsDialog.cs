using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace embroideryReader
{
    public partial class frmSettingsDialog : Form
    {
        private bool colorChanged = false;
        private nc_settings.IniFile _settings;
        public nc_settings.IniFile settings
        {
            get
            {
                return _settings;
            }
            set
            {
                _settings = value;
                if (settings.getValue("background color", "enabled") == "yes" &&
                    frmMain.checkColorFromStrings(
                    settings.getValue("background color", "red"),
                    settings.getValue("background color", "green"),
                    settings.getValue("background color", "blue")))
                {
                    lblColor.BackColor = frmMain.makeColorFromStrings(
                        settings.getValue("background color", "red"),
                        settings.getValue("background color", "green"),
                        settings.getValue("background color", "blue"));
                }
            }
        }

        public frmSettingsDialog()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //public void ShowDialog()
        //{
        //public 
        //if (settings.getValue("background color", "enabled") == "yes" &&
        //    frmMain.checkColorFromStrings(
        //    settings.getValue("background color", "red"),
        //    settings.getValue("background color", "green"),
        //    settings.getValue("background color", "blue")))
        //{
        //    lblColor.BackColor = frmMain.makeColorFromStrings(
        //        settings.getValue("background color", "red"),
        //        settings.getValue("background color", "green"),
        //        settings.getValue("background color", "blue"));
        //}
        //this.Show();
        //}

        private void btnColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = lblColor.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK && lblColor.BackColor != colorDialog1.Color)
            {
                lblColor.BackColor = colorDialog1.Color;
                colorChanged = true;
            }
        }

        private void btnResetColor_Click(object sender, EventArgs e)
        {
            if (lblColor.BackColor != Color.FromKnownColor(KnownColor.Control))
            {
                lblColor.BackColor = Color.FromKnownColor(KnownColor.Control);
                colorChanged = true;
                //settings.setValue("background color", "enabled", "no");
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (colorChanged)
            {
                if (lblColor.BackColor != Color.FromKnownColor(KnownColor.Control))
                {
                    settings.setValue("background color", "enabled", "yes");
                    settings.setValue("background color", "red", lblColor.BackColor.R.ToString());
                    settings.setValue("background color", "green", lblColor.BackColor.G.ToString());
                    settings.setValue("background color", "blue", lblColor.BackColor.B.ToString());
                }
                else
                {
                    settings.setValue("background color", "enabled", "no");
                }
            }
        }
    }
}