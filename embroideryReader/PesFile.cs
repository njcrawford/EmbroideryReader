using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace embroideryReader
{
    public class stitchBlock
    {
        public System.Drawing.Color color;
        public Int32 unknownNumber;
        public Int32 stitchesTotal;
        public Point[] stitches;
        public stitchBlock()
        {
            color = System.Drawing.Color.Black;
            //stitches = new List<System.Drawing.Point>();
        }
    }

    public struct intPair
    {
        public int a;
        public int b;
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
        public List<Int16> csewsegHeader = new List<short>();
        public List<stitchBlock> blocks = new List<stitchBlock>();
        public List<intPair> afterStictchesTable = new List<intPair>();

        Int64 startStitches = 0;

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
                pesHeader.Add(fileIn.ReadInt16());
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
                embOneHeader.Add(tmpval);
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
                csewsegHeader.Add(fileIn.ReadInt16());

            }
            startStitches = fileIn.BaseStream.Position;
            //MessageBox.Show(message);

            //start of point pairs
            //message = "";
            //long startPos = fileIn.BaseStream.Position;
            //bytesRead = fileIn.BaseStream.Position;
            List<Point> currentBlock = new List<Point>();
            Color currentColor = Color.Black;
            Int32 tmpStitchCount = 0;
            stitchesLeft = 10; //give it kickstart to get over the beginning
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
                            tmp.unknownNumber = lastStrangeNum;
                            tmp.stitchesTotal = tmpStitchCount;
                            blocks.Add(tmp);
                            tmpStitchCount = 0;
                            currentBlock = new List<Point>();
                            currentColor = System.Drawing.Color.FromArgb((rnd.Next(0, 255)), (rnd.Next(0, 255)), (rnd.Next(0, 255)));
                        }
                        lastStrangeNum = fileIn.ReadInt16();//don't know what this is, maybe stitching speed? Seems to be 1/100 sec values
                        //timer1.Interval = lastStrangeNum * 10;
                        stitchesLeft = fileIn.ReadInt16();
                        tmpStitchCount += stitchesLeft;
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
            intPair tmpPair = new intPair();
            tmpPair.a = fileIn.ReadInt16();
            tmpPair.b = fileIn.ReadInt16();
            while (tmpPair.a != 0)
            {
                afterStictchesTable.Add(tmpPair);
                tmpPair = new intPair();
                tmpPair.a = fileIn.ReadInt16();//strange number
                tmpPair.b = fileIn.ReadInt16();//file block that strange number end at. Not the same as stichBlock.
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

        public void saveDebugInfo()
        {
            System.IO.StreamWriter outfile = new System.IO.StreamWriter(System.IO.Path.ChangeExtension(_filename, ".txt"));
            string name = "";
            outfile.WriteLine("pes header");
            for (int i = 0; i < pesHeader.Count; i++)
            {
                name = (i + 1).ToString();
                outfile.WriteLine(name + "\t" + pesHeader[i].ToString());
            }
            outfile.WriteLine("embone header");
            for (int i = 0; i < embOneHeader.Count; i++)
            {
                switch (i + 1)
                {
                    case 24:
                        name = "width";
                        break;
                    case 25:
                        name = "height";
                        break;
                    default:
                        name = (i + 1).ToString();
                        break;
                }

                outfile.WriteLine(name + "\t" + embOneHeader[i].ToString());
            }
            outfile.WriteLine("csewseg header");
            for (int i = 0; i < csewsegHeader.Count; i++)
            {

                switch (i + 1)
                {
                    case 4:
                        name = "base x";
                        outfile.WriteLine(name + "\t" + csewsegHeader[i].ToString());
                        break;
                    case 5:
                        name = "base y";
                        outfile.WriteLine(name + "\t" + csewsegHeader[i].ToString() + " (" + (csewsegHeader[i] + imageHeight).ToString() + ")");
                        break;
                    case 6:
                        name = "start x";
                        outfile.WriteLine(name + "\t" + csewsegHeader[i].ToString());
                        break;
                    case 7:
                        name = "start y";
                        outfile.WriteLine(name + "\t" + csewsegHeader[i].ToString() + " (" + (csewsegHeader[i] + imageHeight).ToString() + ")");
                        break;
                    default:
                        name = (i + 1).ToString();
                        outfile.WriteLine(name + "\t" + csewsegHeader[i].ToString());
                        break;
                }
                //outfile.WriteLine(name + "\t" + csewsegHeader[i].ToString());
            }
            outfile.WriteLine("stitches start: " + startStitches.ToString());
            outfile.WriteLine("block info");
            for (int i = 0; i < this.blocks.Count; i++)
            {
                outfile.WriteLine((i + 1).ToString() + "\t" + blocks[i].unknownNumber.ToString() + "\t" + blocks[i].stitchesTotal.ToString());
            }
            outfile.WriteLine("after stitches table");
            for (int i = 0; i < afterStictchesTable.Count; i++)
            {
                outfile.WriteLine((i + 1).ToString() + "\t" + afterStictchesTable[i].a.ToString() + ", " + afterStictchesTable[i].b.ToString());
            }
            outfile.Close();
        }
    }
}
