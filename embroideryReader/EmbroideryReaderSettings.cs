/*
Embroidery Reader - an application to view .pes embroidery designs

Copyright (C) 2010 Nathan Crawford
 
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

        private const String VALUE_YES = "yes";
        private const String VALUE_NO = "no";

        private const String VALUE_TRUE = "true";
        private const String VALUE_FALSE = "false";

        private const String UPDATE_URL = "http://www.njcrawford.com/updates/embroidery-reader.ini";
        private const String SETTINGS_FILENAME = "embroideryreader.ini";

        public EmbroideryReaderSettings()
        {
            settings = new NJCrawford.IniFile(SETTINGS_FILENAME);
            if (!String.IsNullOrEmpty(updateLocation))
            {
                settings.setValue(SETTING_UPDATE_LOCATION, null);
            }
        }

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
    }
}
