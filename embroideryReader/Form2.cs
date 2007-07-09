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
        public Point[] points = new Point[0];
        public Color drawColor = System.Drawing.Color.Black;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            //Point[] points = new Point[pointList.Count];
            //pointList.CopyTo(points);
            e.Graphics.DrawLines(new System.Drawing.Pen(drawColor), points);
        }
    }
}