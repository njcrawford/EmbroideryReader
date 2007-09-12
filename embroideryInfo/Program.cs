using System;
using System.Collections.Generic;
using System.Text;

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
                    embroideryReader.PesFile design = new embroideryReader.PesFile(args[0]);
                    design.saveDebugInfo();
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
