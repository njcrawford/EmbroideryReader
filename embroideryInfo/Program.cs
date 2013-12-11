/*
Embroidery Reader - an application to view .pes embroidery designs

Copyright (C) 2011  Nathan Crawford
 
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
You can contact me at http://www.njcrawford.com/contact.php.
*/

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

                    if (args[0] == "--image" && args.Length > 1)
                    {
                        PesFile.PesFile design = new PesFile.PesFile(args[1]);
                        Bitmap DrawArea = design.designToBitmap(5, false, 0);
                        Bitmap temp = new Bitmap(DrawArea.Width, DrawArea.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        Graphics tempGraph = Graphics.FromImage(temp);
                        tempGraph.FillRectangle(Brushes.White, 0, 0, temp.Width, temp.Height);
                        tempGraph.DrawImageUnscaled(DrawArea, 0, 0);
                        tempGraph.Dispose();
                        temp.Save(System.IO.Path.ChangeExtension(args[1], ".png"));
                    }
                    else
                    {
                        PesFile.PesFile design = new PesFile.PesFile(args[0]);
                        design.saveDebugInfo();
                    }
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
