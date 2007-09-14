using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace emroideryTester
{
    public partial class Form1 : Form
    {
        //BinaryReader file;
        short[] shorts;

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
                byte[] bytes;

                bytes = File.ReadAllBytes(openFileDialog1.FileName);
                GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                IntPtr ptr = handle.AddrOfPinnedObject();
                //ReDim bytes((bytes.Length \ 2) - 1)
                //int temp = bytes.Length;
                //temp = temp /2
                shorts = new short[bytes.Length / 2 - 1];
                Marshal.Copy(ptr, shorts, 0, shorts.Length);
                handle.Free();
                this.Text = openFileDialog1.FileName;
                btnNext.Enabled = true;
            }

        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            //textBox1.Text += file.BaseStream.Position.ToString() +" 0x"+ file.ReadByte().ToString("X")+Environment.NewLine;
            string temp = "";
            textBox1.Visible = false;
            for (int i = 0; i < shorts.Length; i++)
            {
                temp += shorts[i].ToString() + Environment.NewLine;
            }
            textBox1.Text = temp;
            textBox1.Visible = true;
            textBox1.Select(textBox1.Text.Length, 0);
            textBox1.ScrollToCaret();

            //itemNumber++;
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