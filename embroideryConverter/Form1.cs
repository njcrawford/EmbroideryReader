using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace embroideryConverter
{
    public partial class Form1 : Form
    {
        protected string folderPath = string.Empty;
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BrowseBtn_Click(object sender, EventArgs e)
        {
            // Set the description of the dialog
            folderBrowserDialog1.Description = "Select a folder";

            // Show the FolderBrowserDialog and capture the result
            DialogResult result = folderBrowserDialog1.ShowDialog();

            // If the result is OK, meaning that a folder was selected and the OK button was clicked
            if (result == DialogResult.OK)
            {
                // Get the selected folder path
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            folderPath = textBox1.Text;
        }

        private void processDirectory(string dir)
        {
            foreach (string file in System.IO.Directory.EnumerateFiles(dir, "*.pes"))
            {
                GenerateImage(file);
            }

            foreach (string childDir in System.IO.Directory.EnumerateDirectories(dir))
            {
                processDirectory(childDir);
                updateStatus("");
            }
        }
        void GenerateImage(string filename)
        {
            try
            {
                if (!isNewer(filename)) return;
                updateStatus(filename);

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
                MessageBox.Show("Error processing " + filename + ". " + ex.Message);
            }
        }

        private void updateStatus(string filename)
        {
            this.status.Text = filename;
            Application.DoEvents();
        }

        private bool isNewer(string pesFile)
        {
            bool newer = true;

            string pngFile = pesFile.ToLower().Replace(".pes", ".png");

            if (!System.IO.File.Exists(pngFile))
            {
                newer = true;
            }
            else
            {
                System.IO.FileInfo pesInfo = new System.IO.FileInfo(pesFile);
                System.IO.FileInfo pngInfo = new System.IO.FileInfo(pngFile);

                if (pesInfo.LastWriteTime > pngInfo.LastWriteTime)
                {
                    newer = true;
                }
                else
                {
                    newer = false;
                }
            }
            
            return newer;
        }
        private void ConvertBtn_Click(object sender, EventArgs e)
        {
            folderPath = cleanUp(folderPath);
            if (isValid(folderPath))
            {
                processDirectory(folderPath);
                MessageBox.Show("Conversion Complete");
            }
            else
            {
                MessageBox.Show("Invalid Path");
            }
        }

        bool isValid(string path)
        {
            if (path == null) return false;
            if (path.Length == 0) return false;
            if (!System.IO.Directory.Exists(path)) return false;

            return true;
        }

        string cleanUp( string path )
        {
            string cleanPath = path;
            if (cleanPath == null) goto returnCleanPath;
            if (cleanPath.Length == 0) goto returnCleanPath;
            cleanPath = cleanPath.Trim('\"');

        returnCleanPath:
            return cleanPath;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}