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
        private const string DEFAULT_TRANSLATION_NAME = "English (EN-US)";

        // String IDs
        public enum StringID {
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
        // Names are just the file name without the extension
        public List<String> GetAvailableTranslations()
        {
            List<String> retval = new List<string>();
            foreach (String file in System.IO.Directory.EnumerateFiles(
                System.IO.Path.Combine(Environment.CurrentDirectory, TRANSLATIONS_FOLDER),
                "*" + TRANSLATION_FILE_EXT,
                System.IO.SearchOption.TopDirectoryOnly))
            {
                retval.Add(System.IO.Path.GetFileNameWithoutExtension(file));
            }
            return retval;
        }

        // Load a translation file
        // Names are just the file name without the extension
        public void Load(String translationName)
        {
            translationFile = new IniFile(System.IO.Path.Combine(TRANSLATIONS_FOLDER, translationName + TRANSLATION_FILE_EXT));
            defaultFile = new IniFile(System.IO.Path.Combine(TRANSLATIONS_FOLDER, DEFAULT_TRANSLATION_NAME + TRANSLATION_FILE_EXT));
        }

        // Returns the translated string, or a string representation of the
        // string ID if the translation isn't available.
        public String GetTranslatedString(StringID sid)
        {
            string retval;
            retval = translationFile.getValue(sid.ToString());
            
            // Check the default translation if string is not found in the loaded translation
            if (retval == null)
            {
                retval = defaultFile.getValue(sid.ToString());
            }

            // If it's not found in the default, return a placeholder string
            if (retval == null)
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
                if (translationFile.getValue(sid.ToString()) == null)
                {
                    retval = false;
                    break;
                }
            }

            return retval;
        }
    }
}
