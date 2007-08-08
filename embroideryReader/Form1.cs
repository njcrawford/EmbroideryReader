using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace embroideryReader
{
    public partial class Form1 : Form
    {
        private string[] args;
        //private Form2 _form2;
        public Pen drawPen = Pens.Black;
        public Bitmap DrawArea;
        public PesFile design;
        private SettingsTester.nc_Settings settings = new SettingsTester.nc_Settings("embroideryreader.ini");
        //private PesFile design;
        //private long bytesRead = 0;
        //System.IO.BinaryReader fileIn;
        //Random rnd = new Random();
        //int stitchCount = 0;
        //int stitchesLeft = 0;
        //int skipStitches = 0;
        //int minX = int.MaxValue;
        //int minY = int.MaxValue;
        //int maxX = int.MinValue;
        //int maxY = int.MinValue;
        //int imageWidth;
        //int imageHeight;
        //int lastStrangeNum=-1;


        public Form1()
        {
            InitializeComponent();
            args = Environment.GetCommandLineArgs();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //System.IO.BinaryReader fileIn;
            //char charIn;
            //string message = "";
            //string filename;
            //openFileDialog1.ShowDialog();
            //filename = openFileDialog1.FileName;
            //if (!System.IO.File.Exists(filename))
            //{
            //    return;
            //}
            ////fileIn = new System.IO.BinaryReader(System.IO.File.Open("118866.pes", System.IO.FileMode.Open));
            ////fileIn = new System.IO.BinaryReader(System.IO.File.Open("144496.pes", System.IO.FileMode.Open));
            //fileIn = new System.IO.BinaryReader(System.IO.File.Open(filename, System.IO.FileMode.Open));
            ////charIn = fileIn.ReadChar();
            //for (int i = 0; i < 8; i++)//8 bytes
            //{
            //    message += fileIn.ReadChar();
            //    bytesRead++;
            //}
            //message += Environment.NewLine;
            //for (int i = 0; i < 8; i++)//16 bytes
            //{
            //    message += fileIn.ReadInt16().ToString();
            //    message += Environment.NewLine;
            //    bytesRead += 2;
            //}
            //for (int i = 0; i < 7; i++)//7 bytes
            //{
            //    message += fileIn.ReadChar();

            //    bytesRead++;
            //}
            //message += Environment.NewLine;
            ////MessageBox.Show(message);

            ////message = "";
            //int headerValNum = 1;
            //int tmpval;
            //for (int i = 0; i < 33; i++) //read 66 bytes
            //{
            //    tmpval = fileIn.ReadInt16();
            //    switch (headerValNum)
            //    {
            //        case 24:
            //            imageWidth = tmpval;
            //            break;
            //        case 25:
            //            imageHeight = tmpval;
            //            break;
            //    }

            //    message += tmpval.ToString();
            //    if (headerValNum % 3 == 0)
            //    {
            //        message += Environment.NewLine;
            //    }
            //    else
            //    {
            //        message += "\t| ";
            //    }
            //    headerValNum++;
            //    //message += "\t| ";
            //    //message += fileIn.ReadInt16().ToString();
            //    //message += "\t| ";
            //    //message += fileIn.ReadInt16().ToString();
            //    //message += Environment.NewLine;
            //    //bytesRead += 6;
            //}
            ////MessageBox.Show(message);

            ////message = "";
            //for (int i = 0; i < 7; i++)//7 bytes
            //{
            //    message += fileIn.ReadChar();
            //    bytesRead++;
            //}
            ////MessageBox.Show(message);

            ////message = "";
            //MessageBox.Show(fileIn.BaseStream.Position.ToString());
            //for (int i = 0; i < 7; i++)//14 bytes
            //{
            //    message += fileIn.ReadInt16();
            //    message += Environment.NewLine;
            //    bytesRead += 2;
            //}
            //MessageBox.Show(message);

            ////start of point pairs
            //message = "";
            //long startPos = fileIn.BaseStream.Position;
            //bytesRead = fileIn.BaseStream.Position;
            //_form2 = new Form2();
            //string filename;
            //openFileDialog1.ShowDialog();
            //filename = openFileDialog1.FileName;
            //if (!System.IO.File.Exists(filename) )
            //{
            //    return;
            //}
            //_form2.design = new PesFile(filename);
            //_form2.DrawArea = new Bitmap(_form2.design.GetWidth(), _form2.design.GetHeight());
            //_form2.setPanelSize(_form2.design.GetWidth(), _form2.design.GetHeight());
            ////_form2.Width = imageWidth + 30;
            ////_form2.Height = imageHeight + 45;

            //_form2.Show();
            //_form2.finishDesign();
            //_form2.drawColor = Color.FromArgb((rnd.Next(0, 255)), (rnd.Next(0, 255)), (rnd.Next(0, 255)));
            //_form2.drawPen = new Pen(Color.FromArgb((rnd.Next(0, 255)), (rnd.Next(0, 255)), (rnd.Next(0, 255))),4);
            string filename;
            openFileDialog1.ShowDialog();
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

        private void button2_Click(object sender, EventArgs e)
        {
            //_form2.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //_form2.nextStitch();
            //_form2.Invalidate();
        }

        //private void nextStitch()
        //{
        //    int tmpx;
        //    int tmpy;
        //    Int32 realx;
        //    Int32 realy;
        //    if (fileIn.BaseStream.Position + 4 < fileIn.BaseStream.Length)
        //    {
        //        realx = fileIn.ReadInt16();
        //        realy = fileIn.ReadInt16();
        //        if (realx == -32765)
        //        {

        //            if (realy == 1) //probably means change color
        //            {
        //                //timer1.Enabled = false;
        //                //colorDialog1.ShowDialog();
        //                //_form2.drawColor = colorDialog1.Color;
        //                _form2.drawPen = new Pen(Color.FromArgb((rnd.Next(0, 255)), (rnd.Next(0, 255)), (rnd.Next(0, 255))),4);
        //                _form2.Invalidate();
        //                _form2.prevPoint = new Point(-1, -1);
        //            }
        //            lastStrangeNum= fileIn.ReadInt16();//don't know what this is, maybe color index?
        //            timer1.Interval = lastStrangeNum*10;
        //            stitchesLeft = fileIn.ReadInt16();
        //            if (realy == 1)
        //            {
        //                skipStitches = stitchesLeft;//skip these stiches, since they just seem to get in the way
        //            }
        //        }
        //        else
        //        {
        //            tmpx = realx;//x is ok
        //            tmpy = realy + imageHeight;//y needs to be translated
        //            bytesRead += 4;
        //            if (skipStitches > 0)
        //            {
        //                skipStitches--;
        //            }
        //            else
        //            {
        //                _form2.addPoint(new Point(tmpx, tmpy));
        //                if (realx < minX)
        //                {
        //                    minX = realx;
        //                }
        //                if (realx > maxX)
        //                {
        //                    maxX = realx;
        //                }
        //                if (realy < minY)
        //                {
        //                    minY = realy;
        //                }
        //                if (realy > maxY)
        //                {
        //                    maxY = realy;
        //                }
        //            }
        //            stitchCount++;
        //            stitchesLeft--;
        //        }
        //        label1.Text = "file pos: " + fileIn.BaseStream.Position.ToString() + ", last values: " + realx.ToString() + ", " + realy.ToString() + ", total stiches: " + stitchCount.ToString() + ", stitches til next section: " + (stitchesLeft + 1).ToString() + ", min, max vals: " + minX.ToString() + "," + minY.ToString() + ";" + maxX.ToString() + "," + maxY.ToString() + ", strangenum: " + lastStrangeNum;
        //        if (stitchesLeft < 0)
        //        {
        //            timer1.Enabled = false;
        //            fileIn.Close();
        //        }
        //    }
        // }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //_form2.nextStitch();
            //_form2.Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //timer1.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //timer1.Enabled = false;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //while (stitchesLeft >= 0)
            //{
            //    nextStitch();
            //}
            //_form2.finishDesign();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string temp;
            temp = settings.getValue("update location");
            if (temp == null || temp == "")
            {
                settings.setValue("update location", "http://www.njcrawford.com/embreader/");
            }
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            //MessageBox.Show(args.Length.ToString());
            if (args.Length > 1)
            {
                openFile(args[1]);
            }
        }
        private void openFile(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                return;
            }
            //_form2 = new Form2();
            //_form2.design = new PesFile(filename);
            design = new PesFile(filename);
            //_form2.DrawArea = new Bitmap(_form2.design.GetWidth(), _form2.design.GetHeight());
            DrawArea = new Bitmap(design.GetWidth(), design.GetHeight());
            setPanelSize(design.GetWidth(), design.GetHeight());

            //_form2.Show();
            finishDesign();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename;
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Embroidery Files (*.pes)|*.pes|All Files (*.*)|*.*";
            openFileDialog1.ShowDialog();
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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (DrawArea != null)
            {
                e.Graphics.DrawImage(DrawArea, 0, 0);
            }
        }
        public void setPanelSize(int x, int y)
        {
            panel1.Width = x;
            panel1.Height = y;
        }
        public void finishDesign()
        {
            Graphics xGraph;
            xGraph = Graphics.FromImage(DrawArea);
            for (int i = 0; i < design.blocks.Count; i++)
            {
                xGraph.DrawLines(new Pen(design.blocks[i].color, 2.5f), design.blocks[i].stitches);
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
            UpdateTester.nc_Update updater = new UpdateTester.nc_Update(settings.getValue("update location"));
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
            if(updater.IsUpdateAvailable())
            {
                if (MessageBox.Show("Version " + updater.VersionAvailable() + " is available.\nYou have version " + currentVersion() + ". Would you like to update?\n(If you choose Yes, the update will be downloaded and installed, and the program will be restarted)", "New version available", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (!updater.InstallUpdate())
                    {
                        MessageBox.Show("Update failed, error message: " + updater.GetLastError());
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                }
            }
            else if (updater.GetLastError() != "")
            {
                MessageBox.Show("Encountered an error while checking for updates: " + updater.GetLastError());
            }
            else
            {
                MessageBox.Show("No updates are available right now. (Latest version is "+updater.VersionAvailable()+")");
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
            return Assembly.GetCallingAssembly().GetName().Version.ToString();
        }
    }
}