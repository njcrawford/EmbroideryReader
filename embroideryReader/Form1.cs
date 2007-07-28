using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace embroideryReader
{
    public partial class Form1 : Form
    {
        private Form2 _form2 = new Form2();
        private long bytesRead = 0;
        System.IO.BinaryReader fileIn;
        Random rnd = new Random();
        int stitchCount = 0;
        int stitchesLeft = 0;
        int skipStitches = 0;
        int minX = int.MaxValue;
        int minY = int.MaxValue;
        int maxX = int.MinValue;
        int maxY = int.MinValue;
        int imageWidth;
        int imageHeight;


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //System.IO.BinaryReader fileIn;
            //char charIn;
            string message = "";
            string filename;
            openFileDialog1.ShowDialog();
            filename = openFileDialog1.FileName;
            if (!System.IO.File.Exists(filename))
            {
                return;
            }
            //fileIn = new System.IO.BinaryReader(System.IO.File.Open("118866.pes", System.IO.FileMode.Open));
            //fileIn = new System.IO.BinaryReader(System.IO.File.Open("144496.pes", System.IO.FileMode.Open));
            fileIn = new System.IO.BinaryReader(System.IO.File.Open(filename, System.IO.FileMode.Open));
            //charIn = fileIn.ReadChar();
            for (int i = 0; i < 8; i++)//8 bytes
            {
                message += fileIn.ReadChar();
                bytesRead++;
            }
            message += Environment.NewLine;
            for (int i = 0; i < 8; i++)//16 bytes
            {
                message += fileIn.ReadInt16().ToString();
                message += Environment.NewLine;
                bytesRead += 2;
            }
            for (int i = 0; i < 7; i++)//7 bytes
            {
                message += fileIn.ReadChar();

                bytesRead++;
            }
            message += Environment.NewLine;
            //MessageBox.Show(message);

            //message = "";
            int headerValNum = 1;
            int tmpval;
            for (int i = 0; i < 33; i++) //read 66 bytes
            {
                tmpval = fileIn.ReadInt16();
                switch (headerValNum)
                {
                    case 24:
                        imageWidth = tmpval;
                        break;
                    case 25:
                        imageHeight = tmpval;
                        break;
                }

                message += tmpval.ToString();
                if (headerValNum % 3 == 0)
                {
                    message += Environment.NewLine;
                }
                else
                {
                    message += "\t| ";
                }
                headerValNum++;
                //message += "\t| ";
                //message += fileIn.ReadInt16().ToString();
                //message += "\t| ";
                //message += fileIn.ReadInt16().ToString();
                //message += Environment.NewLine;
                //bytesRead += 6;
            }
            //MessageBox.Show(message);

            //message = "";
            for (int i = 0; i < 7; i++)//7 bytes
            {
                message += fileIn.ReadChar();
                bytesRead++;
            }
            //MessageBox.Show(message);

            //message = "";
            MessageBox.Show(fileIn.BaseStream.Position.ToString());
            for (int i = 0; i < 7; i++)//14 bytes
            {
                message += fileIn.ReadInt16();
                message += Environment.NewLine;
                bytesRead += 2;
            }
            MessageBox.Show(message);

            //start of point pairs
            message = "";
            long startPos = fileIn.BaseStream.Position;
            bytesRead = fileIn.BaseStream.Position;
            _form2.DrawArea = new Bitmap(imageWidth, imageHeight);
            _form2.Width = imageWidth + 30;
            _form2.Height = imageHeight + 45;
            _form2.Show();
            //_form2.drawColor = Color.FromArgb((rnd.Next(0, 255)), (rnd.Next(0, 255)), (rnd.Next(0, 255)));
            _form2.drawPen = new Pen(Color.FromArgb((rnd.Next(0, 255)), (rnd.Next(0, 255)), (rnd.Next(0, 255))),2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _form2.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            nextStitch();
        }

        private void nextStitch()
        {
            int tmpx;
            int tmpy;
            Int32 realx;
            Int32 realy;
            if (fileIn.BaseStream.Position + 4 < fileIn.BaseStream.Length)
            {
                realx = fileIn.ReadInt16();
                realy = fileIn.ReadInt16();
                if (realx == -32765)
                {

                    if (realy == 1) //probably means change color
                    {
                        //timer1.Enabled = false;
                        //colorDialog1.ShowDialog();
                        //_form2.drawColor = colorDialog1.Color;
                        _form2.drawPen = new Pen(Color.FromArgb((rnd.Next(0, 255)), (rnd.Next(0, 255)), (rnd.Next(0, 255))),2);
                        _form2.Invalidate();
                        _form2.prevPoint = new Point(-1, -1);
                    }
                    fileIn.ReadInt16();//don't know what this is, maybe color index?
                    stitchesLeft = fileIn.ReadInt16();
                    if (realy == 1)
                    {
                        skipStitches = stitchesLeft;//skip these stiches, since they just seem to get in the way
                    }
                }
                else
                {
                    tmpx = realx;//x is ok
                    tmpy = realy + imageHeight;//y needs to be translated
                    bytesRead += 4;
                    if (skipStitches > 0)
                    {
                        skipStitches--;
                    }
                    else
                    {
                        _form2.addPoint(new Point(tmpx, tmpy));
                        if (realx < minX)
                        {
                            minX = realx;
                        }
                        if (realx > maxX)
                        {
                            maxX = realx;
                        }
                        if (realy < minY)
                        {
                            minY = realy;
                        }
                        if (realy > maxY)
                        {
                            maxY = realy;
                        }
                    }
                    stitchCount++;
                    stitchesLeft--;
                }
                label1.Text = "file pos: " + fileIn.BaseStream.Position.ToString() + ", last values: " + realx.ToString() + ", " + realy.ToString() + ", total stiches: " + stitchCount.ToString() + ", stitches til next section: " + (stitchesLeft + 1).ToString() + ", min, max vals: " + minX.ToString() + "," + minY.ToString() + ";" + maxX.ToString() + "," + maxY.ToString();
                if (stitchesLeft < 0)
                {
                    timer1.Enabled = false;
                    fileIn.Close();
                }
            }
         }

        private void timer1_Tick(object sender, EventArgs e)
        {
            nextStitch();
            _form2.Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            while (stitchesLeft >= 0)
            {
                nextStitch();
            }
        }

    }
}