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
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace embroideryReader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Handle unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException +=
                new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            // Normal program.cs stuff
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }

        // Functions to deal with unhandled exceptions
        // Based on code from http://stevenbenner.com/2010/01/reporting-of-unhandled-exceptions-in-your-distributable-net-application/
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            reportError((Exception)e.ExceptionObject);
        }

        public static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            reportError(e.Exception);
        }

        public static void reportError(Exception ex)
        {
            try
            {
                if (MessageBox.Show(
                    "Embroidery Reader has encountered a error.\n\n" +
                        "Click Yes to report this error via the contact form on my website.\n" +
                        "Click No if you'd rather not report the error.\n" +
                        "Please consider submitting the report to help find and fix this issue.\n\n" +
                        "Error:\n" +
                        ex.Message + "\n" +
                        ex.StackTrace,
                    "Report Application Error?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Stop) == DialogResult.Yes)
                {
                    // Base url
                    string url = "http://www.njcrawford.com/contact/error-reports/?1subject=Automated%20Error%20Report&1message=";

                    // Add error message to it
                    Assembly caller = Assembly.GetEntryAssembly();
                    string errorText = "Description: (If possible, please describe what happened right before the error)\n\n" +
                        "Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n" +
                        "Program: " + caller.FullName + "\n" +
                        "OS: " + Environment.OSVersion.ToString() + "\n" +
                        "OS Culture: " + System.Globalization.CultureInfo.CurrentCulture.Name + "\n" +
                        "Framework: " + Environment.Version + "\n\n" +
                        "Error:\n" +
                        ex.GetType().ToString() + " (" + ex.Message + ")\n" +
                        ex.StackTrace;
                    // URL encode message
                    url += System.Uri.EscapeDataString(errorText);

                    // Open URL in browser
                    System.Diagnostics.Process.Start(url);
                }
            }
            finally
            {
                Application.Exit();
            }
        }
    }
}
