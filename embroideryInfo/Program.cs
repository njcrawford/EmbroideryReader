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
using System.Drawing;

namespace embroideryInfo
{
    class Program
    {
        static void printHelp()
        {
            Console.WriteLine("No input file specified.");
            Console.WriteLine("To generate design debug text file:");
            Console.WriteLine("embroideryInfo.exe input.pes");
            Console.WriteLine("To generate PNG file:");
            Console.WriteLine("embroideryInfo.exe --image input.pes");
        }

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if(args[0] == "--help" || args[0] == "-h" || args[0] == "/?")
                {
                    printHelp();
                }
                else if (args[0] == "--image" && args.Length > 1)
                {
                    try
                    {
                        PesFile.PesFile design = new PesFile.PesFile(args[1]);
                        Bitmap DrawArea = design.designToBitmap(5.0f, false, 0.0f, 1.0f);
                        Bitmap temp = new Bitmap(DrawArea.Width, DrawArea.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                        Graphics tempGraph = Graphics.FromImage(temp);
                        tempGraph.FillRectangle(Brushes.White, 0, 0, temp.Width, temp.Height);
                        tempGraph.DrawImageUnscaled(DrawArea, 0, 0);
                        tempGraph.Dispose();
                        temp.Save(System.IO.Path.ChangeExtension(args[1], ".png"));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                else
                {
                    try
                    { 
                        PesFile.PesFile design = new PesFile.PesFile(args[0]);
                        design.saveDebugInfo();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            else
            {
                printHelp();
            }
        }
    }
}
