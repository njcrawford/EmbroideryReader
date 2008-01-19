using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace embroideryInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                try
                {
                    PesFile.PesFile design = new PesFile.PesFile(args[0]);
                    design.saveDebugInfo();

                    //Bitmap DrawArea = design.designToBitmap(5);
                    //Bitmap temp = new Bitmap(DrawArea.Width, DrawArea.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    //Graphics tempGraph = Graphics.FromImage(temp);
                    //tempGraph.FillRectangle(Brushes.White, 0, 0, temp.Width, temp.Height);
                    //tempGraph.DrawImageUnscaled(DrawArea, 0, 0);
                    //tempGraph.Dispose();
                    //temp.Save(args[0] + ".png");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else
            {
                Console.WriteLine("Specify input file");
                for (int x = 0; x < args.Length; x++)
                {
                    Console.WriteLine(args[x]);
                }
            }
        }
    }
}
