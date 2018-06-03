/*
Embroidery Reader - an application to view .pes embroidery designs

Copyright (C) 2016 Nathan Crawford
 
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
using System.Drawing;
using System.Windows.Forms;

namespace embroideryReader
{
    public partial class frmSettingsDialog : Form
    {
        private EmbroideryReaderSettings settings;
        private Translation translation;
        private List<Tuple<String, String>> availableTranslations = new List<Tuple<string,string>>();

        // Local copies of settings to update
        private bool backgroundColorEnabled;
        private Color transparencyGridColor;
        private int gridSize;

        public EmbroideryReaderSettings settingsToModify
        {
            get
            {
                return settings;
            }
            set
            {
                // Save a local reference to update later if/when the user clicks the OK button
                settings = value;

                // Load background color settings
                backgroundColorEnabled = settings.backgroundColorEnabled;
                if (backgroundColorEnabled)
                {
                    pnlBackground.BackColor = settings.backgroundColor;
                }

                // Load thread thickness setting
                txtThreadThickness.Text = settings.threadThickness.ToString();

                // Load filter stitches settings
                chkUglyStitches.Checked = settings.filterStiches;
                txtThreshold.Text = settings.filterStitchesThreshold.ToString();

                // Load transparency grid settings
                chkDrawGrid.Checked = settings.transparencyGridEnabled;
                gridSize = settings.transparencyGridSize;
                txtGridSize.Text = gridSize.ToString();
                transparencyGridColor = settings.transparencyGridColor;
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
                backgroundColorEnabled = true;
            }
        }

        private void btnResetColor_Click(object sender, EventArgs e)
        {
            pnlBackground.BackColor = Color.FromKnownColor(KnownColor.Control);
            backgroundColorEnabled = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            // Save thread thickness setting
            float threadThickness;
            if (Single.TryParse(txtThreadThickness.Text, out threadThickness))
            {
                settings.threadThickness = threadThickness;
            }

            // Save filter stitches settings
            settings.filterStiches = chkUglyStitches.Checked;
            float threshold;
            if (Single.TryParse(txtThreshold.Text, out threshold))
            {
                settings.filterStitchesThreshold = threshold;
            }

            // Save Transparency grid settings
            settings.transparencyGridEnabled = chkDrawGrid.Checked;
            settings.transparencyGridSize = gridSize;
            if(chkDrawGrid.Checked)
            {
                settings.transparencyGridColor = transparencyGridColor;
            }

            // Save background color settings
            settings.backgroundColorEnabled = backgroundColorEnabled;
            if(backgroundColorEnabled)
            {
                settings.backgroundColor = pnlBackground.BackColor;
            }

            // Save translation setting
            settings.translation = availableTranslations[cmbLanguage.SelectedIndex].Item2;

            // Save settings file
            settings.save();
        }

        public Translation setTranslation
        {
            set
            {
                translation = value;
                loadTranslatedStrings();
                availableTranslations = translation.GetAvailableTranslations();
                foreach (Tuple<String, String> names in availableTranslations)
                {
                    cmbLanguage.Items.Add(names.Item1 + " (" + names.Item2 + ")");
                    if(names.Item2 == settings.translation)
                    {
                        cmbLanguage.SelectedIndex = cmbLanguage.Items.Count - 1;
                    }
                }
                // Make sure a valid language is selected if no match was found
                if(cmbLanguage.Items.Count != 0 && cmbLanguage.SelectedIndex == -1)
                {
                    cmbLanguage.SelectedIndex = 0;
                }
            }
        }

        private void loadTranslatedStrings()
        {
            this.Text = translation.GetTranslatedString(Translation.StringID.SETTINGS);
            grpBackground.Text = translation.GetTranslatedString(Translation.StringID.BACKGROUND);
            lblBackgroundColor.Text = translation.GetTranslatedString(Translation.StringID.BACKGROUND_COLOR);
            btnColor.Text = translation.GetTranslatedString(Translation.StringID.PICK_COLOR);
            btnResetColor.Text = translation.GetTranslatedString(Translation.StringID.RESET_COLOR);
            grpStitch.Text = translation.GetTranslatedString(Translation.StringID.STITCH_DRAW);
            lblThreadThickness.Text = translation.GetTranslatedString(Translation.StringID.THREAD_THICKNESS);
            txtThreadThickness.Left = lblThreadThickness.Right + 5;
            lblPixelThick.Text = translation.GetTranslatedString(Translation.StringID.PIXELS);
            lblPixelThick.Left = txtThreadThickness.Right + 5;
            chkUglyStitches.Text = translation.GetTranslatedString(Translation.StringID.REMOVE_UGLY_STITCHES);
            lblUglyLength.Text = translation.GetTranslatedString(Translation.StringID.UGLY_STITCH_LENGTH);
            txtThreshold.Left = lblUglyLength.Right + 5;
            lblPixelLength.Text = translation.GetTranslatedString(Translation.StringID.PIXELS);
            lblPixelLength.Left = txtThreshold.Right + 5;
            chkDrawGrid.Text = translation.GetTranslatedString(Translation.StringID.ENABLE_TRANSPARENCY_GRID);
            grpLanguage.Text = translation.GetTranslatedString(Translation.StringID.LANGUAGE);
            btnCancel.Text = translation.GetTranslatedString(Translation.StringID.CANCEL);
            btnOK.Text = translation.GetTranslatedString(Translation.StringID.OK);
            btnGridColor.Text = translation.GetTranslatedString(Translation.StringID.PICK_COLOR);
            btnResetGridColor.Text = translation.GetTranslatedString(Translation.StringID.RESET_COLOR);
            lblGridSize.Text = translation.GetTranslatedString(Translation.StringID.GRID_SIZE);
            txtGridSize.Left = lblGridSize.Right + 5;
            lblGridSizePixels.Text = translation.GetTranslatedString(Translation.StringID.PIXELS);
            lblGridSizePixels.Left = txtGridSize.Right + 5;
            lblIncompleteTranslation.Text = translation.GetTranslatedString(Translation.StringID.TRANSLATION_INCOMPLETE);
        }

        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            translation.Load(availableTranslations[cmbLanguage.SelectedIndex].Item2);
            lblIncompleteTranslation.Visible = !translation.IsComplete();
            loadTranslatedStrings();
        }

        private void chkDrawGrid_CheckedChanged(object sender, EventArgs e)
        {
            pnlBackground.Invalidate();
            pnlBackground.Update();

            updateGridColorControls();
        }

        private void updateGridColorControls()
        {
            btnGridColor.Enabled = chkDrawGrid.Checked;
            btnResetGridColor.Enabled = chkDrawGrid.Checked;
            txtGridSize.Enabled = chkDrawGrid.Checked;
        }

        private void pnlBackground_Paint(object sender, PaintEventArgs e)
        {
            if (chkDrawGrid.Checked)
            {
                using (Pen gridPen = new Pen(transparencyGridColor))
                {
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
            colorDialog1.Color = transparencyGridColor;
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                transparencyGridColor = colorDialog1.Color;
                pnlBackground.Invalidate();
            }
        }

        private void btnResetGridColor_Click(object sender, EventArgs e)
        {
            transparencyGridColor = Color.LightGray;
            pnlBackground.Invalidate();
        }

        private void txtGridSize_TextChanged(object sender, EventArgs e)
        {
            if (Int32.TryParse(txtGridSize.Text, out gridSize))
            {
                // Try to keep grid size in a reasonable range
                if (gridSize < 1 || gridSize > 1000)
                {
                    gridSize = 5;
                }
                pnlBackground.Invalidate();
            }
        }
    }
}
