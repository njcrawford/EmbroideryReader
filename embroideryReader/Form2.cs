using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace embroideryReader
{
    public partial class Form2 : Form
    {
        //public List<Point> pointList = new List<Point>();
        //public Point[] points = new Point[0];
        public Color drawColor = System.Drawing.Color.Black;
        private Bitmap DrawArea;
        public Point prevPoint = new Point(-1,-1);

        public Form2()
        {
            InitializeComponent();
            DrawArea = new Bitmap(1000, 1000);
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            //Point[] points = new Point[pointList.Count];
            //pointList.CopyTo(points);
            //if (points.Length >= 2)
            //{
            //    e.Graphics.DrawLines(new System.Drawing.Pen(drawColor), points);
            //}
            e.Graphics.DrawImage(DrawArea, 0, 0);
        }

        public void addPoint(Point p)
        {
            //Point[] tmp = new Point[points.Length + 1];
            //points.CopyTo(tmp,0);
            //tmp[tmp.Length - 1] = p;
            //points = tmp;

            if (prevPoint != null && prevPoint.X != -1 && prevPoint.Y != -1)
            {
                Graphics xGraph;
                xGraph = Graphics.FromImage(DrawArea);
                xGraph.DrawLine(new Pen(drawColor), prevPoint, p);
                xGraph.Dispose();
            }
            prevPoint = p;

        }
    }
}