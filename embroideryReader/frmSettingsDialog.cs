/*
Embroidery Reader - an application to view .pes embroidery designs

Copyright (C) 2009  Nathan Crawford
 
This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
 
You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA
02111-1307, USA.

A copy of the full GPL 2 license can be found in the docs directory.
You can contact me at http://www.njcrawford.com/contact.php.
*/


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
        private NJCrawford.IniFile _settings;
        public NJCrawford.IniFile settings
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
                if (settings.getValue("thread thickness") != null)
                {
                    double threadThickness = 1;
                    if (!Double.TryParse(settings.getValue("thread thickness"), out threadThickness))
                    {
                        threadThickness = 5;
                    }

                    if (threadThickness < 1)
                    {
                        threadThickness = 1;
                    }
                    txtThreadThickness.Text = threadThickness.ToString();
                }
                else
                {
                    txtThreadThickness.Text = "5";
                }

                if (settings.getValue("filter stitches") == "true")
                {
                    chkUglyStitches.Checked = true;
                }
                else
                {
                    chkUglyStitches.Checked = false;
                }

                if (settings.getValue("filter stitches threshold") != null)
                {
                    txtThreshold.Text= settings.getValue("filter stitches threshold");
                }
                else
                {
                    txtThreshold.Text = "120";
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
            double threadThickness = 5;
            if (Double.TryParse(txtThreadThickness.Text, out threadThickness))
            {
                if (threadThickness < 1)
                {
                    threadThickness = 1;
                }
                settings.setValue("thread thickness", threadThickness.ToString());
            }

            if (chkUglyStitches.Checked)
            {
                settings.setValue("filter stitches", "true");
            }
            else
            {
                settings.setValue("filter stitches", "false");
            }

            double threshold = 120;
            if (Double.TryParse(txtThreshold.Text, out threshold))
            {
                if (threshold < 10)
                {
                    threshold = 10;
                }
                settings.setValue("filter stitches threshold", threshold.ToString());
            }
        }
    }
}