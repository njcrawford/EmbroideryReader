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
You can contact me at http://www.njcrawford.com/contact/.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace embroideryTester
{
    public partial class Form1 : Form
    {
        //BinaryReader file;
        //byte[] shorts;

        //int byteNume = 0;
        //short itemNumber = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //byte[] bytes;

                //bytes = File.ReadAllBytes(openFileDialog1.FileName);
                //shorts = File.ReadAllBytes(openFileDialog1.FileName);
                //GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                //IntPtr ptr = handle.AddrOfPinnedObject();
                ////ReDim bytes((bytes.Length \ 2) - 1)
                ////int temp = bytes.Length;
                ////temp = temp /2
                //shorts = new short[bytes.Length / 2 - 1];
                //Marshal.Copy(ptr, shorts, 0, shorts.Length);
                //handle.Free();
                //this.Text = openFileDialog1.FileName;
                //btnNext.Enabled = true;

                System.IO.BinaryReader fileIn = new System.IO.BinaryReader(System.IO.File.Open(openFileDialog1.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read));

                long restorePos = fileIn.BaseStream.Position;
                byte[] tempbytes = fileIn.ReadBytes(10240);
                string tempstring = "";
                for (int ctr = 0; ctr < tempbytes.Length; ctr++)
                {
                    tempstring += (char)tempbytes[ctr];
                }
                if (tempstring.Contains("CEmbOne"))
                {
                    fileIn.BaseStream.Position = restorePos + tempstring.IndexOf("CEmbOne") + 7;
                }
                else if (tempstring.Contains("CEmbPunch"))
                {
                    fileIn.BaseStream.Position = restorePos + tempstring.IndexOf("CEmbPunch") + 9;
                }
                string temp = "";
                textBox1.Visible = false;
                while(fileIn.BaseStream.Position +1< fileIn.BaseStream.Length)
                {
                    temp += fileIn.ReadInt16().ToString() + Environment.NewLine;
                }
                textBox1.Text = temp;
                textBox1.Visible = true;
            }

        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            ////textBox1.Text += file.BaseStream.Position.ToString() +" 0x"+ file.ReadByte().ToString("X")+Environment.NewLine;
            //string temp = "";
            //textBox1.Visible = false;
            //for (int i = 0; i < shorts.Length; i++)
            //{
            //    temp += shorts[i].ToString("X") + Environment.NewLine;
            //}
            //textBox1.Text = temp;
            //textBox1.Visible = true;
            //textBox1.Select(textBox1.Text.Length, 0);
            //textBox1.ScrollToCaret();

            ////itemNumber++;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.MaxLength = Int32.MaxValue;
            this.BackColor = GetRGB(2037741);
        }

        private Color getColorFromIndex(int index)
        {
            Color retval = Color.Black;
            switch (index)
            {
                case 1:
                    retval = Color.FromArgb(8134414);
                    break;
                case 2:
                    retval = Color.FromArgb(11561576);
                    break;
            }
            return retval;
        }

        private Color GetRGB(long RGB)
        {
            long Red, Green, Blue;
            Red = RGB & 255;
            Green = RGB % 256 & 255;
            Blue = RGB % 256 ^ 2 & 255;
            Console.WriteLine("RGB: " + Blue.ToString() + ", " + Green.ToString() + ", " + Red.ToString());
            return Color.FromArgb((int)Blue, (int)Green, (int)Red);
        }
    }
}
