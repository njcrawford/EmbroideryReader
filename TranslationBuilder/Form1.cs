using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OdsReadWrite;
using System.IO;

namespace TranslationBuilder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            lblStatus.Text = "Building...";
            this.Invalidate();
            Application.DoEvents();

            List<string> languageName = new List<string>();
            List<string> outputs = new List<string>();
            DataSet theData = new OdsReaderWriter().ReadOdsFile("..\\..\\..\\translations\\translations.ods");
            int rowCounter = 0;
            foreach (DataRow thisRow in theData.Tables[0].Rows)
            {
                if (rowCounter == 0)
                {
                    for (int i = 2; i < thisRow.ItemArray.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(thisRow.ItemArray[i].ToString()))
                        {
                            languageName.Add(thisRow.ItemArray[i].ToString());
                            outputs.Add("");
                        }
                    }
                }
                else
                {
                    string textId;
                    if (!String.IsNullOrEmpty(thisRow.ItemArray[0].ToString()))
                    {
                        textId = thisRow.ItemArray[0].ToString();
                    }
                    else
                    {
                        continue;
                    }

                    for (int i = 2; i < thisRow.ItemArray.Length; i++)
                    {
                        if (!String.IsNullOrEmpty(thisRow.ItemArray[i].ToString()))
                        {
                            outputs[i - 2] += textId + "=" + thisRow.ItemArray[i] + Environment.NewLine;
                        }
                    }
                }
                rowCounter++;
            }
            for (int i = 0; i < outputs.Count; i++)
            {
                StreamWriter outfile = new StreamWriter("..\\..\\..\\translations\\" + languageName[i] + ".ini");
                outfile.WriteLine("# Embroidery Reader language strings file. To translate Embroidery Reader to a new");
                outfile.WriteLine("# language, copy this file to a new name and translate each string.");
                outfile.WriteLine("# If you make a new translation, please contact me! I'd love to");
                outfile.WriteLine("# include your translation with Embroidery Reader.");
                outfile.WriteLine("# The best way to contact me is through http://www.njcrawford.com/contact/.");
                outfile.Write(outputs[i]);
                outfile.Close();
                //MessageBox.Show(languageName[i] + Environment.NewLine + outputs[i]);
            }
            lblStatus.Text = "Done";
        }
    }
}
