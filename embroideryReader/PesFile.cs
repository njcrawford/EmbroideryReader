using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace embroideryReader
{
    public class stitchBlock
    {
        public System.Drawing.Color color;
        public Int32 colorIndex;
        public Int32 stitchesTotal;
        public Point[] stitches;
        public stitchBlock()
        {
            color = System.Drawing.Color.Black;
        }
    }

    public struct intPair
    {
        public int a;
        public int b;
    }

    public class PesFile
    {
        System.IO.BinaryReader fileIn;
        Random rnd = new Random();
        int stitchCount = 0;
        int stitchesLeft = 0;
        int skipStitches = 0;
        int imageWidth;
        int imageHeight;
        int lastColorIndex = -1;
        string _filename;
        public List<Int16> pesHeader = new List<short>();
        public List<Int16> embOneHeader = new List<short>();
        public List<Int16> csewsegHeader = new List<short>();
        public List<stitchBlock> blocks = new List<stitchBlock>();
        public List<intPair> afterStictchesTable = new List<intPair>();

        Int64 startStitches = 0;

        bool _readyToUse = false;

        public PesFile(string filename)
        {
            OpenFile(filename);
        }

        private void OpenFile(string filename)
        {
            _filename = filename;
            fileIn = new System.IO.BinaryReader(System.IO.File.Open(filename, System.IO.FileMode.Open));
            string startFileSig = "";
            for (int i = 0; i < 8; i++)//8 bytes
            {
                //message += fileIn.ReadChar();
                startFileSig += fileIn.ReadChar();
            }
            if (startFileSig != "#PES0001")//this is not a file that we can read
            {
                return;
            }
            //message += Environment.NewLine;
            for (int i = 0; i < 8; i++)//16 bytes
            {
                pesHeader.Add(fileIn.ReadInt16());
                //message += fileIn.ReadInt16().ToString();
                //message += Environment.NewLine;
            }
            string embOneHeaderString = "";
            for (int i = 0; i < 7; i++)//7 bytes
            {
                //message += fileIn.ReadChar();
                embOneHeaderString+= fileIn.ReadChar();
            }
            if(embOneHeaderString != "CEmbOne")//probably a corrupted file
            {
                return;
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

            }
            //MessageBox.Show(message);

            //message = "";
            string sewSegHeader = "";
            for (int i = 0; i < 7; i++)//7 bytes
            {
                //message += fileIn.ReadChar();
                sewSegHeader+= fileIn.ReadChar();
            }
            if (sewSegHeader != "CSewSeg")//probably corrupt
            {
                return;
            }
            //MessageBox.Show(message);

            //message = "";
            //MessageBox.Show(fileIn.BaseStream.Position.ToString());
            for (int i = 0; i < 7; i++)//14 bytes
            {
                //message += fileIn.ReadInt16();
                //message += Environment.NewLine;
                Int16 temp = fileIn.ReadInt16();
                if (i == 1)//second value is starting color
                {
                    lastColorIndex = temp;

                    Console.WriteLine("starting color" + temp.ToString());
                }
                csewsegHeader.Add(temp);

            }
            startStitches = fileIn.BaseStream.Position;

            //start of point pairs
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

                        if (realy == 1) //end of block
                        {
                            stitchBlock tmp = new stitchBlock();
                            tmp.stitches = new Point[currentBlock.Count];
                            currentBlock.CopyTo(tmp.stitches);
                            //if (blocks.Count > 0)// && blocks[blocks.Count - 1].colorIndex != lastColorIndex)//don't need to change the color if next block is the same
                            //{
                                tmp.color = getColorFromIndex(lastColorIndex);
                                //currentColor = System.Drawing.Color.FromArgb((rnd.Next(0, 255)), (rnd.Next(0, 255)), (rnd.Next(0, 255)));

                            //}
                            tmp.colorIndex = lastColorIndex;
                            tmp.stitchesTotal = tmpStitchCount;
                            blocks.Add(tmp);
                            tmpStitchCount = 0;
                            currentBlock = new List<Point>();
                        }
                        lastColorIndex = fileIn.ReadInt16();//get color for this block
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
                        if (skipStitches > 0)
                        {
                            skipStitches--;
                        }
                        else
                        {
                            currentBlock.Add(new System.Drawing.Point(tmpx, tmpy));
                        }
                        stitchCount++;
                        stitchesLeft--;
                    }
                }
            }

            if (currentBlock.Count > 0)
            {
                stitchBlock tmp = new stitchBlock();
                tmp.stitches = new Point[currentBlock.Count];
                currentBlock.CopyTo(tmp.stitches);
                tmp.color = getColorFromIndex(lastColorIndex);
                tmp.colorIndex = lastColorIndex;
                blocks.Add(tmp);
                currentBlock = new List<Point>();
            }

            //color index table
            intPair tmpPair = new intPair();
            tmpPair.a = fileIn.ReadInt16();
            tmpPair.b = fileIn.ReadInt16();
            while (tmpPair.a != 0)
            {
                afterStictchesTable.Add(tmpPair);
                tmpPair = new intPair();
                tmpPair.a = fileIn.ReadInt16();
                tmpPair.b = fileIn.ReadInt16();
            }
            fileIn.Close();
            _readyToUse = true;
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
                    case 2:
                        name = "start color";
                        outfile.WriteLine(name + "\t" + csewsegHeader[i].ToString());
                        break;
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
                outfile.WriteLine((i + 1).ToString() + "\t" + blocks[i].colorIndex.ToString() + "\t" + blocks[i].stitchesTotal.ToString());
            }
            outfile.WriteLine("after stitches table");
            for (int i = 0; i < afterStictchesTable.Count; i++)
            {
                outfile.WriteLine((i + 1).ToString() + "\t" + afterStictchesTable[i].a.ToString() + ", " + afterStictchesTable[i].b.ToString());
            }
            outfile.Close();
        }

        public bool isReadyToUse()
        {
            return _readyToUse;
        }

        private Color getColorFromIndex(int index)
        {
            Color retval = Color.White;
            Console.WriteLine("color index: " + index.ToString());
            switch (index)
            {
                case 20:
                    retval = Color.FromArgb(0, 0, 0);
                    break;
                case 45:
                    retval = Color.FromArgb(178, 175, 212);
                    break;
                case 46:
                    retval = Color.FromArgb(104, 106, 176);
                    break;
                case 50:
                    retval = Color.FromArgb(19, 43, 26);
                    break;
                case 56:
                    retval = Color.FromArgb(47, 126, 32);
                    break;
                case 60:
                    retval = Color.FromArgb(240, 249, 112);
                    break;
            }
            return retval;
        }
    }
}
