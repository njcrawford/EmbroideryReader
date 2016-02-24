using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace embroideryReader
{
    class EmbroideryReaderUpdates
    {
        public enum versionImportance { unknownOrUnavailable, bugfix, feature, critical, };

        System.Net.WebClient downloader = new System.Net.WebClient();
        NJCrawford.StringIniReader updateInfo;
        System.Uri updateURI;
        string lastError = "";
        private bool isInfoReady = false;
        private BackgroundWorker internalThread;
        private string updateURL;

        private const string VERSION = "version";
        private const string RELEASE_DATE = "release date";
        private const string MORE_INFO_URL = "more info url";

        public const string SETTINGS_SECTION_NAME = "update info";
        public const string SETTINGS_NEWER_VERSION = "newer version available";
        public const string SETTINGS_MORE_INFO = "more info url";
        public const string SETTINGS_RELEASE_DATE = "release date";
        public const string SETTINGS_LAST_CHECK = "last update check";

        private Mutex infoReadyMutex;

        private Version callerVersion;
        private string callerName;

        private NJCrawford.IniFile appSettings;
        public delegate void callbackFunctionType();
        private callbackFunctionType callbackFunction;

        public EmbroideryReaderUpdates(string updateLocation, NJCrawford.IniFile settingsFile, callbackFunctionType callback)
        {
            callbackFunction = callback;
            appSettings = settingsFile;
            infoReadyMutex = new Mutex();
            updateURL = updateLocation;
            callerVersion = Assembly.GetCallingAssembly().GetName().Version;
            callerName = Assembly.GetCallingAssembly().GetName().Name;
            //internalThread = new Thread(new ThreadStart(internalThreadFunction));
            internalThread = new BackgroundWorker();
            internalThread.DoWork += new DoWorkEventHandler(internalThread_DoWork);
            internalThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(internalThread_RunWorkerCompleted);
            internalThread.RunWorkerAsync();
        }

        void internalThread_DoWork(object sender, DoWorkEventArgs e)
        {
            string downloadString = "";
            updateURI = new Uri(updateURL);
            downloader.Headers.Add("user-agent", "UpdateCheck/" + Assembly.GetExecutingAssembly().GetName().Version + " " + callerName + "/" + callerVersion);
            try
            {
                downloadString = downloader.DownloadString(updateURI);
                updateInfo = new NJCrawford.StringIniReader(downloadString);
            }
            catch (Exception ex)
            {
                //don't complain, just remember it for later
                lastError = ex.Message;
            }

            // I suspect that this will prevent a race condition
            infoReadyMutex.WaitOne();
            isInfoReady = true;
            infoReadyMutex.ReleaseMutex();
        }

        void internalThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (appSettings != null)
            {
                if (IsInfoValid())
                {
                    appSettings.setValue(SETTINGS_SECTION_NAME, SETTINGS_NEWER_VERSION, VersionAvailable().ToString());
                    appSettings.setValue(SETTINGS_SECTION_NAME, SETTINGS_MORE_INFO, getMoreInfoURL());
                    appSettings.setValue(SETTINGS_SECTION_NAME, SETTINGS_RELEASE_DATE, getReleaseDate().ToShortDateString());
                }
                appSettings.setValue(SETTINGS_SECTION_NAME, SETTINGS_LAST_CHECK, DateTime.Now.ToShortDateString());
            }
            if (callbackFunction != null)
            {
                callbackFunction();
            }
        }

        /*<summary>Returns null if not available for any reason.</summary>*/
        public Version VersionAvailable()
        {
            Version retval = null;
            string versionString = updateInfo.getValue(VERSION, "");
            if (!String.IsNullOrEmpty(versionString))
            {
                try
                {
                    retval = new Version(versionString);
                }
                catch (FormatException fex)
                {
                    lastError = fex.Message;
                }
            }
            return retval;
        }

        public bool IsUpdateAvailable()
        {
            bool retval = false;
            Version currentVersion = Assembly.GetCallingAssembly().GetName().Version;

            if (VersionAvailable() != null)
            {
                retval = (VersionAvailable() > currentVersion);
            }

            return retval;
        }

        public string GetLastError()
        {
            return lastError;
        }

        public void ResetLastError()
        {
            lastError = "";
        }

        /*<summary>Returns the date and possibly time
         * the latest update was released. Returns DateTime.MinValue
         * if not available.</summary>
         */
        public DateTime getReleaseDate()
        {
            DateTime retval = DateTime.MinValue;
            try
            {
                retval = Convert.ToDateTime(updateInfo.getValue(RELEASE_DATE, ""));
            }
            catch (FormatException fx)
            {
                lastError = fx.Message;
            }
            return retval;
        }

        // gets 'more info url'
        public string getMoreInfoURL()
        {
            return updateInfo.getValue(MORE_INFO_URL, "");
        }

        public bool IsInfoReady()
        {
            bool retval;

            // I suspect that this will prevent a race condition
            infoReadyMutex.WaitOne();
            retval = isInfoReady;
            infoReadyMutex.ReleaseMutex();

            return retval;
        }

        public void waitForInfo()
        {
            while (!isInfoReady)
            {
                System.Threading.Thread.Sleep(100);
                Application.DoEvents();
            }
        }

        public bool IsInfoValid()
        {
            string versionString = updateInfo.getValue(VERSION, "");
            if (String.IsNullOrEmpty(versionString))
            {
                lastError = "Missing version in update info";
                return false;
            }
            else
            {
                // Make sure the version is valid
                // Check for 4 int16s seperated by dots
                string[] parts = versionString.Split('.');
                bool versionOK = true;
                if (parts.Length == 4)
                {
                    Int16 junk;
                    for (int i = 0; i < 4; i++)
                    {
                        if (!Int16.TryParse(parts[i], out junk))
                        {
                            versionOK = false;
                        }
                    }
                }
                else
                {
                    versionOK = false;
                }

                if (!versionOK)
                {
                    lastError = "Invalid version format";
                    return false;
                }
            }

            string releaseDateString = updateInfo.getValue(RELEASE_DATE, "");
            if (String.IsNullOrEmpty(releaseDateString))
            {
                lastError = "Missing release date in update info";
                return false;
            }
            else
            {
                // Make sure the date is valid
                DateTime junk;
                if (!DateTime.TryParse(releaseDateString, out junk))
                {
                    lastError = "Invalid release date";
                    return false;
                }
            }

            string moreInfoURL = updateInfo.getValue(MORE_INFO_URL, "");
            if (String.IsNullOrEmpty(moreInfoURL))
            {
                lastError = "Missing 'more info url' in update info";
                return false;
            }
            else
            {
                // Make sure url is valid
                if(!Uri.IsWellFormedUriString(moreInfoURL, UriKind.Absolute))
                {
                    lastError = "Invalid 'more info url'";
                    return false;
                }
            }

            return true;
        }
    }
}
