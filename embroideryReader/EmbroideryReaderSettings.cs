/*
Embroidery Reader - an application to view .pes embroidery designs

Copyright (C) 2011 Nathan Crawford
 
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
You can contact me at http://www.njcrawford.com/contact
*/


using System;
using System.Collections.Generic;
using System.Text;

namespace embroideryReader
{
    public class EmbroideryReaderSettings
    {
        private NJCrawford.IniFile settings;

        // update location is obsolete, but leaving here for historical purposes
        private const String SETTING_UPDATE_LOCATION = "update location";

        private const String SECTION_BACKGROUND_COLOR = "background color";
        private const String SETTING_BACKGROUND_COLOR_ENABLED = "enabled";
        private const String SETTING_BACKGROUND_COLOR_RED = "red";
        private const String SETTING_BACKGROUND_COLOR_GREEN = "green";
        private const String SETTING_BACKGROUND_COLOR_BLUE = "blue";

        private const String SETTING_FILTER_STITCHES = "filter stitches";
        private const String SETTING_FILTER_STITCHES_THRESHOLD = "filter stitches threshold";

        private const String SETTING_THREAD_THICKNESS = "thread thickness";

        private const String SETTING_LAST_OPEN_FILE_FOLDER = "last open file folder";

        private const String SETTING_LAST_SAVE_IMAGE_LOCATION = "last save image location";
	
        private const String SETTING_WINDOW_WIDTH = "window width";
        private const String SETTING_WINDOW_HEIGHT = "window height";

        private const String VALUE_YES = "yes";
        private const String VALUE_NO = "no";

        private const String VALUE_TRUE = "true";
        private const String VALUE_FALSE = "false";

        private const String UPDATE_URL = "http://www.njcrawford.com/updates/embroidery-reader.ini";
        private const String SETTINGS_FILENAME = "embroideryreader.ini";
        private const String SETTINGS_PATH_COMPANY = "NJCrawford Software";
        private const String SETTINGS_PATH_APP_NAME = "Embroidery Reader";

        public EmbroideryReaderSettings()
        {
            string settingsPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            settingsPath = System.IO.Path.Combine(settingsPath, SETTINGS_PATH_COMPANY);
            settingsPath = System.IO.Path.Combine(settingsPath, SETTINGS_PATH_APP_NAME);
            if (!System.IO.Directory.Exists(settingsPath))
            {
                System.IO.Directory.CreateDirectory(settingsPath);
            }
            settingsPath = System.IO.Path.Combine(settingsPath, SETTINGS_FILENAME);
            // new settings file in application data folder
            settings = new NJCrawford.IniFile(settingsPath);

            // if the new file doesn't exist but the old one does, copy useful settings from old file to new
            if (!System.IO.File.Exists(settingsPath) && System.IO.File.Exists(SETTINGS_FILENAME))
            {
                // Old settings file stored in installation folder breaks on 
                // Windows 7. 
                NJCrawford.IniFile oldSettings = new NJCrawford.IniFile(SETTINGS_FILENAME);

                if (!String.IsNullOrEmpty(oldSettings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_ENABLED)))
                {
                    settings.setValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_ENABLED, oldSettings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_ENABLED));
                }

