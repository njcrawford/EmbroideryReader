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
using System.Reflection;
using System.Runtime.InteropServices;

namespace embroideryReader
{
    public partial class frmMain : Form
    {
        private string[] args;
        private Pen drawPen = Pens.Black;
        private Bitmap DrawArea;
        private PesFile.PesFile design;
        private EmbroideryReaderSettings settings = new EmbroideryReaderSettings();

        private const String APP_TITLE = "Embroidery Reader";

        private Translation translation;

        public frmMain()
        {
            InitializeComponent();
            args = Environment.GetCommandLineArgs();
        }

        private void checkSettings()
        {
            if (settings.backgroundColorEnabled)
            {
                panel2.BackColor = settings.backgroundColor;
            }
            else
            {
                panel2.BackColor = Color.FromKnownColor(KnownColor.Control);
            }
            this.Width = settings.windowWidth;
            this.Height = settings.windowHeight;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            checkSettings();
            this.Text = APP_TITLE;
            if (args.Length > 1)
            {
                openFile(args[1]);
            }

            loadTranslatedStrings(settings.translation);
        }

        public static bool checkColorFromStrings(string red, string green, string blue)
        {
            byte redByte;
            byte greenByte;
            byte blueByte;
            bool retval = false;
            if (String.IsNullOrEmpty(red) || String.IsNullOrEmpty(green) || String.IsNullOrEmpty(blue))
            {
                retval = false;
            }
            else
            {
                try
                {
                    redByte = Convert.ToByte(red);
                    greenByte = Convert.ToByte(green);
                    blueByte = Convert.ToByte(blue);
                    retval = true;
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine(ex.Message);
#endif
                    retval = false;
                }
            }
            return retval;
        }

        public static Color makeColorFromStrings(string red, string green, string blue)
        {
            if (checkColorFromStrings(red, green, blue))
            {
                return Color.FromArgb(Convert.ToByte(red), Convert.ToByte(green), Convert.ToByte(blue));
            }
            else
            {
                return Color.Red;
            }
        }

        private void updateDesignImage()
        {
            Bitmap tempImage = design.designToBitmap((float)settings.threadThickness, (settings.filterStiches), settings.filterStitchesThreshold);

            if (settings.transparencyGridEnabled)
            {
                DrawArea = new Bitmap(tempImage.Width, tempImage.Height);
                using (Graphics g = Graphics.FromImage(DrawArea))
                {
                    Color gridColor = settings.transparencyGridColor;
                    using (Pen gridPen = new Pen(gridColor))
                    {
                        int gridSize = settings.transparencyGridSize;
                        for (int xStart = 0; (xStart * gridSize) < DrawArea.Width; xStart++)
                        {
                            for (int yStart = 0; (yStart * gridSize) < DrawArea.Height; yStart++)
                            {
                                // Fill even columns in even rows and odd columns in odd rows
                                if ((xStart % 2) == (yStart % 2))
                                {
                                    g.FillRectangle(gridPen.Brush, (xStart * gridSize), (yStart * gridSize), gridSize, gridSize);
                                }
                            }
                        }
                    }

                    g.DrawImage(tempImage, 0, 0);
                }
            }
            else
            {
                DrawArea = tempImage;
            }

            panel1.Width = design.GetWidth() + (int)(settings.threadThickness * 2);
            panel1.Height = design.GetHeight() + (int)(settings.threadThickness * 2);
            panel1.Invalidate();
        }

