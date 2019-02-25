/*
Embroidery Reader - an application to view .pes embroidery designs

Copyright (C) 2019 Nathan Crawford
 
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
    public class StitchBlock
    {
        public Color color;
        public Int32 colorIndex;
        public Int32 stitchesTotal;
        public Stitch[] stitches;
        public byte unknownStartByte;
        public StitchBlock()
        {
            color = Color.Black;
        }
    }
}
