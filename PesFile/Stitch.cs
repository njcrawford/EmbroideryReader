/*
Embroidery Reader - an application to view .pes embroidery designs

Copyright (C) 2018 Nathan Crawford
 
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
using System.Drawing;

namespace PesFile
{
    public class Stitch
    {
        public enum MoveBitSize
        {
            Unknown,
            SevenBits,
            TwelveBits
        };

        public enum StitchType
        {
            Unknown,
            NormalStitch,
            MovementBeginAnchor,
            MovementEndAnchor,
            MovementOnly
        }

        public Point a;
        public Point b;
        // Extra bits to output with debug info
        public int extraBits1;
        public int extraBits2;
        // Was this stitch represented as 7 bit move or 12 bit move in the file
        public MoveBitSize XMoveBits;
        public MoveBitSize YMoveBits;
        public StitchType stitchType;

        public Stitch(Point pointA, Point pointB)
        {
            this.a = pointA;
            this.b = pointB;
        }

        public Stitch(Point pointA, Point pointB, int extraBits1, int extraBits2, MoveBitSize XMoveBits, MoveBitSize YMoveBits, StitchType stitchType)
        {
            this.a = pointA;
            this.b = pointB;
            this.extraBits1 = extraBits1;
            this.extraBits2 = extraBits2;
            this.XMoveBits = XMoveBits;
            this.YMoveBits = YMoveBits;
            this.stitchType = stitchType;
        }

        public double CalcLength()
        {
            double diffx = Math.Abs(a.X - b.X);
            double diffy = Math.Abs(a.Y - b.Y);
            return Math.Sqrt(Math.Pow(diffx, 2.0) + Math.Pow(diffy, 2.0));
        }
    }
}