        private void openFile(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                return;
            }
            design = new PesFile.PesFile(filename);
            if (design.getStatus() == PesFile.statusEnum.Ready)
            {
                this.Text = System.IO.Path.GetFileName(filename) + " - " + APP_TITLE;

                updateDesignImage();

                if (design.getFormatWarning())
                {
                    toolStripStatusLabel1.Text = translation.GetTranslatedString(Translation.StringID.UNSUPPORTED_FORMAT); // "The format of this file is not completely supported";
                }
                else if (design.getColorWarning())
                {
                    toolStripStatusLabel1.Text = translation.GetTranslatedString(Translation.StringID.COLOR_WARNING); // "Colors shown for this design may be inaccurate"
                }
                else
                {
                    toolStripStatusLabel1.Text = "";
                }
                copyToolStripMenuItem.Enabled = true;
                saveDebugInfoToolStripMenuItem.Enabled = true;
                printPreviewToolStripMenuItem.Enabled = true;
                printToolStripMenuItem.Enabled = true;
                rotateLeftToolStripMenuItem.Enabled = true;
                rotateRightToolStripMenuItem.Enabled = true;
                refreshToolStripMenuItem.Enabled = true;
                showDebugInfoToolStripMenuItem.Enabled = true;
                saveAsBitmapToolStripMenuItem.Enabled = true;
                panel2.Select();
            }
            else
            {
                string message = translation.GetTranslatedString(Translation.StringID.ERROR_FILE) + // "An error occured while reading the file:"
                    Environment.NewLine + design.getLastError();
                if (design.getStatus() == PesFile.statusEnum.ParseError)
                {
                    message += Environment.NewLine + translation.GetTranslatedString(Translation.StringID.CORRUPT_FILE); // "This file is either corrupt or not a valid PES file."
                }
                MessageBox.Show(message);
                copyToolStripMenuItem.Enabled = false;
                saveDebugInfoToolStripMenuItem.Enabled = false;
                printPreviewToolStripMenuItem.Enabled = false;
                printToolStripMenuItem.Enabled = false;
                rotateLeftToolStripMenuItem.Enabled = false;
                rotateRightToolStripMenuItem.Enabled = false;
                refreshToolStripMenuItem.Enabled = false;
                showDebugInfoToolStripMenuItem.Enabled = false;
                saveAsBitmapToolStripMenuItem.Enabled = false;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename;
            if (settings.lastOpenFileFolder != null)
            {
                openFileDialog1.InitialDirectory = settings.lastOpenFileFolder;
            }
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = translation.GetTranslatedString(Translation.StringID.FILE_TYPE_PES) + " (*.pes)|*.pes|" + // "Embroidery Files (*.pes)|*.pes|
                translation.GetTranslatedString(Translation.StringID.FILE_TYPE_ALL) + " (*.*)|*.*"; // All Files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName;
                if (!System.IO.File.Exists(filename))
                {
                    return;
                }
                else
                {
                    settings.lastOpenFileFolder = System.IO.Path.GetDirectoryName(filename);
                    openFile(filename);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (DrawArea != null)
            {
                e.Graphics.DrawImage(DrawArea, 0, 0);
                //e.Graphics.DrawImage(DrawArea, e.Graphics.ClipBounds, e.Graphics.ClipBounds, GraphicsUnit.Pixel);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(String.Format(translation.GetTranslatedString(Translation.StringID.ABOUT_MESSAGE), currentVersion())); // "EmbroideryReader version " + currentVersion() + ". This program reads and displays embroidery designs from .PES files."
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NJCrawford.UpdateCheck updater = new NJCrawford.UpdateCheck(settings.updateLocation, null, null);
            updater.waitForInfo();

            if (updater.GetLastError() != "")
            {
                MessageBox.Show(translation.GetTranslatedString(Translation.StringID.ERROR_UPDATE) + // "Encountered an error while checking for updates: "
                    updater.GetLastError());
            }
            else if (updater.IsUpdateAvailable())
            {
                if (MessageBox.Show(String.Format(translation.GetTranslatedString(Translation.StringID.NEW_VERSION_MESSAGE),
                    updater.VersionAvailable(), updater.getReleaseDate().ToShortDateString(), currentVersion()) + // "Version " + updater.VersionAvailable() + " was released on " + updater.getReleaseDate().ToShortDateString() + ". You have version " + currentVersion() + "."
                    Environment.NewLine +
                    translation.GetTranslatedString(Translation.StringID.NEW_VERSION_QUESTION), // "Would you like to go to the Embroidery Reader website to download or find out more about the new version?",
                    translation.GetTranslatedString(Translation.StringID.NEW_VERSION_TITLE), // "New version available",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(updater.getMoreInfoURL());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(translation.GetTranslatedString(Translation.StringID.ERROR_WEBPAGE) + // "An error occured while trying to open the webpage:"
                             Environment.NewLine + ex.ToString());
                    }
                }
            }
            else
            {
                MessageBox.Show(translation.GetTranslatedString(Translation.StringID.NO_UPDATE) + // "No updates are available right now."
                     Environment.NewLine + 
                     String.Format(translation.GetTranslatedString(Translation.StringID.LATEST_VERSION),
                     updater.VersionAvailable(), currentVersion())); // "(Latest version is " + updater.VersionAvailable() + ", you have version " + currentVersion() + ")");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            settings.windowWidth = this.Width;
            settings.windowHeight = this.Height;
            settings.save();
        }

        private string currentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void saveDebugInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (design != null)
            {
                try
                {
                    string debugFile = design.saveDebugInfo();
                    MessageBox.Show(String.Format(translation.GetTranslatedString(Translation.StringID.DEBUG_INFO_SAVED), // "Saved debug info to " + debugFile
                        debugFile));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(translation.GetTranslatedString(Translation.StringID.ERROR_DEBUG) + // "There was an error while saving debug info:"
                        Environment.NewLine + ex.ToString());
                }
            }
            else
            {
                MessageBox.Show(translation.GetTranslatedString(Translation.StringID.NO_DESIGN)); // "No design loaded."
            }
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSettingsDialog tempForm = new frmSettingsDialog();
            tempForm.settingsToModify = settings;
            tempForm.setTranslation = translation;
            if (tempForm.ShowDialog() == DialogResult.OK)
            {
                settings = tempForm.settingsToModify;
                checkSettings();
            }
            loadTranslatedStrings(settings.translation);
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            if (DrawArea != null)
            {
                float dpiX = 100;
                float dpiY = 100;
                double inchesPerMM = 0.03937007874015748031496062992126;
                e.Graphics.ScaleTransform((float)(dpiX * inchesPerMM * 0.1), (float)(dpiY * inchesPerMM * 0.1));
                Bitmap tempDrawArea = design.designToBitmap((float)settings.threadThickness, (settings.filterStiches), settings.filterStitchesThreshold);
                e.Graphics.DrawImage(DrawArea, 30, 30);
            }
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.ShowDialog();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DrawArea != null)
            {
                Clipboard.Clear();
                Bitmap temp = new Bitmap(DrawArea.Width, DrawArea.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Graphics tempGraph = Graphics.FromImage(temp);
                tempGraph.FillRectangle(Brushes.White, 0, 0, temp.Width, temp.Height);
                tempGraph.DrawImageUnscaled(DrawArea, 0, 0);
                tempGraph.Dispose();
                Clipboard.SetImage(temp);
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (design != null && design.getStatus() == PesFile.statusEnum.Ready)
            {
                updateDesignImage();

                if (design.getClassWarning())
                {
                    toolStripStatusLabel1.Text = translation.GetTranslatedString(Translation.StringID.UNSUPPORTED_CLASS); // "This file contains a class that is not yet supported"
                }
                else if (design.getFormatWarning())
                {
                    toolStripStatusLabel1.Text = translation.GetTranslatedString(Translation.StringID.UNSUPPORTED_FORMAT); // "The format of this file is not completely supported"
                }
                else if (design.getColorWarning())
                {
                    toolStripStatusLabel1.Text = translation.GetTranslatedString(Translation.StringID.COLOR_WARNING); // "Colors shown for this design may be inaccurate"
                }
                else
                {
                    toolStripStatusLabel1.Text = "";
                }
            }
        }

        private void rotateLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap temp = new Bitmap(DrawArea.Height, DrawArea.Width);
            Graphics g = Graphics.FromImage(temp);
            g.RotateTransform(270.0f);
            g.DrawImage(DrawArea, -DrawArea.Width, 0);
            g.Dispose();
            DrawArea = temp;
            int temp2 = panel1.Width;
            panel1.Width = panel1.Height;
            panel1.Height = temp2;
            panel1.Invalidate();
        }

        private void rotateRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap temp = new Bitmap(DrawArea.Height, DrawArea.Width);
            Graphics g = Graphics.FromImage(temp);
            g.RotateTransform(90.0f);
            g.DrawImage(DrawArea, 0, -DrawArea.Height);
            g.Dispose();
            DrawArea = temp;
            int temp2 = panel1.Width;
            panel1.Width = panel1.Height;
            panel1.Height = temp2;
            panel1.Invalidate();
        }

