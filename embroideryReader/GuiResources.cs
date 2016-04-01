using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace embroideryReader
{
    class GuiResources
    {
        /// uiFlags: 0 - Count of GDI objects
        /// uiFlags: 1 - Count of USER objects
        /// - Win32 GDI objects (pens, brushes, fonts, palettes, regions, device contexts, bitmap headers)
        /// - Win32 USER objects:
        ///      - WIN32 resources (accelerator tables, bitmap resources, dialog box templates, font resources, menu resources, raw data resources, string table entries, message table entries, cursors/icons)
        /// - Other USER objects (windows, menus)
        ///
        [DllImport("User32")]
        extern public static int GetGuiResources(IntPtr hProcess, int uiFlags);

        public static int GetGuiResourcesGDICount()
        {
            return GetGuiResources(Process.GetCurrentProcess().Handle, 0);
        }

        public static int GetGuiResourcesUserCount()
        {
            return GetGuiResources(Process.GetCurrentProcess().Handle, 1);
        }
    }
}
