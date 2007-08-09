// $Id$

using System;
using System.Collections.Generic;
using System.Text;

namespace SettingsTester
{
    class fileSection
    {
        public string name;
        public List<stringPair> values = new List<stringPair>();
        //public List<string> unknownLines = new List<string>();

        public fileSection()
        {
            //sectionNum = -1;
            name = "";
            values = new List<stringPair>();
            //unknownLines = new List<string>();
        }
    }

    class stringPair
    {
        public string value;
        public string name;
        //public int lineNum;

        public stringPair()
        {
            value = "";
            name = "";
            //lineNum = -1;
        }
    }

    class nc_Settings
    {
        private string _filename = "";
        private List<fileSection> _sections = new List<fileSection>();

        public static string appPath()
        {
            return System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
        }

        /** If filename does not contain any slashes or backslashes, appPath()
         * is automatically prepended.*/
        public nc_Settings(string filename)
        {
            _filename = filename;
            if (!_filename.Contains("\"") && !_filename.Contains("/"))
            {
                _filename = System.IO.Path.Combine(appPath(), _filename);
            }
            _sections.Add(new fileSection()); //make sure the file has a default section, and it is the first
            readValues();
        }

        /** Returns the value associated with name if it exists,
         * returns a null string if it doesn't. */
        public string getValue(string name)
        {
            return getValue("", name);
        }

        /** Returns the value of _values[index] if it exists,
         * returns a null string if it doesn't. */
        //public string getValue(int index)
        //{
        //    string retval = null;
        //    if (index < _values.Count)
        //    {
        //        retval = _values[index].value;
        //    }
        //    return retval;
        //}

        public string getValue(string sectionName, string valueName)
        {
            string retval = null;
            for (int y = 0; y < _sections.Count; y++)
            {
                if (_sections[y].name.Equals(sectionName))
                {
                    for (int x = 0; x < _sections[y].values.Count; x++)
                    {
                        if (_sections[y].values[x].name.Equals(valueName))
                        {
                            retval = _sections[y].values[x].value;
                            break;
                        }
                    }
                }
            }
            return retval;
        }

        //public int getValueCount()
        //{
        //    return _values.Count;
        //}

        /** Returns the name of _values[index] if it exists,
         * returns a null string if it doesn't. */
        //public string getName(int index)
        //{
        //    string retval = null;
        //    if (index < _values.Count)
        //    {
        //        retval = _values[index].name;
        //    }
        //    return retval;
        //}

        /** Sets the value of name to value. If name doesn't exist,
         * it will be added. Uses default (no name) section. */
        public void setValue(string name, string value)
        {
            setValue("", name, value);
        }

        /** Sets the value of name to value. If name doesn't exist,
         * it will be added. Sections are added as needed. */
        public void setValue(string sectionName, string valueName, string value)
        {
            bool foundValue = false;
            bool foundSection = false;

            for (int y = 0; y < _sections.Count; y++)
            {
                if (_sections[y].name.Equals(sectionName))
                {
                    foundSection = true;
                    for (int x = 0; x < _sections[y].values.Count; x++)
                    {
                        if (_sections[y].values[x].name.Equals(valueName))
                        {
                            stringPair tmp = new stringPair();
                            tmp.name = valueName;
                            tmp.value = value;
                            //tmp.lineNum = _sections[y].values[x].lineNum;
                            foundValue = true;
                            _sections[y].values[x] = tmp;
                            break;
                        }
                    }
                    if (!foundValue)
                    {
                        stringPair tmp = new stringPair();
                        tmp.name = valueName;
                        tmp.value = value;
                        //tmp.lineNum = -1;
                        _sections[y].values.Add(tmp);
                    }
                }

            }
            if (!foundSection)
            {
                fileSection tmpSection = new fileSection();
                stringPair tmpValue = new stringPair();
                tmpSection.name = sectionName;
                //tmpValue.lineNum = -1;
                tmpValue.name = valueName;
                tmpValue.value = value;
                _sections.Add(tmpSection);
                _sections[_sections.Count - 1].values.Add(tmpValue);
            }
        }

        /** Writes all sections and name-value pairs to the file.
         * Creates the file if it doesn't exist. */
        public void save()
        {
            System.IO.StreamWriter outFile;
            string tempfile = System.IO.Path.GetDirectoryName(_filename);
            tempfile = System.IO.Path.Combine(tempfile, System.IO.Path.GetFileNameWithoutExtension(_filename) + ".tmp" + System.IO.Path.GetExtension(_filename));
            outFile = System.IO.File.CreateText(tempfile);
            for (int y = 0; y < _sections.Count; y++)
            {
                if (y != 0)
                {
                    outFile.WriteLine();
                    outFile.WriteLine("[" + _sections[y].name + "]");
                }
                for (int x = 0; x < _sections[y].values.Count; x++)
                {
                    outFile.WriteLine(_sections[y].values[x].name + "=" + _sections[y].values[x].value);
                }
            }
            outFile.Close();
            System.IO.File.Copy(tempfile, _filename, true);
            System.IO.File.Delete(tempfile);
        }

        /** Re-reads name-value pairs from file.
         * Uses same function as constructor. ( readValues() ) */
        public void reReadValues()
        {
            readValues();
        }

        /** Reads name-value pairs from file. Uses setValue()
         * to add name-value pairs to _sections[n].values, so
         * if the value already exists it will be overwritten,
         * if not it will be added. Any line without an equals 
         * sign will be discarded. Exception: lines that start 
         * with a [ and have a ] later will be read as section 
         * headers. Whitespace before and after will be eaten.*/
        private void readValues()
        {
            System.IO.StreamReader inFile;
            string inLine;
            string section = "";
            //stringPair tmp;
            if (System.IO.File.Exists(_filename))
            {
                inFile = System.IO.File.OpenText(_filename);
                inLine = inFile.ReadLine();
                while (inLine != null)
                {
                    if (inLine.Trim().StartsWith("[") && inLine.Contains("]"))
                    {
                        int start = inLine.IndexOf('[') +1;
                        int length = inLine.IndexOf(']') - start;
                        section = inLine.Substring(start, length);
                    }
                    else if (inLine.Contains("="))
                    {
                        setValue(section, inLine.Remove(inLine.IndexOf("=")), inLine.Substring(inLine.IndexOf("=") + 1));
                    }
                    inLine = inFile.ReadLine();
                }
                inFile.Close();
            }
        }
    }
}
