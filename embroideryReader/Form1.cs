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


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //System.IO.BinaryReader fileIn;
            //char charIn;
            string message = "";
            fileIn = new System.IO.BinaryReader(System.IO.File.Open("118866.pes", System.IO.FileMode.Open));
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
            for (int i = 0; i < 11; i++) //read 66 bytes
            {
                message += fileIn.ReadInt16().ToString();
                message += "\t| ";
                message += fileIn.ReadInt16().ToString();
                message += "\t| ";
                message += fileIn.ReadInt16().ToString();
                message += Environment.NewLine;
                bytesRead += 6;
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

            //start of point pairs?
            message = "";
            //int tmpx;
            //int tmpy;
            //List<Point> tmpPoints = new List<Point>();
            long startPos = fileIn.BaseStream.Position;
            bytesRead = fileIn.BaseStream.Position;
            //MessageBox.Show(fileIn.BaseStream.Position.ToString());
            //while (bytesRead + 4 <= 10000)
            ////while (bytesRead + 4 <= startPos + 1204 + 200)
            //{
            //    //if (bytesRead > 1322)
            //    //{
            //    //    int junk = 0;
            //    //    junk++;
            //    //}
            //    tmpy = Convert.ToInt32( (fileIn.ReadInt16() / 2.0) + 600);
            //    tmpx = Convert.ToInt32( (fileIn.ReadInt16() / 2.0) + 600);
            //    bytesRead += 4;
            //    tmpPoints.Add(new Point(tmpx, tmpy));
            //}
            //_form2.points = new Point[tmpPoints.Count];
            //tmpPoints.CopyTo(_form2.points);
            //fileIn.Close();
            //MessageBox.Show(tmpPoints.Count.ToString());
            _form2.Show();
            _form2.drawColor = Color.FromArgb((rnd.Next(0, 255)), (rnd.Next(0, 255)), (rnd.Next(0, 255)));
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
            //List<Point> tmpPoints = new List<Point>();
            //while (bytesRead + 4 <= 10000)
            //while (bytesRead + 4 <= startPos + 1204 + 200)
            //{
            //if (bytesRead > 1322)
            //{
            //    int junk = 0;
            //    junk++;
            //}
            Int32 realx;
            Int32 realy;
            if (fileIn.BaseStream.Position + 4 < fileIn.BaseStream.Length)
            {
                realx = fileIn.ReadInt16();
                realy = fileIn.ReadInt16();
                if (realx == -32765)
                {

                    if (realy == 1)
                    {
                        //timer1.Enabled = false;
                        //colorDialog1.ShowDialog();
                        //_form2.drawColor = colorDialog1.Color;
                        _form2.drawColor = Color.FromArgb((rnd.Next(0, 255)), (rnd.Next(0, 255)), (rnd.Next(0, 255)));
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
                    tmpx = Convert.ToInt32((realx / 2.0) + 100);
                    tmpy = Convert.ToInt32((realy / 2.0) + 600);
                    bytesRead += 4;
                    //tmpPoints.Add(new Point(tmpx, tmpy));
                    if (skipStitches > 0)
                    {
                        skipStitches--;
                    }
                    else
                    {
                        _form2.addPoint(new Point(tmpx, tmpy));
                    }
                    stitchCount++;
                    stitchesLeft--;
                    _form2.Invalidate();
                }
                label1.Text = "file pos: " + fileIn.BaseStream.Position.ToString() + ", last values: " + realx.ToString() + ", " + realy.ToString() + ", stiches: " + stitchCount.ToString() + ", stitches left: " + stitchesLeft.ToString();
                if (stitchesLeft < 0)
                {
                    timer1.Enabled = false;
                    fileIn.Close();
                }
            }
            //_form2.points = new Point[tmpPoints.Count];
            //tmpPoints.CopyTo(_form2.points);

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            nextStitch();
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