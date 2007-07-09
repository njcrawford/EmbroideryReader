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

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.IO.BinaryReader fileIn;
            //char charIn;
            string message = "";
            fileIn = new System.IO.BinaryReader(System.IO.File.Open("118866.pes", System.IO.FileMode.Open));
            //charIn = fileIn.ReadChar();
            for (int i = 0; i < 8; i++)
            {
                message += fileIn.ReadChar();
                bytesRead++;
            }
            message += Environment.NewLine;
            for (int i = 0; i < 8; i++)
            {
                message += fileIn.ReadInt16().ToString();
                message += Environment.NewLine;
                bytesRead += 2;
            }
            for (int i = 0; i < 7; i++)
            {
                message += fileIn.ReadChar();
                bytesRead++;
            }
            //MessageBox.Show(message);

            message = "";
            for (int i = 0; i < 33; i++) //read 66 bytes
            {
                message += fileIn.ReadInt16().ToString();
                //message += "\t| ";
                //message += fileIn.ReadInt32().ToString();
                //message += "\t| ";
                //message += fileIn.ReadByte().ToString();
                message += Environment.NewLine;
                bytesRead += 2;
            }
            //MessageBox.Show(message);

            message = "";
            for (int i = 0; i < 7; i++)
            {
                message += fileIn.ReadChar();
                bytesRead++;
            }
            //MessageBox.Show(message);

            message = "";
            for (int i = 0; i < 10; i++)
            {
                message += fileIn.ReadInt16();
                message += Environment.NewLine;
                bytesRead += 2;
            }
            MessageBox.Show(message);
            
            //int tmpRed;
            //int tmpGreen;
            //int tmpBlue;

            //tmpRed = fileIn.ReadByte();
            //tmpGreen = fileIn.ReadByte();
            //tmpBlue = fileIn.ReadByte();
            //fileIn.ReadByte();

            //Color tmpColor = System.Drawing.Color.FromArgb(tmpRed, tmpGreen, tmpBlue);
            //_form2.drawColor = tmpColor;

            //start of point pairs?
            message = "";
            int tmpx;
            int tmpy;
            List<Point> tmpPoints = new List<Point>();
            long startPos = fileIn.BaseStream.Position;
            bytesRead = fileIn.BaseStream.Position;
            while (bytesRead + 4 <= fileIn.BaseStream.Length)
            //while (bytesRead + 4 <= startPos + 1204 + 200)
            {
                //if (bytesRead > 1322)
                //{
                //    int junk = 0;
                //    junk++;
                //}
                tmpy = Convert.ToInt32( (fileIn.ReadInt16() / 2.0) + 600);
                tmpx = Convert.ToInt32( (fileIn.ReadInt16() / 2.0) + 600);
                bytesRead += 4;
                tmpPoints.Add(new Point(tmpx, tmpy));
            }
            _form2.points = new Point[tmpPoints.Count];
            tmpPoints.CopyTo(_form2.points);
            fileIn.Close();
            //MessageBox.Show(tmpPoints.Count.ToString());
            _form2.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _form2.Show();
        }

    }
}