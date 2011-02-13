/*
Embroidery Reader - an application to view .pes embroidery designs

Copyright (C) 2011 Nathan Crawford
 
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
You can contact me at http://www.njcrawford.com/contact
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
        private EmbroideryReaderSettings settings;
        public EmbroideryReaderSettings settingsToModify
        {
            get
            {
                return settings;
            }
            set
            {
                settings = value;
                if (settings.backgroundColorEnabled)
                {
                    lblColor.BackColor = settings.backgroundColor;
                }

                txtThreadThickness.Text = settings.threadThickness.ToString();

                chkUglyStitches.Checked = settings.filterStiches;

                txtThreshold.Text = settings.filterStitchesThreshold.ToString();
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
                    settings.backgroundColorEnabled = true;
                    settings.backgroundColor = lblColor.BackColor;
                }
                else
                {
                    settings.backgroundColorEnabled = false;
                }
            }
            double threadThickness;
            if (Double.TryParse(txtThreadThickness.Text, out threadThickness))
            {
                settings.threadThickness = threadThickness;
            }

            settings.filterStiches = chkUglyStitches.Checked;

            double threshold;
            if (Double.TryParse(txtThreshold.Text, out threshold))
            {
                settings.filterStitchesThreshold = threshold;
            }
        }
    }
}
