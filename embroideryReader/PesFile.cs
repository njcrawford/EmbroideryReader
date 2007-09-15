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
                embOneHeaderString += fileIn.ReadChar();
            }
            if (embOneHeaderString != "CEmbOne")//probably a corrupted file
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
                sewSegHeader += fileIn.ReadChar();
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
                case 1:
                    retval = Color.FromArgb(14, 31, 124);
                    break;
                case 2:
                    retval = Color.FromArgb(10, 85, 163);
                    break;
                case 3:
                    retval = Color.FromArgb(48, 135, 119);
                    break;
                case 4:
                    retval = Color.FromArgb(75, 107, 175);
                    break;
                case 5:
                    retval = Color.FromArgb(237, 23, 31);
                    break;
                case 6:
                    retval = Color.FromArgb(209, 92, 0);
                    break;
                case 7:
                    retval = Color.FromArgb(145, 54, 151);
                    break;
                case 8:
                    retval = Color.FromArgb(228, 154, 203);
                    break;
                case 9:
                    retval = Color.FromArgb(145, 95, 172);
                    break;
                case 10:
                    retval = Color.FromArgb(157, 214, 125);
                    break;
                case 11:
                    retval = Color.FromArgb(232, 169, 0);
                    break;
                case 12:
                    retval = Color.FromArgb(254, 186, 53);
                    break;
                case 13:
                    retval = Color.FromArgb(255, 255, 0);
                    break;
                case 14:
                    retval = Color.FromArgb(112, 188, 31);
                    break;
                case 15:
                    retval = Color.FromArgb(145, 95, 172);
                    break;
                case 16:
                    retval = Color.FromArgb(168, 168, 168);
                    break;
                case 17:
                    retval = Color.FromArgb(123, 111, 0);
                    break;
                case 18:
                    retval = Color.FromArgb(255, 255, 179);
                    break;
                case 19:
                    retval = Color.FromArgb(79, 85, 86);
                    break;
                case 20:
                    retval = Color.FromArgb(0, 0, 0);
                    break;
                case 21:
                    retval = Color.FromArgb(11, 61, 145);
                    break;
                case 22:
                    retval = Color.FromArgb(119, 1, 118);
                    break;
                case 23:
                    retval = Color.FromArgb(41, 49, 51);
                    break;
                case 24:
                    retval = Color.FromArgb(42, 19, 1);
                    break;
                case 25:
                    retval = Color.FromArgb(246, 74, 138);
                    break;
                case 26:
                    retval = Color.FromArgb(178, 118, 36);
                    break;
                case 27:
                    retval = Color.FromArgb(252, 187, 196);
                    break;
                case 28:
                    retval = Color.FromArgb(254, 55, 15);
                    break;
                case 29:
                    retval = Color.FromArgb(240, 240, 240);
                    break;
                case 30:
                    retval = Color.FromArgb(106, 28, 138);
                    break;
                case 31:
                    retval = Color.FromArgb(168, 221, 196);
                    break;
                case 32:
                    retval = Color.FromArgb(37, 132, 187);
                    break;
                case 33:
                    retval = Color.FromArgb(254, 179, 67);
                    break;
                case 34:
                    retval = Color.FromArgb(255, 240, 141);
                    break;
                case 35:
                    retval = Color.FromArgb(208, 166, 96);
                    break;
                case 36:
                    retval = Color.FromArgb(209, 84, 0);
                    break;
                case 37:
                    retval = Color.FromArgb(102, 186, 73);
                    break;
                case 38:
                    retval = Color.FromArgb(19, 74, 70);
                    break;
                case 39:
                    retval = Color.FromArgb(135, 135, 135);
                    break;
                case 40:
                    retval = Color.FromArgb(216, 202, 198);
                    break;
                case 41:
                    retval = Color.FromArgb(67, 86, 7);
                    break;
                case 42:
                    retval = Color.FromArgb(254, 227, 197);
                    break;
                case 43:
                    retval = Color.FromArgb(249, 147, 188);
                    break;
                case 44:
                    retval = Color.FromArgb(0, 56, 34);
                    break;
                case 45:
                    retval = Color.FromArgb(178, 175, 212);
                    break;
                case 46:
                    retval = Color.FromArgb(104, 106, 176);
                    break;
                case 47:
                    retval = Color.FromArgb(239, 227, 185);
                    break;
                case 48:
                    retval = Color.FromArgb(247, 56, 102);
                    break;
                case 49:
                    retval = Color.FromArgb(181, 76, 100);
                    break;
                case 50:
                    retval = Color.FromArgb(19, 43, 26);
                    break;
                case 51:
                    retval = Color.FromArgb(199, 1, 85);
                    break;
                case 52:
                    retval = Color.FromArgb(254, 158, 50);
                    break;
                case 53:
                    retval = Color.FromArgb(168, 222, 235);
                    break;
                case 54:
                    retval = Color.FromArgb(0, 103, 26);
                    break;
                case 55:
                    retval = Color.FromArgb(78, 41, 144);
                    break;
                case 56:
                    retval = Color.FromArgb(47, 126, 32);
                    break;
                case 57:
                    retval = Color.FromArgb(253, 217, 222);
                    break;
                case 58:
                    retval = Color.FromArgb(255, 217, 17);
                    break;
                case 59:
                    retval = Color.FromArgb(9, 91, 166);
                    break;
                case 60:
                    retval = Color.FromArgb(240, 249, 112);
                    break;
                case 61:
                    retval = Color.FromArgb(227, 243, 91);
                    break;
                case 62:
                    retval = Color.FromArgb(255, 200, 100);
                    break;
                case 63:
                    retval = Color.FromArgb(255, 200, 150);
                    break;
                case 64:
                    retval = Color.FromArgb(255, 200, 200);
                    break;
            }
            return retval;
        }
    }
}
