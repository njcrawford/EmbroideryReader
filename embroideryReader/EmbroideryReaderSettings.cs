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

        private const String SETTING_WINDOW_MAXIMIZED = "window maximized";
        private const String SETTING_WINDOW_WIDTH = "window width";
        private const String SETTING_WINDOW_HEIGHT = "window height";
        private const String SETTING_AUTOSCALE_DESIGN = "auto scale design";

        private const String SECTION_TRANSPARENCY_GRID = "transparency grid";
        private const String SETTING_TRANSPARENCY_GRID_ENABLE = "enabled";
        private const String SETTING_TRANSPARENCY_GRID_SIZE = "size";
        private const String SETTING_TRANSPARENCY_GRID_COLOR_RED = "red";
        private const String SETTING_TRANSPARENCY_GRID_COLOR_GREEN = "green";
        private const String SETTING_TRANSPARENCY_GRID_COLOR_BLUE = "blue";

        private const String SETTING_TRANSLATION = "translation";

        private const String UPDATE_URL = "http://www.njcrawford.com/updates/embroidery-reader.ini";
        private const String SETTINGS_FILENAME = "embroideryreader.ini";
        private const String SETTINGS_PATH_COMPANY = "NJCrawford Software";
        private const String SETTINGS_PATH_APP_NAME = "Embroidery Reader";

        private const int DEFAULT_WINDOW_SIZE = 300;
        private const int MINIMUM_WINDOW_SIZE = 150;

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

                if (oldSettings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_ENABLED, -1) != -1)
                {
                    settings.setValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_ENABLED, oldSettings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_ENABLED, ""));
                }

                if (oldSettings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_RED, -1) != -1)
                {
                    settings.setValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_RED, oldSettings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_RED, ""));
                }

                if (oldSettings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_GREEN, -1) != -1)
                {
                    settings.setValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_GREEN, oldSettings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_GREEN, ""));
                }

                if (oldSettings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_BLUE, -1) != -1)
                {
                    settings.setValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_BLUE, oldSettings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_BLUE, ""));
                }

                if (oldSettings.getValue(SETTING_FILTER_STITCHES, false))
                {
                    settings.setValue(SETTING_FILTER_STITCHES, oldSettings.getValue(SETTING_FILTER_STITCHES, ""));
                }

                if (oldSettings.getValue(SETTING_FILTER_STITCHES_THRESHOLD, 0.0) > 0.0)
                {
                    settings.setValue(SETTING_FILTER_STITCHES_THRESHOLD, oldSettings.getValue(SETTING_FILTER_STITCHES_THRESHOLD, ""));
                }

                if (oldSettings.getValue(SETTING_THREAD_THICKNESS, 0.0) > 0.0)
                {
                    settings.setValue(SETTING_THREAD_THICKNESS, oldSettings.getValue(SETTING_THREAD_THICKNESS, ""));
                }

                if (!String.IsNullOrEmpty(oldSettings.getValue(SETTING_LAST_OPEN_FILE_FOLDER, "")))
                {
                    settings.setValue(SETTING_LAST_OPEN_FILE_FOLDER, oldSettings.getValue(SETTING_LAST_OPEN_FILE_FOLDER, ""));
                }

                if (!String.IsNullOrEmpty(oldSettings.getValue(SETTING_LAST_SAVE_IMAGE_LOCATION, "")))
                {
                    settings.setValue(SETTING_LAST_SAVE_IMAGE_LOCATION, oldSettings.getValue(SETTING_LAST_SAVE_IMAGE_LOCATION, ""));
                }
            }

            // Default to transparency grid enabled
            if (String.IsNullOrWhiteSpace(settings.getValue(SECTION_TRANSPARENCY_GRID, SETTING_TRANSPARENCY_GRID_ENABLE, "")))
            {
                settings.setValue(SECTION_TRANSPARENCY_GRID, SETTING_TRANSPARENCY_GRID_ENABLE, true);
            }

            // Update deprecated settings
            if (settings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_ENABLED, "") == "yes")
            {
                settings.setValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_ENABLED, true);
            }
            else
            {
                settings.setValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_ENABLED, false);
            }

            // Default language to english
            if (String.IsNullOrWhiteSpace(settings.getValue(SETTING_TRANSLATION, "")))
            {
                settings.setValue(SETTING_TRANSLATION, "English (EN-US)");
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
                return settings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_ENABLED, false);
            }
            set
            {
                settings.setValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_ENABLED, value);
            }
        }

        public System.Drawing.Color backgroundColor
        {
            get
            {
                if (!backgroundColorEnabled)
                {
                    return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);
                }

                try
                {
                    return System.Drawing.Color.FromArgb(settings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_RED, -1),
                                              settings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_GREEN, -1),
                                              settings.getValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_BLUE, -1));
                }
                catch(ArgumentException argEx)
                {
                    return System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);
                }
            }
            set
            {
                settings.setValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_RED, value.R);
                settings.setValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_GREEN, value.G);
                settings.setValue(SECTION_BACKGROUND_COLOR, SETTING_BACKGROUND_COLOR_BLUE, value.B);
            }
        }

        public bool filterStiches
        {
            get
            {
                return (settings.getValue(SETTING_FILTER_STITCHES, false));
            }
            set
            {
                settings.setValue(SETTING_FILTER_STITCHES, value);
            }
        }

        public float threadThickness
        {
            get
            {
                float thickness = settings.getValue(SETTING_THREAD_THICKNESS, 5.0f);
                if (thickness < 1.0f)
                {
                    thickness = 1.0f;
                }
                return thickness;
            }
            set
            {
                settings.setValue(SETTING_THREAD_THICKNESS, value);
            }
        }

        public float filterStitchesThreshold
        {
            get
            {
                float threshold = settings.getValue(SETTING_FILTER_STITCHES_THRESHOLD, 64.0f);
                if (threshold < 10.0f)
                {
                    threshold = 10.0f;
                }
                return threshold;
            }
            set
            {
                settings.setValue(SETTING_FILTER_STITCHES_THRESHOLD, value);
            }
        }

        public String lastOpenFileFolder
        {
            get
            {
                return settings.getValue(SETTING_LAST_OPEN_FILE_FOLDER, "");
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
                return settings.getValue(SETTING_LAST_SAVE_IMAGE_LOCATION, "");
            }
            set
            {
                settings.setValue(SETTING_LAST_SAVE_IMAGE_LOCATION, value);
            }
        }

        public bool windowMaximized
        {
            get
            {
                return settings.getValue(SETTING_WINDOW_MAXIMIZED, false);
            }
            set
            {
                settings.setValue(SETTING_WINDOW_MAXIMIZED, value);
            }
        }

        public int windowWidth
        {
            get
	        {
                // Return the stored window size, but no smaller than MINUMUM_WINDOW_SIZE
                return Math.Max(settings.getValue(SETTING_WINDOW_WIDTH, DEFAULT_WINDOW_SIZE), MINIMUM_WINDOW_SIZE);
	        }
	        set
	        {
	            settings.setValue(SETTING_WINDOW_WIDTH, value);
	        }
        }

        public int windowHeight
        {
            get
            {
                // Return the stored window size, but no smaller than MINUMUM_WINDOW_SIZE
                return Math.Max(settings.getValue(SETTING_WINDOW_HEIGHT, DEFAULT_WINDOW_SIZE), MINIMUM_WINDOW_SIZE);
            }
            set
            {
                settings.setValue(SETTING_WINDOW_HEIGHT, value);
            }
        }

        public bool AutoScaleDesign
        {
            get
            {
                return settings.getValue(SETTING_AUTOSCALE_DESIGN, true);
            }
            set
            {
                settings.setValue(SETTING_AUTOSCALE_DESIGN, value);
            }
        }

        public bool transparencyGridEnabled
        {
            get
            {
                return (settings.getValue(SECTION_TRANSPARENCY_GRID, SETTING_TRANSPARENCY_GRID_ENABLE, false));
            }
            set
            {
                settings.setValue(SECTION_TRANSPARENCY_GRID, SETTING_TRANSPARENCY_GRID_ENABLE, value);
            }
        }

        public int transparencyGridSize
        {
            get
            {
                return settings.getValue(SECTION_TRANSPARENCY_GRID, SETTING_TRANSPARENCY_GRID_SIZE, 5);
            }
            set
            {
                settings.setValue(SECTION_TRANSPARENCY_GRID, SETTING_TRANSPARENCY_GRID_SIZE, value);
            }
        }

        public System.Drawing.Color transparencyGridColor
        {
            get
            {
                if (!transparencyGridEnabled)
                {
                    return System.Drawing.Color.LightGray;
                }

                try
                {
                    return System.Drawing.Color.FromArgb(settings.getValue(SECTION_TRANSPARENCY_GRID, SETTING_TRANSPARENCY_GRID_COLOR_RED, -1),
                                            settings.getValue(SECTION_TRANSPARENCY_GRID, SETTING_TRANSPARENCY_GRID_COLOR_GREEN, -1),
                                            settings.getValue(SECTION_TRANSPARENCY_GRID, SETTING_TRANSPARENCY_GRID_COLOR_BLUE, -1));
                }
                catch(ArgumentException argEx)
                {
                    return System.Drawing.Color.LightGray;
                }
            }
            set
            {
                settings.setValue(SECTION_TRANSPARENCY_GRID, SETTING_TRANSPARENCY_GRID_COLOR_RED, value.R);
                settings.setValue(SECTION_TRANSPARENCY_GRID, SETTING_TRANSPARENCY_GRID_COLOR_GREEN, value.G);
                settings.setValue(SECTION_TRANSPARENCY_GRID, SETTING_TRANSPARENCY_GRID_COLOR_BLUE, value.B);
            }
        }

        public String translation
        {
            get
            {
                return settings.getValue(SETTING_TRANSLATION, "");
            }
            set
            {
                settings.setValue(SETTING_TRANSLATION, value);
            }
        }
    }
}
