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
using System.Drawing;
using System.IO;

namespace PesFile
{
    public class PECFormatException : System.Exception
    {
        public PECFormatException(string message) : base(message) { }
    }

    public class PesFile
    {
        private int imageWidth;
        private int imageHeight;
        private string _filename;
        public List<StitchBlock> blocks = new List<StitchBlock>();
        public List<Tuple<int, int>> colorTable = new List<Tuple<int, int>>();
        private Int64 startStitches = 0;
        private Point translateStart;
        private UInt16 pesVersion;
        // Native format appears to use 0.1mm steps, for 254 steps per inch
        public int NativeDPI = 254;


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
        private Int16 Get12Bit2sComplement(byte high, byte low)
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
        private SByte Get7Bit2sComplement(byte b)
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
            _filename = filename;

            // The using statements ensure fileIn is closed, no matter how the statement is exited.
            // Open the file in read-only mode, but allow read-write sharing. This prevents a case where opening
            // the file read-only with read sharing could fail because some other application already has it
            // open with read-write access even though it's not writing to the file. (I suspect some antivirus
            // programs may be doing this) Using this mode may allow reading half-written files, but I don't expect
            // that will be an issue with PES designs, since they're generally written once when downloaded and left
            // alone after that.
            using (FileStream fileStreamIn = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BinaryReader fileIn = new BinaryReader(fileStreamIn))
                {
                    string startFileSig = "";
                    for (int i = 0; i < 4; i++) // 4 bytes
                    {
                        // This needs to be read as a byte, since characters can be multiple bytes depending on encoding
                        startFileSig += (char)fileIn.ReadByte();
                    }
                    if (startFileSig != "#PES")
                    {
                        // This is not a file that we can read
                        throw new PECFormatException("Missing #PES at beginning of file");
                    }

                    // PES version
                    string versionString = "";
                    for (int i = 0; i < 4; i++) // 4 bytes
                    {
                        // This needs to be read as a byte, since characters can be multiple bytes depending on encoding
                        versionString += (char)fileIn.ReadByte();
                    }
                    if (!UInt16.TryParse(versionString, out pesVersion))
                    {
                        // This is not a file that we can read
                        throw new PECFormatException("PES version is not the correct format");
                    }

                    int pecstart = fileIn.ReadInt32();
                    // Sanity check on PEC start position
                    if (fileIn.BaseStream.Length < (pecstart + 532))
                    {
                        // This file is probably truncated
                        throw new PECFormatException("File appears to be truncated (PEC section is beyond end of file)");
                    }

                    // Read number of colors in this design
                    fileIn.BaseStream.Position = pecstart + 48;
                    int numColors = fileIn.ReadByte() + 1;
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
                    Stitch prevStitch = null;
                    bool firstStitchOfBlock = true;
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
                            curBlock.color = GetColorFromIndex(colorIndex);
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
                            curBlock.color = GetColorFromIndex(colorIndex);
                            //read useless(?) byte
                            // The value of this 'useless' byte seems to start with 2 for the first block and
                            // alternate between 2 and 1 for every other block after that. The only exception
                            // I've noted is the last block which appears to always be 0.
                            curBlock.unknownStartByte = fileIn.ReadByte();
                            blocks.Add(curBlock);

                            firstStitchOfBlock = true;

                            tempStitches = new List<Stitch>();
                        }
                        else
                        {
                            int deltaX = 0;
                            int deltaY = 0;
                            int extraBits1 = 0x00;
                            Stitch.MoveBitSize xMoveBits;
                            Stitch.MoveBitSize yMoveBits;
                            if ((val1 & 0x80) == 0x80)
                            {
                                // Save the top 4 bits to output with debug info
                                // The top bit means this is a 12 bit value, but I don't know what the next 3 bits mean.
                                // The only combinations I've observed in real files are 0x10 and 0x20. 0x20 occurs
                                // about 4 times as much as 0x10 in the samples I have available.
                                extraBits1 = val1 & 0x70;

                                // This is a 12-bit int. Allows for needle movement
                                // of up to +2047 or -2048.
                                deltaX = Get12Bit2sComplement(val1, val2);

                                xMoveBits = Stitch.MoveBitSize.TwelveBits;

                                // The X value used both bytes, so read next byte
                                // for Y value.
                                val1 = fileIn.ReadByte();
                            }
                            else
                            {
                                // This is a 7-bit int. Allows for needle movement
                                // of up to +63 or -64.
                                deltaX = Get7Bit2sComplement(val1);

                                xMoveBits = Stitch.MoveBitSize.SevenBits;

                                // The X value only used 1 byte, so copy the second
                                // to to the first for Y value.
                                val1 = val2;
                            }

                            int extraBits2 = 0x00;
                            if ((val1 & 0x80) == 0x80)
                            {
                                // Save the top 4 bits to output with debug info
                                // The top bit means this is a 12 bit value, but I don't know what the next 3 bits mean.
                                // In all the files I've checked, extraBits2 is the same as extraBits1.
                                extraBits2 = val1 & 0x70;

                                // This is a 12-bit int. Allows for needle movement
                                // of up to +2047 or -2048.
                                // Read in the next byte to get the full value
                                val2 = fileIn.ReadByte();
                                deltaY = Get12Bit2sComplement(val1, val2);

                                yMoveBits = Stitch.MoveBitSize.TwelveBits;
                            }
                            else
                            {
                                // This is a 7-bit int. Allows for needle movement
                                // of up to +63 or -64.
                                deltaY = Get7Bit2sComplement(val1);

                                yMoveBits = Stitch.MoveBitSize.SevenBits;
                                // Finished reading data for this stitch, no more
                                // bytes needed.
                            }

                            Stitch.StitchType stitchType;
                            if (deltaX == 0 && deltaY == 0)
                            {
                                // A stitch with zero movement seems to indicate the current and next stitches are movement only.
                                // Almost always occurs at the end of a block.
                                stitchType = Stitch.StitchType.MovementBeginAnchor;
                            }
                            else if (extraBits1 == 0x20 && extraBits2 == 0x20)
                            {
                                // A stitch with extrabits 0x20 seems to indicate the current and next stitches are movement only.
                                // Almost always occurs within a block, not the beginning or end.
                                stitchType = Stitch.StitchType.MovementOnly;
                            }
                            else if (extraBits1 == 0x10 && extraBits2 == 0x10)
                            {
                                // A stitch with extrabits 0x10 seems to indicate the current stitch is movement only.
                                // Almost always occurs at the beginning of a block.
                                stitchType = Stitch.StitchType.MovementEndAnchor;
                            }
                            else if (prevStitch != null &&
                                (prevStitch.stitchType == Stitch.StitchType.MovementOnly ||
                                prevStitch.stitchType == Stitch.StitchType.MovementBeginAnchor))
                            {
                                stitchType = Stitch.StitchType.MovementEndAnchor;
                            }
                            else
                            {
                                if (firstStitchOfBlock)
                                {
                                    // First stitch of block is usually a movement to position the needle in the block
                                    firstStitchOfBlock = false;
                                    stitchType = Stitch.StitchType.MovementEndAnchor;
                                }
                                else
                                {
                                    // Assume everything else is a normal stitch
                                    stitchType = Stitch.StitchType.NormalStitch;
                                }
                            }

                            // Add stitch to list
                            prevStitch = new Stitch(
                                    new Point(prevX, prevY),
                                    new Point(prevX + deltaX, prevY + deltaY),
                                    extraBits1,
                                    extraBits2,
                                    xMoveBits,
                                    yMoveBits,
                                    stitchType
                                );
                            tempStitches.Add(prevStitch);

                            // Calculate new "previous" position
                            prevX = prevX + deltaX;
                            prevY = prevY + deltaY;

                            // Update maximum distances
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
                }
            }
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
        public string SaveDebugInfo()
        {
            string retval = System.IO.Path.ChangeExtension(_filename, ".txt");
            using (System.IO.StreamWriter outfile = new System.IO.StreamWriter(retval))
            {
                outfile.Write(GetDebugInfo());
                outfile.Close();
            }
            return retval;
        }

