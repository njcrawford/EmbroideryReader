using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PesFile
{
    public class StitchBlock
    {
        public Color color;
        public Int32 colorIndex;
        public Int32 stitchesTotal;
        public Stitch[] stitches;
        public byte unknownStartByte;
        public StitchBlock()
        {
            color = System.Drawing.Color.Black;
        }
    }
}
