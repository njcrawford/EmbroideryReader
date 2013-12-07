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
