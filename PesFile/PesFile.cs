/*
Embroidery Reader - an application to view .pes embroidery designs

Copyright (C) 2011  Nathan Crawford
 
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
You can contact me at http://www.njcrawford.com/contact/.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace PesFile
{
    public enum statusEnum { NotOpen, IOError, ParseError, Ready };
    public class stitchBlock
    {
        public Color color;
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
    
    public class PesColors
    {
    	public static int[][] colorMap = new int[65][];
    	
    	colorValues[1] = new int[] {14, 31, 124};
    	colorValues[2] = new int[] {10, 85, 163};
    	colorValues[3] = new int[] {48, 135, 119};
    	colorValues[4] = new int[] {75, 107, 175};
    	colorValues[5] = new int[] {237, 23, 31};
    	colorValues[6] = new int[] {209, 92, 0};
    	colorValues[7] = new int[] {145, 54, 151};
    	colorValues[8] = new int[] {228, 154, 203};
    	colorValues[9] = new int[] {145, 95, 172};
    	colorValues[10] = new int[] {157, 214, 125};
    	colorValues[11] = new int[] {232, 169, 0};
    	colorValues[12] = new int[] {254, 186, 53};
    	colorValues[13] = new int[] {255, 255, 0};
    	colorValues[14] = new int[] {112, 188, 31};
    	colorValues[15] = new int[] {186, 152, 0};
    	colorValues[16] = new int[] {168, 168, 168};
    	colorValues[17] = new int[] {123, 111, 0};
    	colorValues[18] = new int[] {255, 255, 179};
    	colorValues[19] = new int[] {79, 85, 86};
    	colorValues[20] = new int[] {0, 0, 0};
    	colorValues[21] = new int[] {11, 61, 145};
    	colorValues[22] = new int[] {119, 1, 118};
    	colorValues[23] = new int[] {41, 49, 51};
    	colorValues[24] = new int[] {42, 19, 1};
    	colorValues[25] = new int[] {246, 74, 138};
    	colorValues[26] = new int[] {178, 118, 36};
    	colorValues[27] = new int[] {252, 187, 196};
    	colorValues[28] = new int[] {254, 55, 15};
    	colorValues[29] = new int[] {240, 240, 240};
    	colorValues[30] = new int[] {106, 28, 138};
    	colorValues[31] = new int[] {168, 221, 196};
    	colorValues[32] = new int[] {37, 132, 187};
    	colorValues[33] = new int[] {254, 179, 67};
    	colorValues[34] = new int[] {255, 240, 141};
    	colorValues[35] = new int[] {208, 166, 96};
    	colorValues[36] = new int[] {209, 84, 0};
    	colorValues[37] = new int[] {102, 186, 73};
    	colorValues[38] = new int[] {19, 74, 70};
    	colorValues[39] = new int[] {135, 135, 135};
    	colorValues[40] = new int[] {216, 202, 198};
    	colorValues[41] = new int[] {67, 86, 7};
    	colorValues[42] = new int[] {254, 227, 197};
    	colorValues[43] = new int[] {249, 147, 188};
    	colorValues[44] = new int[] {0, 56, 34};
    	colorValues[45] = new int[] {178, 175, 212};
    	colorValues[46] = new int[] {104, 106, 176};
    	colorValues[47] = new int[] {239, 227, 185};
    	colorValues[48] = new int[] {247, 56, 102};
    	colorValues[49] = new int[] {181, 76, 100};
    	colorValues[50] = new int[] {19, 43, 26};
    	colorValues[51] = new int[] {199, 1, 85};
    	colorValues[52] = new int[] {254, 158, 50};
    	colorValues[53] = new int[] {168, 222, 235};
    	colorValues[54] = new int[] {0, 103, 26};
    	colorValues[55] = new int[] {78, 41, 144};
    	colorValues[56] = new int[] {47, 126, 32};
    	colorValues[57] = new int[] {253, 217, 222};
    	colorValues[58] = new int[] {255, 217, 17};
    	colorValues[59] = new int[] {9, 91, 166};
    	colorValues[60] = new int[] {240, 249, 112};
    	colorValues[61] = new int[] {227, 243, 91};
    	colorValues[62] = new int[] {255, 200, 100};
    	colorValues[63] = new int[] {255, 200, 150};
    	colorValues[64] = new int[] {255, 200, 200};
    }

    public class PesFile
    {
        System.IO.BinaryReader fileIn;
        int imageWidth;
        int imageHeight;
        string _filename;
        public List<Int64> pesHeader = new List<Int64>();
        public List<Int16> embOneHeader = new List<short>();
        public List<Int16> sewSegHeader = new List<short>();
        public List<Int16> embPunchHeader = new List<short>();
        public List<Int16> sewFigSegHeader = new List<short>();
        public List<stitchBlock> blocks = new List<stitchBlock>();
        public List<intPair> colorTable = new List<intPair>();
        private statusEnum readyStatus = statusEnum.NotOpen;
        Int64 startStitches = 0;
        string lastError = "";
        Point translateStart;
        UInt16 pesVersion;


        // If set to true, this variable means we couldn't figure out some or
        // all of the colors and white will be used instead of those colors.
        private bool colorWarning = false;

        private bool formatWarning = false;

        private bool classWarning = false;

        public PesFile(string filename)
        {
            OpenFile(filename);
        }

        private void OpenFile(string filename)
        {
#if !DEBUG
            try
            {
#endif
                _filename = filename;
                fileIn = new System.IO.BinaryReader(System.IO.File.Open(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read));

                string startFileSig = "";
                for (int i = 0; i < 4; i++) // 4 bytes
                {
                    startFileSig += fileIn.ReadChar();
                }
                if (startFileSig != "#PES")
                {
                	// This is not a file that we can read
                    readyStatus = statusEnum.ParseError;
                    lastError = "Missing #PES at beginning of file";
                    fileIn.Close();
                    return;
                }
                
                // PES version
                string versionString = "";
                for (int i = 0; i < 4; i++) // 4 bytes
                {
                    versionString += fileIn.ReadChar();
                }
                pesVersion = Convert.ToUInt16(versionString);
                
                int pecstart = fileIn.ReadInt32();

                fileIn.BaseStream.Position = pecstart + 48;
                int numColors = fileIn.ReadByte() +1;
                List<byte> colorList = new List<byte>();
                for (int x = 0; x < numColors; x++)
                {
                    colorList.Add(fileIn.ReadByte());
                }

                fileIn.BaseStream.Position = pecstart + 532;
                bool thisPartIsDone = false;
                stitchBlock curBlock;
                int prevX = 0;
                int prevY = 0;
                int maxX = 0;
                int minX = 0;
                int maxY = 0;
                int minY = 0;
                int colorNum = -1;
                int colorIndex = 0;
                List<Point> tempStitches = new List<Point>();
                while (!thisPartIsDone)
                {
                    byte val1;
                    byte val2;
                    val1 = fileIn.ReadByte();
                    val2 = fileIn.ReadByte();
                    if (val1 == 255 && val2 == 0)
                    {
                        //end of stitches
                        thisPartIsDone = true;

                        //add the last block
                        curBlock = new stitchBlock();
                        curBlock.stitches = new Point[tempStitches.Count];
                        tempStitches.CopyTo(curBlock.stitches);
                        curBlock.stitchesTotal = tempStitches.Count;
                        colorNum++;
                        colorIndex = colorList[colorNum];
                        curBlock.colorIndex = colorIndex;
                        curBlock.color = getColorFromIndex(colorIndex);
                        blocks.Add(curBlock);
                    }
                    else if (val1 == 254 && val2 == 176)
                    {
                        //color switch, start a new block

                        curBlock = new stitchBlock();
                        curBlock.stitches = new Point[tempStitches.Count];
                        tempStitches.CopyTo(curBlock.stitches);
                        curBlock.stitchesTotal = tempStitches.Count;
                        colorNum++;
                        colorIndex = colorList[colorNum];
                        curBlock.colorIndex = colorIndex;
                        curBlock.color = getColorFromIndex(colorIndex);
                        blocks.Add(curBlock);

                        tempStitches = new List<Point>();

                        //read useless(?) byte
                        fileIn.ReadByte();
                    }
                    else
                    {
                        int deltaX = 0;
                        int deltaY = 0;
                        if ((val1 & 0x80) == 0x80)
                        {
                            //this is a jump stitch
                            deltaX = ((val1 & 0x0f) << 8) + val2;
                            /* dead code? bit we're checking is masked out in previous line
                            if ((deltaX & 0x0800) == 0x0800)
                            {
                                deltaX = deltaX - 4096;
                            }
                            */
                            //read next byte for Y value
                            val2 = fileIn.ReadByte();
                        }
                        else
                        {
                            //normal stitch
                            deltaX = val1;
                            if (deltaX > 63)
                            {
                                deltaX = deltaX - 128;
                            }
                        }

                        if ((val2 & 0x08) == 0x80)
                        {
                            //this is a jump stitch
                            int val3 = fileIn.ReadByte();
                            deltaY = ((val2 & 0x0f) << 8) + val3;
                            /* dead code? bit we're checking is masked out in previous line
                            if ((deltaY & 0x0800) == 0x0800)
                            {
                                deltaY = deltaY - 4096;
                            }*/
                        }
                        else
                        {
                            //normal stitch
                            deltaY = val2;
                            if (deltaY > 63)
                            {
                                deltaY = deltaY - 128;
                            }
                        }
                        tempStitches.Add(new Point(prevX + deltaX, prevY + deltaY));
                        prevX = prevX + deltaX;
                        prevY = prevY + deltaY;
                        if (prevX > maxX)
                        {
                            maxX = prevX;
                        }
                        else if (prevX < minX)
                        {
                            minX = prevX;
                        }

                        if (prevY > maxY)
                        {
                            maxY = prevY;
                        }
                        else if (prevY < minY)
                        {
                            minY = prevY;
                        }
                    }
                }
                imageWidth = maxX - minX;
                imageHeight = maxY - minY;
                translateStart.X = -minX;
                translateStart.Y = -minY;
                readyStatus = statusEnum.Ready;

                // Close the file
                fileIn.Close();

