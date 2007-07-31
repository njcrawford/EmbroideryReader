using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace embroideryReader
{
    public class stitchBlock
    {
        public System.Drawing.Color color;
        public Point[] stitches;
        public stitchBlock()
        {
            color = System.Drawing.Color.Black;
            //stitches = new List<System.Drawing.Point>();
        }
    }

    public class PesFile
    {
        //private long bytesRead = 0;
        System.IO.BinaryReader fileIn;
        Random rnd = new Random();
        int stitchCount = 0;
        int stitchesLeft = 0;
        int skipStitches = 0;
        //int minX = int.MaxValue;
        //int minY = int.MaxValue;
        //int maxX = int.MinValue;
        //int maxY = int.MinValue;
        int imageWidth;
        int imageHeight;
        int lastStrangeNum = -1;
        string _filename;
        public List<Int16> pesHeader = new List<short>();
        public List<Int16> embOneHeader = new List<short>();
        public List<stitchBlock> blocks = new List<stitchBlock>();

        public PesFile(string filename)
        {
            OpenFile(filename);
        }

        private void OpenFile(string filename)
        {
            //string message = "";
            _filename = filename;
            //string filename;
            //openFileDialog1.ShowDialog();
            //filename = openFileDialog1.FileName;
            //if (!System.IO.File.Exists(filename))
            //{
            //    return;
            //}
            //fileIn = new System.IO.BinaryReader(System.IO.File.Open("118866.pes", System.IO.FileMode.Open));
            //fileIn = new System.IO.BinaryReader(System.IO.File.Open("144496.pes", System.IO.FileMode.Open));
            fileIn = new System.IO.BinaryReader(System.IO.File.Open(filename, System.IO.FileMode.Open));
            //charIn = fileIn.ReadChar();
            for (int i = 0; i < 8; i++)//8 bytes
            {
                //message += fileIn.ReadChar();
                //bytesRead++;
                fileIn.ReadChar();
            }
            //message += Environment.NewLine;
            for (int i = 0; i < 8; i++)//16 bytes
            {
                pesHeader.Add( fileIn.ReadInt16());
                //message += fileIn.ReadInt16().ToString();
                //message += Environment.NewLine;
                //bytesRead += 2;
            }
            for (int i = 0; i < 7; i++)//7 bytes
            {
                //message += fileIn.ReadChar();
                //bytesRead++;
                fileIn.ReadChar();
            }
            //message += Environment.NewLine;
            //MessageBox.Show(message);

            //message = "";
            //int headerValNum = 1;
            Int16 tmpval;
            for (int i = 0; i < 33; i++) //read 66 bytes
            {
                tmpval = fileIn.ReadInt16();
                embOneHeader.Add( tmpval);
                switch (i)
                {
                    case 23:
                        imageWidth = tmpval;
                        break;
                    case 24:
                        imageHeight = tmpval;
                        break;
                }

                //message += tmpval.ToString();
                //if (headerValNum % 3 == 0)
                //{
                //    message += Environment.NewLine;
                //}
                //else
                //{
                //    message += "\t| ";
                //}
                //headerValNum++;
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
                //message += fileIn.ReadChar();
                //bytesRead++;
                fileIn.ReadChar();
            }
            //MessageBox.Show(message);

            //message = "";
            //MessageBox.Show(fileIn.BaseStream.Position.ToString());
            for (int i = 0; i < 7; i++)//14 bytes
            {
                //message += fileIn.ReadInt16();
                //message += Environment.NewLine;
                //bytesRead += 2;
                fileIn.ReadInt16();
            }
            //MessageBox.Show(message);

            //start of point pairs
            //message = "";
            //long startPos = fileIn.BaseStream.Position;
            //bytesRead = fileIn.BaseStream.Position;
            List<Point> currentBlock = new List<Point>();
            Color currentColor = Color.Black;
            while (stitchesLeft >= 0)
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
                            //_form2.drawPen = new Pen(Color.FromArgb((rnd.Next(0, 255)), (rnd.Next(0, 255)), (rnd.Next(0, 255))), 4);
                            //_form2.prevPoint = new Point(-1, -1);
                            stitchBlock tmp = new stitchBlock();
                            tmp.stitches = new Point[currentBlock.Count];
                            currentBlock.CopyTo(tmp.stitches);
                            tmp.color = currentColor;
                            blocks.Add(tmp);
                            currentBlock = new List<Point>();
                            currentColor = System.Drawing.Color.FromArgb((rnd.Next(0, 255)), (rnd.Next(0, 255)), (rnd.Next(0, 255)));
                        }
                        lastStrangeNum = fileIn.ReadInt16();//don't know what this is, maybe stitching speed? Seems to be 1/100 sec values
                        //timer1.Interval = lastStrangeNum * 10;
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
                        //bytesRead += 4;
                        if (skipStitches > 0)
                        {
                            skipStitches--;
                        }
                        else
                        {
                            currentBlock.Add(new System.Drawing.Point(tmpx, tmpy));
                            //_form2.addPoint(new Point(tmpx, tmpy));
                            //if (realx < minX)
                            //{
                            //    minX = realx;
                            //}
                            //if (realx > maxX)
                            //{
                            //    maxX = realx;
                            //}
                            //if (realy < minY)
                            //{
                            //    minY = realy;
                            //}
                            //if (realy > maxY)
                            //{
                            //    maxY = realy;
                            //}
                        }
                        stitchCount++;
                        stitchesLeft--;
                    }
                    //label1.Text = "file pos: " + fileIn.BaseStream.Position.ToString() + ", last values: " + realx.ToString() + ", " + realy.ToString() + ", total stiches: " + stitchCount.ToString() + ", stitches til next section: " + (stitchesLeft + 1).ToString() + ", min, max vals: " + minX.ToString() + "," + minY.ToString() + ";" + maxX.ToString() + "," + maxY.ToString() + ", strangenum: " + lastStrangeNum;
                    //if (stitchesLeft < 0)
                    //{
                    //    timer1.Enabled = false;
                    //    fileIn.Close();
                    //}
                }
            }
            fileIn.Close();
        }

        public int GetWidth()
        {
            return imageWidth;
        }

        public int GetHeight()
        {
            return imageHeight;
        }

        public string GetFileName()
        {
            if (_filename == null)
            {
                return "";
            }
            else
            {
                return _filename;
            }
        }
    }
}