        public string GetDebugInfo()
        {
            System.IO.StringWriter outfile = new System.IO.StringWriter();
            outfile.WriteLine(_filename);
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
                int blockCount = 1;
                foreach(StitchBlock thisBlock in blocks)
                {
                    outfile.WriteLine("block " + blockCount.ToString() + " start (color index " + thisBlock.colorIndex + ")");
                    outfile.WriteLine("unknown start byte: " + thisBlock.unknownStartByte.ToString("X2"));
                    foreach (Stitch thisStitch in thisBlock.stitches)
                    {
                        string tempLine = thisStitch.a.ToString() + " - " + thisStitch.b.ToString() + ", length " + thisStitch.CalcLength().ToString("F02");
                        if (thisStitch.extraBits1 != 0x00)
                        {
                            tempLine += " (extra bits 1: " + thisStitch.extraBits1.ToString("X2") + ")";
                        }
                        if (thisStitch.extraBits2 != 0x00)
                        {
                            tempLine += " (extra bits 2: " + thisStitch.extraBits2.ToString("X2") + ")";
                        }

                        if(thisStitch.XMoveBits == Stitch.MoveBitSize.SevenBits)
                        {
                            tempLine += " (7 bit X move)";
                        }
                        else if(thisStitch.XMoveBits == Stitch.MoveBitSize.TwelveBits)
                        {
                            tempLine += " (12 bit X move)";
                        }

                        if (thisStitch.YMoveBits == Stitch.MoveBitSize.SevenBits)
                        {
                            tempLine += " (7 bit Y move)";
                        }
                        else if (thisStitch.YMoveBits == Stitch.MoveBitSize.TwelveBits)
                        {
                            tempLine += " (12 bit Y move)";
                        }

                        tempLine += " (type " + thisStitch.stitchType + ")";

                        outfile.WriteLine(tempLine);
                    }
                    blockCount++;
                }
            }
            outfile.Close();
            return outfile.ToString();
        }

        public bool GetColorWarning()
        {
            return colorWarning;
        }

        public bool GetFormatWarning()
        {
            return formatWarning;
        }

        private Color GetColorFromIndex(int index)
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

        struct OptimizedBlockData
        {
            public Color color;
            public Point [] points;

            public OptimizedBlockData(Color color, Point [] points)
            {
                this.color = color;
                this.points = points;
            }
        }

        private List<OptimizedBlockData> GetOptimizedDrawData(StitchBlock block, float scale, bool filterUglyStitches, double filterUglyStitchesThreshold)
        {
            List<OptimizedBlockData> retval = new List<OptimizedBlockData>();

            // Skip this block if it doesn't have stitches, for any reason
            if (block.stitches.Length == 0)
            {
                return retval;
            }

            // Start first block
            List<Point> currentPoints = new List<Point>();

            foreach (Stitch thisStitch in block.stitches)
            {
                if (filterUglyStitches && // Check for filter ugly stitches option
                    !formatWarning && // Only filter stitches if we think we understand the format
                    thisStitch.CalcLength() > filterUglyStitchesThreshold) // Check stitch length
                {
                    // This stitch is too long, so skip it and start a new block
                    if (currentPoints.Count != 0)
                    {
                        retval.Add(new OptimizedBlockData(block.color, currentPoints.ToArray()));
                    }
                    currentPoints = new List<Point>();
                    continue;
                }

                // Style special stitches differently
                // TODO: Finish figuring out the remaining stitch types
                if (filterUglyStitches && (thisStitch.stitchType == Stitch.StitchType.MovementBeginAnchor ||
                    thisStitch.stitchType == Stitch.StitchType.MovementOnly ||
                    thisStitch.stitchType == Stitch.StitchType.MovementEndAnchor))
                {
                    // Skip these stitch types, and start a new block
                    if (currentPoints.Count != 0)
                    {
                        retval.Add(new OptimizedBlockData(block.color, currentPoints.ToArray()));
                    }
                    currentPoints = new List<Point>();
                    continue;
                }

                if(currentPoints.Count == 0)
                {
                    // If this is the first point in this optimized block, we'll need the previous point to form a line
                    currentPoints.Add(new Point((int)(thisStitch.a.X * scale), (int)(thisStitch.a.Y * scale)));
                }
                currentPoints.Add(new Point((int)(thisStitch.b.X * scale), (int)(thisStitch.b.Y * scale)));
            }

            if(currentPoints.Count != 0)
            {
                retval.Add(new OptimizedBlockData(block.color, currentPoints.ToArray()));
            }

            return retval;
        }

        // When scale is 1.0, each pixel appears to be 0.1mm, or about 254 ppi
        public Bitmap DesignToBitmap(Single threadThickness, bool filterUglyStitches, double filterUglyStitchesThreshold, float scale)
        {
            // Do some basic input checks
            if(scale < 0.0000001f)
            {
                throw new ArgumentException("Scale must be > 0");
            }
            if(filterUglyStitchesThreshold < 1.0)
            {
                throw new ArgumentException("Filter ungly stitches threshold must be at least 1.0");
            }
            if(threadThickness < 0.1)
            {
                throw new ArgumentException("Thread thickness must be at least 0.1");
            }

            int imageWidth = (int)((GetWidth() + (threadThickness * 2)) * scale);
            int imageHeight = (int)((GetHeight() + (threadThickness * 2)) * scale);
            float tempThreadThickness = threadThickness * scale;
            // This assumes the image format will use 4 bytes per pixel
            UInt64 imageRequiredRam = (UInt64)(imageHeight * imageWidth * 4);

            // Check for a bitmap that might be too large.
            // Apparently, there is a 2GB (minus a little overhead) limit for objects on the GC heap in
            // .Net, even when using a 64 bit framework. More detail at:
            // https://blogs.msdn.microsoft.com/joshwil/2005/08/10/bigarrayt-getting-around-the-2gb-array-size-limit/
            if(imageRequiredRam > (Int32.MaxValue - 1024))
            {
                throw new ArgumentOutOfRangeException("Generated image would be too large (" + imageWidth + "x" + imageHeight + ", " + imageRequiredRam + " bytes)");
            }
            Bitmap DrawArea = new Bitmap(imageWidth, imageHeight);
            // Return now if for any reason there aren't any blocks
            if(blocks.Count == 0)
            {
                return DrawArea;
            }

            using (Graphics xGraph = Graphics.FromImage(DrawArea))
            {
                int translateX = (int)(translateStart.X * scale);
                int translateY = (int)(translateStart.Y * scale);
                xGraph.TranslateTransform(tempThreadThickness + translateX, tempThreadThickness + translateY);
                
                // Draw smoother lines
                xGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                // Only use one pen - I think this will be more resource friendly than creating a new pen for each block
                using (Pen tempPen = new Pen(blocks[0].color, tempThreadThickness))
                {
                    // Set rounded ends and joints
                    tempPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    tempPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    tempPen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;

                    // List to build up draw-optimized data
                    List<OptimizedBlockData> optimizedBlocks = new List<OptimizedBlockData>();

                    // Get optimized data
                    foreach(StitchBlock thisBlock in blocks)
                    {
                        optimizedBlocks.AddRange(GetOptimizedDrawData(thisBlock, scale, filterUglyStitches, filterUglyStitchesThreshold));
                    }

                    // Draw using optimized data
                    foreach(OptimizedBlockData optBlock in optimizedBlocks)
                    {
                        tempPen.Color = optBlock.color;
                        xGraph.DrawLines(tempPen, optBlock.points);
                    }

                    // Done with optimized data
                    optimizedBlocks = null;
                }
            }

            return DrawArea;
        }
    }
}