#if !DEBUG
            }
            catch (System.IO.IOException ioex)
            {
                readyStatus = statusEnum.IOError;
                lastError = ioex.Message;
                if (fileIn != null)
                {
                    fileIn.Close();
                }
            }
            catch (Exception ex)
            {
                readyStatus = statusEnum.ParseError;
                lastError = ex.Message;
                if (fileIn != null)
                {
                    fileIn.Close();
                }
            }
#endif
        }

        /*void readCSewFigSeg(System.IO.BinaryReader file)
        {
            startStitches = fileIn.BaseStream.Position;

            bool doneWithStitches = false;
            int xValue = -100;
            int yValue = -100;
            stitchBlock currentBlock;
            int blockType; //if this is equal to newColorMarker, it's time to change color
            int colorIndex = 0;
            int remainingStitches;
            List<Point> stitchData;
            stitchData = new List<Point>();
            currentBlock = new stitchBlock();

            while (!doneWithStitches)
            {
                //reset variables
                xValue = 0;
                yValue = 0;

                blockType = file.ReadInt16();
                if (blockType == 16716)
                    break;
                colorIndex = file.ReadInt16();
                if (colorIndex == 16716)
                    break;
                remainingStitches = file.ReadInt16();
                if (remainingStitches == 16716)
                    break;
                while (remainingStitches >= 0)
                {
                    xValue = file.ReadInt16();
                    if (xValue == -32765)
                    {
                        break;//drop out before we start eating into the next section 
                    }
                    if (remainingStitches == 0)
                    {
                        int junk2 = 0;
                        junk2 = blocks.Count;

                        file.ReadBytes(24);
                        if (file.ReadInt16() == -1)
                            doneWithStitches = true;

                        currentBlock.stitches = new Point[stitchData.Count];
                        stitchData.CopyTo(currentBlock.stitches);
                        currentBlock.colorIndex = colorIndex;
                        currentBlock.color = getColorFromIndex(colorIndex);
                        currentBlock.stitchesTotal = stitchData.Count;
                        blocks.Add(currentBlock);
                        stitchData = new List<Point>();
                        currentBlock = new stitchBlock();

                        file.ReadBytes(48);

                        break;
                    }
                    else if (xValue == 16716 || xValue == 8224)
                    {
                        doneWithStitches = true;
                        break;
                    }
                    yValue = fileIn.ReadInt16();
                    if (yValue == 16716 || yValue == 8224)
                    {
                        doneWithStitches = true;
                        break;
                    }
                    stitchData.Add(new Point(xValue - translateStart.X, yValue + imageHeight - translateStart.Y));
                    remainingStitches--;
                }
            }
            if (stitchData.Count > 1)
            {
                currentBlock.stitches = new Point[stitchData.Count];
                stitchData.CopyTo(currentBlock.stitches);
                currentBlock.colorIndex = colorIndex;
                currentBlock.color = getColorFromIndex(colorIndex);
                currentBlock.stitchesTotal = stitchData.Count;
                blocks.Add(currentBlock);
            }
        }*/

        List<stitchBlock> filterStitches(List<stitchBlock> input, int threshold)
        {
            List<stitchBlock> retval = new List<stitchBlock>();
            List<Point> tempStitchData = new List<Point>();
            for (int x = 0; x < input.Count; x++)
            {

                for (int i = 0; i < input[x].stitches.Length; i++)
                {
                    if (i > 0)//need a previous point to check against, can't check the first
                    {
                        double diffx = Math.Abs(input[x].stitches[i].X - input[x].stitches[i - 1].X);
                        double diffy = Math.Abs(input[x].stitches[i].Y - input[x].stitches[i - 1].Y);
                        if (Math.Sqrt(Math.Pow(diffx, 2.0) + Math.Pow(diffy, 2.0)) < threshold) //check distance between this point and the last one
                        {
                            if (tempStitchData.Count == 0 && i > 1)//first stitch of block gets left out without this, except for very first stitch
                            {
                                tempStitchData.Add(input[x].stitches[i - 1]);
                            }
                            tempStitchData.Add(input[x].stitches[i]);
                        }
                        else//stitch is too far from the previous one
                        {
                            if (tempStitchData.Count > 2)//add the block and start a new one
                            {
                                stitchBlock tempBlock = new stitchBlock();
                                tempBlock.color = input[x].color;
                                tempBlock.colorIndex = input[x].colorIndex;
                                tempBlock.stitches = new Point[tempStitchData.Count];
                                tempStitchData.CopyTo(tempBlock.stitches);
                                retval.Add(tempBlock);
                                tempStitchData = new List<Point>();
                            }
                            else//reset variables
                            {
                                tempStitchData = new List<Point>();
                            }
                        }
                    }
                    else //just add the first one, don't have anything to compare against
                    {
                        tempStitchData.Add(input[x].stitches[i]);
                    }
                }
                if (tempStitchData.Count > 2)
                {
                    stitchBlock tempBlock = new stitchBlock();
                    tempBlock.color = input[x].color;
                    tempBlock.colorIndex = input[x].colorIndex;
                    tempBlock.stitches = new Point[tempStitchData.Count];
                    tempStitchData.CopyTo(tempBlock.stitches);
                    retval.Add(tempBlock);
                    tempStitchData = new List<Point>();
                }
            }
            return retval;
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
            outfile.Write(getDebugInfo());
            outfile.Close();
        }

        public string getDebugInfo()
        {
            System.IO.StringWriter outfile = new System.IO.StringWriter();
            string name = "";
            outfile.WriteLine("PES header");
            outfile.WriteLine("PES version:\t" + pesVersion);
            for (int i = 0; i < pesHeader.Count; i++)
            {
                name = (i + 1).ToString();
                outfile.WriteLine(name + "\t" + pesHeader[i].ToString());
            }
            if (embOneHeader.Count > 0)
            {
                outfile.WriteLine("CEmbOne header");
                for (int i = 0; i < embOneHeader.Count; i++)
                {
                    switch (i + 1)
                    {
                        case 22:
                            name = "translate x";
                            break;
                        case 23:
                            name = "translate y";
                            break;
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
            }
            if (embPunchHeader.Count > 0)
            {
                outfile.WriteLine("CEmbPunch header");
                for (int i = 0; i < embPunchHeader.Count; i++)
                {
                    switch (i + 1)
                    {
                        default:
                            name = (i + 1).ToString();
                            break;
                    }

                    outfile.WriteLine(name + "\t" + embPunchHeader[i].ToString());
                }
            }

            outfile.WriteLine("stitches start: " + startStitches.ToString());
            outfile.WriteLine("block info");
            outfile.WriteLine("number\tcolor\tstitches");
            for (int i = 0; i < this.blocks.Count; i++)
            {
                outfile.WriteLine((i + 1).ToString() + "\t" + blocks[i].colorIndex.ToString() + "\t" + blocks[i].stitchesTotal.ToString());
            }
            outfile.WriteLine("color table");
            outfile.WriteLine("number\ta\tb");
            for (int i = 0; i < colorTable.Count; i++)
            {
                outfile.WriteLine((i + 1).ToString() + "\t" + colorTable[i].a.ToString() + ", " + colorTable[i].b.ToString());
            }
            if (blocks.Count > 0)
            {
                outfile.WriteLine("Extended stitch debug info");
                for (int blocky = 0; blocky < blocks.Count; blocky++)
                {
                    outfile.WriteLine("block " + (blocky + 1).ToString() + " start");
                    for (int stitchy = 0; stitchy < blocks[blocky].stitches.Length; stitchy++)
                    {
                        outfile.WriteLine(blocks[blocky].stitches[stitchy].X.ToString() + ", " + blocks[blocky].stitches[stitchy].Y.ToString());
                    }
                }
            }
            outfile.Close();
            return outfile.ToString();
        }

        public statusEnum getStatus()
        {
            return readyStatus;
        }

        public string getLastError()
        {
            return lastError;
        }

        public bool getColorWarning()
        {
            return colorWarning;
        }

        public bool getFormatWarning()
        {
            return formatWarning;
        }

        public bool getClassWarning()
        {
            return classWarning;
        }

        private Color getColorFromIndex(int index)
        {
            //Color retval;// = Color.White;
            if(index >= 1 && index <= 64)
            {
            	return Color.FromArgb(
            		PesColors.colorMap[index][0],
            		PesColors.colorMap[index][1],
            		PesColors.colorMap[index][2]
            	);
            }
            else
            {
            	return Color.White;
                colorWarning = true;
            }
        }

        public Bitmap designToBitmap(Single threadThickness, bool filterUglyStitches, int filterUglyStitchesThreshold)
        {
            Bitmap DrawArea;
            Graphics xGraph;

            DrawArea = new Bitmap(GetWidth() + (int)(threadThickness * 2), GetHeight() + (int)(threadThickness * 2));
            //panel1.Width = design.GetWidth() + (int)(threadThickness * 2);
            //panel1.Height = design.GetHeight() + (int)(threadThickness * 2);
            xGraph = Graphics.FromImage(DrawArea);
            xGraph.TranslateTransform(threadThickness+translateStart.X, threadThickness+translateStart.Y);
            //xGraph.FillRectangle(Brushes.White, 0, 0, DrawArea.Width, DrawArea.Height);
            List<stitchBlock> tmpblocks;
#if DEBUG
            tmpblocks = blocks;
#else
            if (filterUglyStitches && !formatWarning) //only filter stitches if we think we understand the format
            {
                tmpblocks = filterStitches(blocks, filterUglyStitchesThreshold);
            }
            else
            {
                tmpblocks = blocks;
            }
#endif
            for (int i = 0; i < tmpblocks.Count; i++)
            {
                if (tmpblocks[i].stitches.Length > 1)//must have 2 points to make a line
                {
                    Pen tempPen = new Pen(tmpblocks[i].color, threadThickness);
                    tempPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    tempPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    tempPen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                    xGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    xGraph.DrawLines(tempPen, tmpblocks[i].stitches);
                }
            }
            xGraph.Dispose();
            return DrawArea;
        }
    }
}
