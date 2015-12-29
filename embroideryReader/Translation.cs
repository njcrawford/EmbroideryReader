/*
Embroidery Reader - an application to view .pes embroidery designs

Copyright (C) 2015 Nathan Crawford
 
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
using System.Linq;
using System.Text;
using NJCrawford;

namespace embroideryReader
{
    public class Translation
    {
        private const string TRANSLATIONS_FOLDER = "translations";
        private const string TRANSLATION_FILE_EXT = ".ini";
        private const string DEFAULT_TRANSLATION_NAME = "EN-US";

        // String IDs
        public enum StringID {
            TRANSLATION_DISPLAY_NAME,

            UNSUPPORTED_FORMAT,
            COLOR_WARNING,
            ERROR_FILE,
            CORRUPT_FILE,

            // File type descriptions
            FILE_TYPE_PES,
            FILE_TYPE_ALL,
            FILE_TYPE_BMP,
            FILE_TYPE_PNG,
            FILE_TYPE_JPG,
            FILE_TYPE_GIF,
            FILE_TYPE_TIFF,

            ABOUT_MESSAGE,
            ERROR_UPDATE,
            VERSION,
            ERROR_WEBPAGE,
            NO_UPDATE,
            ERROR_DEBUG,
            NO_DESIGN,
            UNSUPPORTED_CLASS,
            IMAGE_SAVED,

            // Menu strings
            MENU_FILE,
            MENU_OPEN,
            MENU_SAVE_IMAGE,
            MENU_PRINT,
            MENU_PRINT_PREVIEW,
            MENU_EXIT,
            MENU_EDIT,
            MENU_COPY,
            MENU_PREFS,
            MENU_VIEW,
            ROTATE_LEFT,
            ROTATE_RIGHT,
            MENU_RESET,
            MENU_SCALE_ZOOM,
            MENU_FIT_TO_WINDOW,
            MENU_HELP,
            CHECK_UPDATE,
            SAVE_DEBUG,
            SHOW_DEBUG,
            MENU_ABOUT,

            PICK_COLOR,
            BACKGROUND_COLOR,
            RESET_COLOR,
            CANCEL,
            OK,
            THREAD_THICKNESS,
            PIXELS,
            BACKGROUND,
            STITCH_DRAW,
            REMOVE_UGLY_STITCHES,
            UGLY_STITCH_LENGTH,
            SETTINGS,
            LATEST_VERSION,
            NEW_VERSION_MESSAGE,
            NEW_VERSION_QUESTION,
            NEW_VERSION_TITLE,
            DEBUG_INFO_SAVED,
            ENABLE_TRANSPARENCY_GRID,
            LANGUAGE,
            GRID_SIZE,
            TRANSLATION_INCOMPLETE,

            // This must be last. Used for checking completeness of translation files.
            TOTAL_COUNT,
        };

        IniFile translationFile;
        IniFile defaultFile;

        public Translation(String name)
        {
            Load(name);
        }

        // Returns the names of available translations
        // The first value of each tubple is the display name, the second value
        // is the file name that must be passed to the open function.
        public List<Tuple<String, String>> GetAvailableTranslations()
        {
            List<Tuple<String, String>> retval = new List<Tuple<String, String>>();
            foreach (String file in System.IO.Directory.EnumerateFiles(
                System.IO.Path.Combine(Environment.CurrentDirectory, TRANSLATIONS_FOLDER),
                "*" + TRANSLATION_FILE_EXT,
                System.IO.SearchOption.TopDirectoryOnly))
            {
                IniFile tempFile = new IniFile(file);

                retval.Add(new Tuple<String, String>(tempFile.getValue(StringID.TRANSLATION_DISPLAY_NAME.ToString(), ""), System.IO.Path.GetFileNameWithoutExtension(file)));
            }
            return retval;
        }

        // Load a translation file
        // Names are just the file name without the extension
        public void Load(String translationName)
        {
            string exePath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
            String translationPath = System.IO.Path.Combine(exePath, System.IO.Path.Combine(TRANSLATIONS_FOLDER, translationName + TRANSLATION_FILE_EXT));
            if (System.IO.File.Exists(translationPath))
            {
                translationFile = new IniFile(translationPath);
            }
            else
            {
                translationFile = new IniFile(System.IO.Path.Combine(exePath, System.IO.Path.Combine(TRANSLATIONS_FOLDER, DEFAULT_TRANSLATION_NAME + TRANSLATION_FILE_EXT)));
            }
        }

        // Returns the translated string, or a string representation of the
        // string ID if the translation isn't available.
        public String GetTranslatedString(StringID sid)
        {
            string retval;
            retval = translationFile.getValue(sid.ToString(), "");
            
            // Check the default translation if string is not found in the loaded translation
            if (String.IsNullOrEmpty(retval))
            {
                retval = defaultFile.getValue(sid.ToString(), "");
            }

            // If it's not found in the default, return a placeholder string
            if (String.IsNullOrEmpty(retval))
            {
                retval = "%" + sid.ToString() + "%";
            }

            return retval;
        }

        // Returns true if the loaded translation file contains all expected 
        // string IDs, or false if not.
        public bool IsComplete()
        {
            bool retval = true;

            for (StringID sid = (StringID)0; sid < StringID.TOTAL_COUNT; sid++)
            {
                if (translationFile.getValue(sid.ToString(), (String)null) == null)
                {
                    retval = false;
                    break;
                }
            }

            return retval;
        }
    }
}
