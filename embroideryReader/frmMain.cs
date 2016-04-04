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
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

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

        private float designScale = 1.0f;
        private Size panel2LastUpdateSize;
        private bool maximizeChanged = false;
        private int designRotation = 0;
        private string loadedFileName = "";

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
            if (settings.windowMaximized)
            {
                // Check maximized first
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                // Not maximized, restore last saved window size
                this.Width = settings.windowWidth;
                this.Height = settings.windowHeight;
            }
            setDesignScaleSetting(1.0f, settings.AutoScaleDesign, false);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Set app window title
            this.Text = APP_TITLE;

            // Load and check settings
            checkSettings();
            
            // Load translation
            loadTranslatedStrings(settings.translation);

            // Load design, if specified
            if (args.Length > 1)
            {
                openFile(args[1]);
            }
        }

        // WndProc values
        // See https://msdn.microsoft.com/en-us/library/windows/desktop/ms646360(v=vs.85).aspx for more info
        const int WM_SYSCOMMAND = 0x0112;
        const int SC_MAXIMIZE = 0xF030;
        const int SC_MINIMIZE = 0xF020;
        const int SC_RESTORE = 0xF120;
        const int SC_WPARAM_MASK = 0xFFF0;

        // Override WndProc to capture maximize and restore events
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SYSCOMMAND)
            {
                // Check your window state here
                int maskedWParam = m.WParam.ToInt32() & SC_WPARAM_MASK;
                if (maskedWParam == SC_MAXIMIZE ||
                    maskedWParam == SC_MINIMIZE ||
                    maskedWParam == SC_RESTORE)
                {
                    maximizeChanged = true;
                }
            }
            base.WndProc(ref m);
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
                catch (FormatException /* ex */)
                {
                    retval = false;
                }
                catch (OverflowException /* ex */)
                {
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
            if(design == null)
            {
                // No design loaded - nothing to update
                return;
            }

            Bitmap tempImage = design.designToBitmap((float)settings.threadThickness, (settings.filterStiches), settings.filterStitchesThreshold, 1.0f);

            // Rotate image
            switch (designRotation)
            {
                case 90:
                    tempImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 180:
                    tempImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 270:
                    tempImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }

            // Scale image
            if (fitToWindowToolStripMenuItem.Checked)
            {
                // Calculate size of image based on available drawing area
                float windowWidth = panel2.Width - 3;
                float windowHeight = panel2.Height - 3;
                
                // Figure out which dimension is more constrained
                float widthScale = windowWidth / tempImage.Width;
                float heightScale = windowHeight / tempImage.Height;
                if (widthScale < heightScale)
                {
                    designScale = widthScale;
                }
                else
                {
                    designScale = heightScale;
                }
            }

            int width = (int)(tempImage.Width * designScale);
            int height = (int)(tempImage.Height * designScale);

            if (width < 1 || height < 1)
            {
                // Image area is too small to update
                return;
            }

            if (width != tempImage.Width || height != tempImage.Height)
            {
                // Scale image code from http://stackoverflow.com/questions/1922040/resize-an-image-c-sharp
                Rectangle destRect = new Rectangle(0, 0, width, height);
                Bitmap destImage = new Bitmap(width, height);

                destImage.SetResolution(tempImage.HorizontalResolution, tempImage.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(tempImage, destRect, 0, 0, tempImage.Width, tempImage.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }
                // Keep the scaled image and toss the intermediate image
                tempImage = destImage;
            }

            // About to abandon the current DrawArea object, dispose it now
            if(DrawArea != null)
            {
                DrawArea.Dispose();
                DrawArea = null;
            }

            // Add transparency grid
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

                    // Done with tempImage
                    tempImage.Dispose();
                    tempImage = null;
                }
            }
            else
            {
                // Keeping the object tempImage was pointing at, so don't dispose it
                DrawArea = tempImage;
            }

            panel1.Width = DrawArea.Width;
            panel1.Height = DrawArea.Height;
            panel1.Invalidate();

            panel2LastUpdateSize = panel2.Size;

            // Update window title
            this.Text = System.IO.Path.GetFileName(loadedFileName) + " (" + (designScale * 100).ToString("0") + "%) - " + APP_TITLE;
        }

        private void openFile(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                // "An error occured while reading the file:"
                MessageBox.Show(Translation.StringID.ERROR_FILE + Environment.NewLine + "File \"" + filename + "\" does not exist", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                design = new PesFile.PesFile(filename);
            }
            catch(System.IO.IOException ioex)
            {
                // "An error occured while reading the file:"
                MessageBox.Show(Translation.StringID.ERROR_FILE + Environment.NewLine + filename + ":" + Environment.NewLine + ioex.Message, "IOException", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                design = null;
            }
            catch(PesFile.PECFormatException pecex)
            {
                // "This file is either corrupt or not a valid PES file."
                MessageBox.Show(Translation.StringID.ERROR_FILE + Environment.NewLine + filename + ":" + Environment.NewLine + pecex.Message, "PECFormatException", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                design = null;
            }

            loadedFileName = filename;
            if (design != null)
            {
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
                zoomToolStripMenuItem.Enabled = true;
                showDebugInfoToolStripMenuItem.Enabled = true;
                saveAsBitmapToolStripMenuItem.Enabled = true;
                panel2.Select();
            }
            else
            {
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
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = String.Format(translation.GetTranslatedString(Translation.StringID.ABOUT_MESSAGE), currentVersion()); // "EmbroideryReader version " + currentVersion() + ". This program reads and displays embroidery designs from .PES files."
            message += Environment.NewLine + Environment.NewLine + "GUI GDI count: " + GuiResources.GetGuiResourcesGDICount();
            message += Environment.NewLine + "GUI USER count: " + GuiResources.GetGuiResourcesUserCount();
            message += Environment.NewLine + ".Net framework version: " + Environment.Version;
            MessageBox.Show(message);
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EmbroideryReaderUpdates updater = new EmbroideryReaderUpdates(settings.updateLocation, null, null);
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
            if (this.WindowState == FormWindowState.Maximized)
            {
                settings.windowMaximized = true;
            }
            else
            {
                settings.windowMaximized = false;

                if (this.WindowState == FormWindowState.Normal)
                {
                    // If window is not minimized or maximized, save current size
                    settings.windowWidth = this.Width;
                    settings.windowHeight = this.Height;
                }
            }
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
            if (design != null)
            {
                float inchesPerMM = 0.03937007874015748031496062992126f;
                e.Graphics.ScaleTransform((float)(e.PageSettings.PrinterResolution.X * inchesPerMM * 0.01f), (float)(e.PageSettings.PrinterResolution.Y * inchesPerMM * 0.01f));
                using (Bitmap tempDrawArea = design.designToBitmap((float)settings.threadThickness, settings.filterStiches, settings.filterStitchesThreshold, e.PageSettings.PrinterResolution.X * inchesPerMM * 0.2f))
                {
                    e.Graphics.DrawImage(tempDrawArea, 30, 30);
                }
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
                using (Bitmap temp = new Bitmap(DrawArea.Width, DrawArea.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                {
                    using (Graphics tempGraph = Graphics.FromImage(temp))
                    {
                        tempGraph.FillRectangle(Brushes.White, 0, 0, temp.Width, temp.Height);
                        tempGraph.DrawImageUnscaled(DrawArea, 0, 0);
                    }
                    Clipboard.SetImage(temp);
                }
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            designRotation = 0;
            updateDesignImage();
        }

        private void rotateLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            designRotation -= 90;
            if(designRotation < 0)
            {
                designRotation += 360;
            }
            updateDesignImage();
        }

        private void rotateRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            designRotation += 90;
            if (designRotation >= 360)
            {
                designRotation -= 360;
            }
            updateDesignImage();
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
                using (Bitmap temp = new Bitmap(DrawArea.Width, DrawArea.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb))
                {
                    using (Graphics tempGraph = Graphics.FromImage(temp))
                    {
                        tempGraph.FillRectangle(Brushes.White, 0, 0, temp.Width, temp.Height);
                        tempGraph.DrawImageUnscaled(DrawArea, 0, 0);
                    }
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
            zoomToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.MENU_SCALE_ZOOM);
            fitToWindowToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.MENU_FIT_TO_WINDOW);

            // Help menu
            helpToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.MENU_HELP);
            checkForUpdateToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.CHECK_UPDATE);
            saveDebugInfoToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.SAVE_DEBUG);
            showDebugInfoToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.SHOW_DEBUG);
            aboutToolStripMenuItem.Text = translation.GetTranslatedString(Translation.StringID.MENU_ABOUT);
        }

        private void setDesignScaleSetting(float scale, bool autoSize, bool updateDesign)
        {
            if(autoSize)
            {
                fitToWindowToolStripMenuItem.Checked = true;
                settings.AutoScaleDesign = true;
            }
            else
            {
                fitToWindowToolStripMenuItem.Checked = false;
                settings.AutoScaleDesign = false;
                designScale = scale;
            }
            
            if (updateDesign)
            {
                updateDesignImage();
            }
        }

        private void scale100ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setDesignScaleSetting(1.0f, false, true);
        }

        private void scale90ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setDesignScaleSetting(0.9f, false, true);
        }

        private void scale80ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setDesignScaleSetting(0.8f, false, true);
        }

        private void scale70ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setDesignScaleSetting(0.7f, false, true);
        }

        private void scale60ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setDesignScaleSetting(0.6f, false, true);
        }

        private void scale50ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setDesignScaleSetting(0.5f, false, true);
        }

        private void scale40ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setDesignScaleSetting(0.4f, false, true);
        }

        private void scale30ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setDesignScaleSetting(0.3f, false, true);
        }

        private void scale20ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setDesignScaleSetting(0.2f, false, true);
        }

        private void scale10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setDesignScaleSetting(0.1f, false, true);
        }

        private void scale5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setDesignScaleSetting(0.05f, false, true);
        }

        private void fitToWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Toggle checked state
            fitToWindowToolStripMenuItem.Checked = !fitToWindowToolStripMenuItem.Checked;
            // Update design
            setDesignScaleSetting(1.0f, fitToWindowToolStripMenuItem.Checked, true);
        }

        private void frmMain_ResizeEnd(object sender, EventArgs e)
        {
            // Finished resize, update design scale if set to fit-to-window.
            // This event also captures window move events, so check if the size
            // of panel2 has changed to see if it's really a resize event.
            if (fitToWindowToolStripMenuItem.Checked && panel2LastUpdateSize != panel2.Size)
            {
                updateDesignImage();
            }
        }

        private void panel2_Resize(object sender, EventArgs e)
        {
            // This event is the one we really want, but it fires much too 
            // frequently during a resize event. So, let ReziseEnd handle most
            // of the cases and only process this after a window maximize or
            // restore.
            if(maximizeChanged)
            {
                maximizeChanged = false;
                
                if (fitToWindowToolStripMenuItem.Checked)
                {
                    updateDesignImage();
                }
            }
        }
    }
}
