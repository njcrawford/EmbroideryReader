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

namespace embroideryInfo
{
    class Program
    {
        static void PrintHelp()
        {
            Console.WriteLine("No input file specified.");
            Console.WriteLine("To generate design debug text file:");
            Console.WriteLine("embroideryInfo.exe --debug input.pes");
            Console.WriteLine("To generate design debug text file for all files in current folder:");
            Console.WriteLine("embroideryInfo.exe --debug --all");
            Console.WriteLine("To generate PNG file:");
            Console.WriteLine("embroideryInfo.exe --image input.pes");
            Console.WriteLine("To generate PNG file for all files in the current folder:");
            Console.WriteLine("embroideryInfo.exe --image --all");
        }

        static void GenerateDebug(string filename)
        {
            try
            {
                PesFile.PesFile design = new PesFile.PesFile(filename);
                design.SaveDebugInfo();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void GenerateImage(string filename)
        {
            try
            {
                PesFile.PesFile design = new PesFile.PesFile(filename);
                Bitmap DrawArea = design.DesignToBitmap(5.0f, false, 0.0f, 1.0f);
                Bitmap temp = new Bitmap(DrawArea.Width, DrawArea.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                using (Graphics tempGraph = Graphics.FromImage(temp))
                {
                    tempGraph.FillRectangle(Brushes.White, 0, 0, temp.Width, temp.Height);
                    tempGraph.DrawImageUnscaled(DrawArea, 0, 0);
                    tempGraph.Dispose();
                    temp.Save(System.IO.Path.ChangeExtension(filename, ".png"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if(args[0] == "--help" || args[0] == "-h" || args[0] == "/?")
                {
                    PrintHelp();
                }
                else if (args[0] == "--image" && args.Length > 1)
                {
                    if (args[1] == "--all")
                    {
                        foreach(string file in System.IO.Directory.EnumerateFiles(Environment.CurrentDirectory, "*.pes"))
                        {
                            GenerateImage(file);
                        }
                    }
                    else
                    {
                        GenerateImage(args[1]);
                    }
                }
                else if (args[0] == "--debug" && args.Length > 1)
                {
                    if (args[1] == "--all")
                    {
                        foreach (string file in System.IO.Directory.EnumerateFiles(Environment.CurrentDirectory, "*.pes"))
                        {
                            GenerateDebug(file);
                        }
                    }
                    else
                    {
                        GenerateDebug(args[1]);
                    }
                }
            }
            else
            {
                PrintHelp();
            }
        }
    }
}
