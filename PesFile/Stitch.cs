/*
Embroidery Reader - an application to view .pes embroidery designs

Copyright (C) 2014 Nathan Crawford
 
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
using System.Linq;
using System.Text;
using System.Drawing;

namespace PesFile
{
    public class Stitch
    {
        public Point a;
        public Point b;

        public Stitch(Point pointA, Point pointB)
        {
            this.a = pointA;
            this.b = pointB;
        }

        public double calcLength()
        {
            double diffx = Math.Abs(a.X - b.X);
            double diffy = Math.Abs(a.Y - b.Y);
            return Math.Sqrt(Math.Pow(diffx, 2.0) + Math.Pow(diffy, 2.0));
        }
    }
}
