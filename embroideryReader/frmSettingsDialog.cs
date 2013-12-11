/*
Embroidery Reader - an application to view .pes embroidery designs

Copyright (C) 2013 Nathan Crawford
 
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
        private EmbroideryReaderSettings settings;
        private Translation translation;

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
                    pnlBackground.BackColor = settings.backgroundColor;
                }

                txtThreadThickness.Text = settings.threadThickness.ToString();

                chkUglyStitches.Checked = settings.filterStiches;

                txtThreshold.Text = settings.filterStitchesThreshold.ToString();

                chkDrawGrid.Checked = settings.transparencyGridEnabled;
                updateGridColorControls();
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
            colorDialog1.Color = pnlBackground.BackColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pnlBackground.BackColor = colorDialog1.Color;
                settings.backgroundColor = colorDialog1.Color;
                settings.backgroundColorEnabled = true;
            }
        }

        private void btnResetColor_Click(object sender, EventArgs e)
        {
            pnlBackground.BackColor = Color.FromKnownColor(KnownColor.Control);
            settings.backgroundColorEnabled = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
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

            settings.transparencyGridEnabled = chkDrawGrid.Checked;

            settings.translation = cmbLanguage.SelectedItem.ToString();
        }

        public Translation setTranslation
        {
            set
            {
                translation = value;
                loadTranslatedStrings();
                foreach (String s in translation.GetAvailableTranslations())
                {
                    cmbLanguage.Items.Add(s);
                }
                if (cmbLanguage.Items.Count > 0)
                {
                    cmbLanguage.SelectedItem = settings.translation;
                }
            }
        }

        private void loadTranslatedStrings()
        {
            this.Text = translation.GetTranslatedString(Translation.StringID.SETTINGS);
            grpBackground.Text = translation.GetTranslatedString(Translation.StringID.BACKGROUND);
            pnlBackground.Text = translation.GetTranslatedString(Translation.StringID.BACKGROUND_COLOR);
            btnColor.Text = translation.GetTranslatedString(Translation.StringID.PICK_COLOR);
            btnResetColor.Text = translation.GetTranslatedString(Translation.StringID.RESET_COLOR);
            grpStitch.Text = translation.GetTranslatedString(Translation.StringID.STITCH_DRAW);
            lblThreadThickness.Text = translation.GetTranslatedString(Translation.StringID.THREAD_THICKNESS);
            lblPixelThick.Text = translation.GetTranslatedString(Translation.StringID.PIXELS);
            chkUglyStitches.Text = translation.GetTranslatedString(Translation.StringID.REMOVE_UGLY_STITCHES);
            lblUglyLength.Text = translation.GetTranslatedString(Translation.StringID.UGLY_STITCH_LENGTH);
            txtThreshold.Left = lblUglyLength.Left + lblUglyLength.Width + 5;
            lblPixelLength.Text = translation.GetTranslatedString(Translation.StringID.PIXELS);
            lblPixelLength.Left = txtThreshold.Left + txtThreshold.Width + 5;
            chkDrawGrid.Text = translation.GetTranslatedString(Translation.StringID.DRAW_BACKGROUND_GRID);
            grpLanguage.Text = translation.GetTranslatedString(Translation.StringID.LANGUAGE);
            btnCancel.Text = translation.GetTranslatedString(Translation.StringID.CANCEL);
            btnOK.Text = translation.GetTranslatedString(Translation.StringID.OK);
            btnGridColor.Text = translation.GetTranslatedString(Translation.StringID.PICK_COLOR);
            btnResetGridColor.Text = translation.GetTranslatedString(Translation.StringID.RESET_COLOR);
        }

        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            translation.Load(cmbLanguage.SelectedItem.ToString());
            loadTranslatedStrings();
        }

        private void chkDrawGrid_CheckedChanged(object sender, EventArgs e)
        {
            settings.transparencyGridEnabled = chkDrawGrid.Checked;
            pnlBackground.Invalidate();
            pnlBackground.Update();

            updateGridColorControls();
        }

        private void updateGridColorControls()
        {
            btnGridColor.Enabled = chkDrawGrid.Checked;
            btnResetGridColor.Enabled = chkDrawGrid.Checked;
        }

        private void pnlBackground_Paint(object sender, PaintEventArgs e)
        {
            if (settings.transparencyGridEnabled)
            {
                Color gridColor = settings.transparencyGridColor;
                using (Pen gridPen = new Pen(gridColor))
                {

                    int gridSize = settings.transparencyGridSize;
                    for (int xStart = 0; (xStart * gridSize) < pnlBackground.Width; xStart++)
                    {
                        for (int yStart = 0; (yStart * gridSize) < pnlBackground.Height; yStart++)
                        {
                            // Fill even columns in even rows and odd columns in odd rows
                            if ((xStart % 2) == (yStart % 2))
                            {
                                e.Graphics.FillRectangle(gridPen.Brush, (xStart * gridSize), (yStart * gridSize), gridSize, gridSize);
                            }
                        }
                    }
                }
            }
        }

        private void btnGridColor_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = settings.transparencyGridColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                settings.transparencyGridColor = colorDialog1.Color;
                pnlBackground.Invalidate();
            }
        }

        private void btnResetGridColor_Click(object sender, EventArgs e)
        {
            settings.transparencyGridColor = Color.LightGray;
            pnlBackground.Invalidate();
        }
    }
}
