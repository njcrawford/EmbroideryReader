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
        private nc_settings.IniFile settings = new nc_settings.IniFile("embroideryreader.ini");


        public frmMain()
        {
            InitializeComponent();
            args = Environment.GetCommandLineArgs();
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    string filename;
        //    openFileDialog1.ShowDialog();
        //    filename = openFileDialog1.FileName;
        //    if (!System.IO.File.Exists(filename))
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        openFile(filename);
        //    }
        //}

        private void loadSettings()
        {
            string updateLoc;
            updateLoc = settings.getValue("update location");
            if (updateLoc == null || updateLoc.Length == 0)
            {
                settings.setValue("update location", "http://www.njcrawford.com/embreader/");
            }
            if (settings.getValue("background color", "enabled") == "yes")
            {
                if (checkColorFromStrings(settings.getValue("background color", "red"),
                                          settings.getValue("background color", "green"),
                                          settings.getValue("background color", "blue")))
                {
                    this.BackColor = makeColorFromStrings(settings.getValue("background color", "red"),
                                                          settings.getValue("background color", "green"),
                                                          settings.getValue("background color", "blue"));
                }
                else
                {
                    this.BackColor = Color.FromKnownColor(KnownColor.Control);
                }
            }
            else
            {
                this.BackColor = Color.FromKnownColor(KnownColor.Control);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            loadSettings();
            this.Text = "Embroidery Reader";
            if (args.Length > 1)
            {
                openFile(args[1]);
            }
        }

        public static bool checkColorFromStrings(string red, string green, string blue)
        {
            //string redString;
            //string greenString;
            //string blueString;
            byte redByte;
            byte greenByte;
            byte blueByte;
            //red = settings.getValue("background color", "red");
            //greenString = settings.getValue("background color", "green");
            //blueString = settings.getValue("background color", "blue");
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
                    //this.BackColor = Color.FromArgb(redByte, greenByte, blueByte);
                    retval = true;
                }
                catch (Exception ex)
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

        private void openFile(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                return;
            }
            design = new PesFile.PesFile(filename);
            if (design.getStatus() == PesFile.statusEnum.Ready)
            {
                this.Text = System.IO.Path.GetFileName(filename) + " - Embroidery Reader";
                sizePanel2();

                designToBitmap();

                if (design.getColorWarning())
                {
                    toolStripStatusLabel1.Text = "Colors shown for this design may be inaccurate";
                }
                else
                {
                    toolStripStatusLabel1.Text = "";
                }
            }
            else
            {
                MessageBox.Show("An error occured while reading the file:" + Environment.NewLine + design.getLastError());
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename;
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Embroidery Files (*.pes)|*.pes|All Files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName;
                if (!System.IO.File.Exists(filename))
                {
                    return;
                }
                else
                {
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

        public void designToBitmap()
        {
            Graphics xGraph;
            Single threadThickness = 5;
            if (settings.getValue("thread thickness") != null)
            {
                try
                {
                    threadThickness = Convert.ToSingle(settings.getValue("thread thickness"));
                }
                catch (Exception ex)
                {
                }
            }
            DrawArea = new Bitmap(design.GetWidth() + (int)(threadThickness * 2), design.GetHeight() + (int)(threadThickness * 2));
            panel1.Width = design.GetWidth() + (int)(threadThickness * 2);
            panel1.Height = design.GetHeight() + (int)(threadThickness * 2);
            xGraph = Graphics.FromImage(DrawArea);
            xGraph.TranslateTransform(threadThickness, threadThickness);
            for (int i = 0; i < design.blocks.Count; i++)
            {
                if (design.blocks[i].stitches.Length > 1)//must have 2 points to make a line
                {
                    Pen tempPen = new Pen(design.blocks[i].color, threadThickness);
                    tempPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    tempPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    tempPen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                    xGraph.DrawLines(tempPen, design.blocks[i].stitches);
                }
            }
            xGraph.Dispose();
            panel1.Invalidate();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("EmbroideryReader version " + currentVersion() + ". This program reads and displays embroidery designs from .PES files.");
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //bool isNewerVersion = false;
            nc_Updater.IniFileUpdater updater = new nc_Updater.IniFileUpdater(settings.getValue("update location"));
            //UpdateTester.nc_Update updater = new UpdateTester.nc_Update("http://www.google.com/");
            //char[] sep = { '.' };
            //string[] upVersion = updater.VersionAvailable().Split(sep);
            //string[] curVersion = currentVersion().Split(sep);
            //for (int i = 0; i < 4; i++)
            //{
            //    if (Convert.ToInt32( upVersion[i]) > Convert.ToInt32(curVersion[i]))
            //    {
            //        isNewerVersion = true;
            //        break;
            //    }
            //}
            //if (isNewerVersion)
            if (updater.IsUpdateAvailable())
            {
                if (MessageBox.Show("Version " + updater.VersionAvailable() + " is available." + Environment.NewLine + "You have version " + currentVersion() + ". Would you like to go to the Embroidery Reader website to download it?", "New version available", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(settings.getValue("update location"));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occured while trying to open the webpage:" + Environment.NewLine + ex.ToString());
                    }
                    //if (!updater.InstallUpdate())
                    //{
                    //    MessageBox.Show("Update failed, error message: " + updater.GetLastError());
                    //}
                    //else
                    //{
                    //    Environment.Exit(0);
                    //}
                }
            }
            else if (updater.GetLastError() != "")
            {
                MessageBox.Show("Encountered an error while checking for updates: " + updater.GetLastError());
            }
            else
            {
                MessageBox.Show("No updates are available right now." + Environment.NewLine + "(Latest version is " + updater.VersionAvailable() + ", you have version " + currentVersion() + ")");
            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            settings.save();
        }

        private string currentVersion()
        {
            //Assembly myAsm = Assembly.GetCallingAssembly();
            //AssemblyName aName = myAsm.GetName();
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }



        //[ComImport]
        //[Guid("BB2E617C-0920-11d1-9A0B-00C04FC2D6C1")]
        //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        //public interface IExtractImage
        //{
        //    [PreserveSig]
        //    long GetLocation(
        //        [Out]
        //    IntPtr pszPathBuffer,
        //        int cch,
        //        ref int pdwPriority,
        //        ref SIZE prgSize,
        //        int dwRecClrDepth,
        //        ref int pdwFlags);

        //    [PreserveSig]
        //    int Extract([Out]IntPtr phBmpThumbnail);
        //}


        //[ComImport]
        //[Guid("953BB1EE-93B4-11d1-98A3-00C04FB687DA")]
        //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        //public interface IExtractImage2 : IExtractImage
        //{
        //    int GetDateStamp([In, Out]ref System.Runtime.InteropServices.ComTypes.FILETIME pDateStamp);
        //}

        //public struct SIZE
        //{
        //    public long cx;
        //    public long cy;
        //}

        private void saveDebugInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (design != null)
            {
                try
                {
                    design.saveDebugInfo();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("There was an error while saving debug info:" + Environment.NewLine + ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("No design loaded.");
            }
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            //panel2.Top = 32;
            //panel2.Left = 0;
            //panel2.Height = this.Height - 50;
            //panel2.Width = this.Width - 50;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //panel2.Height = this.Height - 75;
            //panel2.Width = this.Width-8;
            sizePanel2();
        }

        private void sizePanel2()
        {
            panel2.Height = this.Height - 73;
            panel2.Width = this.Width - 8;
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSettingsDialog tempForm = new frmSettingsDialog();
            tempForm.settings = settings;
            if (tempForm.ShowDialog() == DialogResult.OK)
            {
                settings = tempForm.settings;
                loadSettings();
            }
        }
    }
}