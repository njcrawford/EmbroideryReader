/*
Embroidery Reader - an application to view .pes embroidery designs

Copyright (C) 2016 Nathan Crawford
 
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

    public class PesFile
    {
        System.IO.BinaryReader fileIn;
        int imageWidth;
        int imageHeight;
        string _filename;
        public List<StitchBlock> blocks = new List<StitchBlock>();
        public List<Tuple<int, int>> colorTable = new List<Tuple<int, int>>();
        private statusEnum readyStatus = statusEnum.NotOpen;
        Int64 startStitches = 0;
        string lastError = "";
        Point translateStart;
        UInt16 pesVersion;


        // If set to true, this variable means we couldn't figure out some or
        // all of the colors and white will be used instead of those colors.
        private bool colorWarning = false;

        private bool formatWarning = false;

        public PesFile(string filename)
        {
            OpenFile(filename);
        }

        // Returns an Int16 representation of the 12 bit signed int contained in
        // high and low bytes
        private Int16 get12Bit2sComplement(byte high, byte low)
        {
            Int32 retval;

            // Get the bottom 4 bits of the high byte
            retval = high & 0x0f;

            // Shift those bits up where they belong
            retval = retval << 8;

            // Add in the bottom 8 bits
            retval += low;

            // Check for a negative number (check if 12th bit is 1)
            if ((retval & 0x0800) == 0x0800)
            {
                // Make the number negative by subtracting 4096, which is the
                // number of values a 12 bit integer can represent.
                // This is a shortcut for getting the 2's complement value.
                retval -= 4096;
            }

            return (Int16)retval;
        }

        // Returns a signed byte representation of the 7 bit signed int contained
        // in b.
        private SByte get7Bit2sComplement(byte b)
        {
            SByte retval;

            // Ignore the 8th bit. (make sure it's 0)
            b &= 0x7f;

            // Check for a negative number (check if 7th bit is 1)
            if ((b & 0x40) == 0x40)
            {
                // Make the number negative by subtracting 128, which is the
                // number of values a 7 bit integer can represent.
                // This is a shortcut for getting the 2's complement value.
                retval = (SByte)(b - 128);
            }
            else
            {
                // Positive number - no modification needed
                retval = (SByte)b;
            }

            return retval;
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

                // Read number of colors in this design
                fileIn.BaseStream.Position = pecstart + 48;
                int numColors = fileIn.ReadByte() +1;
                List<byte> colorList = new List<byte>();
                for (int x = 0; x < numColors; x++)
                {
                    colorList.Add(fileIn.ReadByte());
                }

                // Read stitch data
                fileIn.BaseStream.Position = pecstart + 532;
                bool thisPartIsDone = false;
                StitchBlock curBlock;
                int prevX = 0;
                int prevY = 0;
                int maxX = 0;
                int minX = 0;
                int maxY = 0;
                int minY = 0;
                int colorNum = -1;
                int colorIndex = 0;
                List<Stitch> tempStitches = new List<Stitch>();
                while (!thisPartIsDone)
                {
                    byte val1;
                    byte val2;
                    val1 = fileIn.ReadByte();
                    val2 = fileIn.ReadByte();
                    if (val1 == 0xff && val2 == 0x00)
                    {
                        //end of stitches
                        thisPartIsDone = true;

                        //add the last block
                        curBlock = new StitchBlock();
                        curBlock.stitches = new Stitch[tempStitches.Count];
                        tempStitches.CopyTo(curBlock.stitches);
                        curBlock.stitchesTotal = tempStitches.Count;
                        colorNum++;
                        colorIndex = colorList[colorNum];
                        curBlock.colorIndex = colorIndex;
                        curBlock.color = getColorFromIndex(colorIndex);
                        blocks.Add(curBlock);
                    }
                    else if (val1 == 0xfe && val2 == 0xb0)
                    {
                        //color switch, start a new block

                        curBlock = new StitchBlock();
                        curBlock.stitches = new Stitch[tempStitches.Count];
                        tempStitches.CopyTo(curBlock.stitches);
                        curBlock.stitchesTotal = tempStitches.Count;
                        colorNum++;
                        colorIndex = colorList[colorNum];
                        curBlock.colorIndex = colorIndex;
                        curBlock.color = getColorFromIndex(colorIndex);
                        //read useless(?) byte
                        // The value of this 'useless' byte seems to alternate
                        // between 2 and 1 for every other block. The only
                        // exception I've noted is the last block which appears
                        // to always be 0.
                        curBlock.unknownStartByte = fileIn.ReadByte();
                        blocks.Add(curBlock);

                        tempStitches = new List<Stitch>();
                    }
                    else
                    {
                        int deltaX = 0;
                        int deltaY = 0;
                        if ((val1 & 0x80) == 0x80)
                        {
                            // This is a 12-bit int. Allows for needle movement
                            // of up to +2047 or -2048.
                            deltaX = get12Bit2sComplement(val1, val2);

                            // The X value used both bytes, so read next byte
                            // for Y value.
                            val1 = fileIn.ReadByte();
                        }
                        else
                        {
                            // This is a 7-bit int. Allows for needle movement
                            // of up to +63 or -64.
                            deltaX = get7Bit2sComplement(val1);

                            // The X value only used 1 byte, so copy the second
                            // to to the first for Y value.
                            val1 = val2;
                        }

                        if ((val1 & 0x80) == 0x80)
                        {
                            // This is a 12-bit int. Allows for needle movement
                            // of up to +2047 or -2048.
                            // Read in the next byte to get the full value
                            val2 = fileIn.ReadByte();
                            deltaY = get12Bit2sComplement(val1, val2);
                        }
                        else
                        {
                            // This is a 7-bit int. Allows for needle movement
                            // of up to +63 or -64.
                            deltaY = get7Bit2sComplement(val1);
                            // Finished reading data for this stitch, no more
                            // bytes needed.
                        }
                        tempStitches.Add(
                            new Stitch(
                                new Point(prevX, prevY),
                                new Point(prevX + deltaX, prevY + deltaY)
                            )
                        );
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

        // Returns the path of the file it saved debug info to
        public string saveDebugInfo()
        {
            string retval = System.IO.Path.ChangeExtension(_filename, ".txt");
            System.IO.StreamWriter outfile = new System.IO.StreamWriter(retval);
            outfile.Write(getDebugInfo());
            outfile.Close();
            return retval;
        }

        public string getDebugInfo()
        {
            System.IO.StringWriter outfile = new System.IO.StringWriter();
            outfile.WriteLine("PES header");
            outfile.WriteLine("PES version:\t" + pesVersion);

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
                outfile.WriteLine((i + 1).ToString() + "\t" + colorTable[i].Item1.ToString() + ", " + colorTable[i].Item2.ToString());
            }
            if (blocks.Count > 0)
            {
                outfile.WriteLine("Extended stitch debug info");
                for (int blocky = 0; blocky < blocks.Count; blocky++)
                {
                    outfile.WriteLine("block " + (blocky + 1).ToString() + " start");
                    outfile.WriteLine("unknown start byte: " + blocks[blocky].unknownStartByte.ToString("X2"));
                    for (int stitchy = 0; stitchy < blocks[blocky].stitches.Length; stitchy++)
                    {
                        outfile.WriteLine(blocks[blocky].stitches[stitchy].a.ToString() + " - " + blocks[blocky].stitches[stitchy].b.ToString());
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

        private Color getColorFromIndex(int index)
        {
            if(index >= 1 && index <= 64)
            {
            	return Color.FromArgb(
            		PesColors.colorMap[index,0],
            		PesColors.colorMap[index,1],
            		PesColors.colorMap[index,2]
            	);
            }
            else
            {
             colorWarning = true;
            	return Color.White;
            }
        }

        public Bitmap designToBitmap(Single threadThickness, bool filterUglyStitches, double filterUglyStitchesThreshold, float scale)
        {
            Bitmap DrawArea;
            Graphics xGraph;
            int imageWidth = (int)((GetWidth() + (threadThickness * 2)) * scale);
            int imageHeight = (int)((GetHeight() + (threadThickness * 2)) * scale);
            float tempThreadThickness = threadThickness * scale;

            DrawArea = new Bitmap(imageWidth, imageHeight);
            using (xGraph = Graphics.FromImage(DrawArea))
            {
                int translateX = (int)(translateStart.X * scale);
                int translateY = (int)(translateStart.Y * scale);
                xGraph.TranslateTransform(tempThreadThickness + translateX, tempThreadThickness + translateY);
                
                // Draw smoother lines
                xGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                for (int i = 0; i < blocks.Count; i++)
                {
                    using (Pen tempPen = new Pen(blocks[i].color, tempThreadThickness))
                    {
                        tempPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                        tempPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                        tempPen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;

                        foreach (Stitch thisStitch in blocks[i].stitches)
                        {
                            if (filterUglyStitches && // Check for filter ugly stitches option
                                !formatWarning && // Only filter stitches if we think we understand the format
                                thisStitch.calcLength() > filterUglyStitchesThreshold) // Check stitch length
                            {
                                // This stitch is too long, so skip it
                                continue;
                            }
                            Point tempA = new Point((int)(thisStitch.a.X * scale), (int)(thisStitch.a.Y * scale));
                            Point tempB = new Point((int)(thisStitch.b.X * scale), (int)(thisStitch.b.Y * scale));
                            xGraph.DrawLine(tempPen, tempA, tempB);
                        }
                    }
                }
            }

            return DrawArea;
        }
    }
}