        private void showDebugInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (design != null)
            {
                try
                {
                    frmTextbox theform = new frmTextbox();
                    theform.showText(design.getDebugInfo());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(translation.GetTranslatedString(Translation.StringID.ERROR_DEBUG) + // "There was an error while saving debug info:"
                        Environment.NewLine + ex.ToString());
                }
            }
            else
            {
                MessageBox.Show(translation.GetTranslatedString(Translation.StringID.NO_DESIGN)); // "No design loaded."
            }
        }

        private void saveAsBitmapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DrawArea != null)
            {
                Bitmap temp = new Bitmap(DrawArea.Width, DrawArea.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                Graphics tempGraph = Graphics.FromImage(temp);
                tempGraph.FillRectangle(Brushes.White, 0, 0, temp.Width, temp.Height);
                tempGraph.DrawImageUnscaled(DrawArea, 0, 0);
                tempGraph.Dispose();
                saveFileDialog1.FileName = "";
                // "Bitmap (*.bmp)|*.bmp|PNG (*.png)|*.png|JPEG (*.jpg)|*.jpg|GIF (*.gif)|*.gif|TIFF (*.tif)|*.tif|All Files (*.*)|*.*"
                saveFileDialog1.Filter = translation.GetTranslatedString(Translation.StringID.FILE_TYPE_BMP) + " (*.bmp)|*.bmp|" + 
                    translation.GetTranslatedString(Translation.StringID.FILE_TYPE_PNG) + " (*.png)|*.png|" +
                    translation.GetTranslatedString(Translation.StringID.FILE_TYPE_JPG) + " (*.jpg)|*.jpg|" +
                    translation.GetTranslatedString(Translation.StringID.FILE_TYPE_GIF) + " (*.gif)|*.gif|" +
                    translation.GetTranslatedString(Translation.StringID.FILE_TYPE_TIFF) + " (*.tif)|*.tif|" +
                    translation.GetTranslatedString(Translation.StringID.FILE_TYPE_ALL) + " (*.*)|*.*";
                if (settings.lastSaveImageLocation != null)
                {
                    saveFileDialog1.InitialDirectory = settings.lastSaveImageLocation;
                }
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string filename = "";
                    filename = saveFileDialog1.FileName;
                    System.Drawing.Imaging.ImageFormat format;
                    switch (System.IO.Path.GetExtension(filename).ToLower())
                    {
                        case ".bmp": format = System.Drawing.Imaging.ImageFormat.Bmp; break;
                        case ".png": format = System.Drawing.Imaging.ImageFormat.Png; break;
                        case ".jpg": format = System.Drawing.Imaging.ImageFormat.Jpeg; break;
                        case ".gif": format = System.Drawing.Imaging.ImageFormat.Gif; break;
                        case ".tif": format = System.Drawing.Imaging.ImageFormat.Tiff; break;
                        default: format = System.Drawing.Imaging.ImageFormat.Bmp; break;
                    }
                    temp.Save(filename, format);
                    showStatus(translation.GetTranslatedString(Translation.StringID.IMAGE_SAVED), 5000); // "Image saved"
                    settings.lastSaveImageLocation = System.IO.Path.GetDirectoryName(filename);
                }
            }
        }

        private void showStatus(string text, int msec)
        {
            toolStripStatusLabel2.Text = text;
            timer1.Interval = msec;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel2.Text = "";
        }

        private void loadTranslatedStrings(String translationName)
        {
            translation = new Translation(translationName);

            // File menu
            fileToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.MENU_FILE);
            openToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.MENU_OPEN);
            saveAsBitmapToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.MENU_SAVE_IMAGE);
            printToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.MENU_PRINT);
            printPreviewToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.MENU_PRINT_PREVIEW);
            exitToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.MENU_EXIT);

            // Edit menu
            editToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.MENU_EDIT);
            copyToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.MENU_COPY);
            preferencesToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.MENU_PREFS);

            // View menu
            viewToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.MENU_VIEW);
            rotateLeftToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.ROTATE_LEFT);
            rotateRightToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.ROTATE_RIGHT);
            refreshToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.MENU_RESET);

            // Help menu
            helpToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.MENU_HELP);
            checkForUpdateToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.CHECK_UPDATE);
            saveDebugInfoToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.SAVE_DEBUG);
            showDebugInfoToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.SHOW_DEBUG);
            aboutToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.MENU_ABOUT);
        }
    }
}