                if (!String.IsNullOrEmpty(oldSettings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_RED)))
                {
                    settings.setValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_RED, oldSettings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_RED));
                }

                if (!String.IsNullOrEmpty(oldSettings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_GREEN)))
                {
                    settings.setValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_GREEN, oldSettings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_GREEN));
                }

                if (!String.IsNullOrEmpty(oldSettings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_BLUE)))
                {
                    settings.setValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_BLUE, oldSettings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_BLUE));
                }

                if (!String.IsNullOrEmpty(oldSettings.getValue(SETTING_FILTER_STITCHES)))
                {
                    settings.setValue(SETTING_FILTER_STITCHES, oldSettings.getValue(SETTING_FILTER_STITCHES));
                }

                if (!String.IsNullOrEmpty(oldSettings.getValue(SETTING_FILTER_STITCHES_THRESHOLD)))
                {
                    settings.setValue(SETTING_FILTER_STITCHES_THRESHOLD, oldSettings.getValue(SETTING_FILTER_STITCHES_THRESHOLD));
                }

                if (!String.IsNullOrEmpty(oldSettings.getValue(SETTING_THREAD_THICKNESS)))
                {
                    settings.setValue(SETTING_THREAD_THICKNESS, oldSettings.getValue(SETTING_THREAD_THICKNESS));
                }

                if (!String.IsNullOrEmpty(oldSettings.getValue(SETTING_LAST_OPEN_FILE_FOLDER)))
                {
                    settings.setValue(SETTING_LAST_OPEN_FILE_FOLDER, oldSettings.getValue(SETTING_LAST_OPEN_FILE_FOLDER));
                }

                if (!String.IsNullOrEmpty(oldSettings.getValue(SETTING_LAST_SAVE_IMAGE_LOCATION)))
                {
                    settings.setValue(SETTING_LAST_SAVE_IMAGE_LOCATION, oldSettings.getValue(SETTING_LAST_SAVE_IMAGE_LOCATION));
                }
            }
        }

        // This is no longer in the settings file, but I'm keeping it here because
        // I can't think of a better place to put it yet.
        public String updateLocation
        {
            get
            {
                return UPDATE_URL;
            }
        }

        public bool backgroundColorEnabled
        {
            get
            {
                return (settings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_ENABLED) == VALUE_YES);
            }
            set
            {
                String output = VALUE_NO;
                if (value)
                {
                    output = VALUE_YES;
                }
                settings.setValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_ENABLED, output);
            }
        }

        public System.Drawing.Color backgroundColor
        {
            get
            {
                if (backgroundColorEnabled)
                {
                    if (frmMain.checkColorFromStrings(settings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_RED),
                                              settings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_GREEN),
                                              settings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_BLUE)))
                    {
                        return frmMain.makeColorFromStrings(settings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_RED),
                                                    settings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_GREEN),
                                                    settings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_BLUE));
                    }
                    else
                    {
                        return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);
                    }
                }
                else
                {
                    return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);
                }
            }
            set
            {
                settings.setValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_RED, value.R.ToString());
                settings.setValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_GREEN, value.G.ToString());
                settings.setValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_BLUE, value.B.ToString());
            }
        }

        public bool filterStiches
        {
            get
            {
                return (settings.getValue(SETTING_FILTER_STITCHES) == VALUE_TRUE);
            }
            set
            {
                String output = VALUE_FALSE;
                if (value)
                {
                    output = VALUE_TRUE;
                }
                settings.setValue(SETTING_FILTER_STITCHES, output);
            }
        }

        public double threadThickness
        {
            get
            {
                double thickness = 5;
                if (settings.getValue(SETTING_THREAD_THICKNESS) != null)
                {
                    if (Double.TryParse(settings.getValue(SETTING_THREAD_THICKNESS), out thickness))
                    {
                        if (thickness < 1)
                        {
                            thickness = 1;
                        }
                    }
                }
                return thickness;
            }
            set
            {
                settings.setValue(SETTING_THREAD_THICKNESS, value.ToString());
            }
        }

        public double filterStitchesThreshold
        {
            get
            {
                int threshold = 120;
                if (settings.getValue(SETTING_FILTER_STITCHES_THRESHOLD) != null)
                {
                    if (Int32.TryParse(settings.getValue(SETTING_FILTER_STITCHES_THRESHOLD), out threshold))
                    {
                        if (threshold < 10)
                        {
                            threshold = 10;
                        }
                    }
                }
                return threshold;
            }
            set
            {
                settings.setValue(SETTING_FILTER_STITCHES_THRESHOLD, value.ToString());
            }
        }

        public String lastOpenFileFolder
        {
            get
            {
                return settings.getValue(SETTING_LAST_OPEN_FILE_FOLDER);
            }
            set
            {
                settings.setValue(SETTING_LAST_OPEN_FILE_FOLDER, value);
            }
        }

        public void save()
        {
            settings.save();
        }

        public String lastSaveImageLocation
        {
            get
            {
                return settings.getValue(SETTING_LAST_SAVE_IMAGE_LOCATION);
            }
            set
            {
                settings.setValue(SETTING_LAST_SAVE_IMAGE_LOCATION, value);
            }
        }

        public int windowWidth
        {
            get
	        {
                Int32 retval;
                string temp = settings.getValue(SETTING_WINDOW_WIDTH);
                if(!Int32.TryParse(temp, out retval))
                {
                    retval = 300;
                }
		        return retval;
	        }
	        set
	        {
	            settings.setValue(SETTING_WINDOW_WIDTH, value.ToString());
	        }
        }

        public int windowHeight
        {
            get
            {
                Int32 retval;
                string temp = settings.getValue(SETTING_WINDOW_HEIGHT);
                if (!Int32.TryParse(temp, out retval))
                {
                    retval = 300;
                }
                return retval;
            }
            set
            {
                settings.setValue(SETTING_WINDOW_HEIGHT, value.ToString());
            }
        }
    }
}
