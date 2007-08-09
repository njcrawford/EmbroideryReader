using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace UpdateTester
{
    public class nc_Update
    {
        System.Net.WebClient downloader = new System.Net.WebClient();
        SettingsTester.nc_Settings updateInfo;
        System.Uri updateURI;
        string lastError = "";

        public nc_Update(string updateLocation)
        {
            if (!Directory.Exists(Path.Combine(SettingsTester.nc_Settings.appPath(), "update")))
            {
                Directory.CreateDirectory(Path.Combine(SettingsTester.nc_Settings.appPath(), "update"));
            }
            try
            {
                updateURI = new Uri(updateLocation);
                downloader.DownloadFile(new Uri(updateURI, "update.ini"), Path.Combine(Path.Combine(SettingsTester.nc_Settings.appPath(), "update"), "update.ini"));
            }
            catch (Exception ex)
            {
                //don't complain
                lastError = ex.Message;
            }
            updateInfo = new SettingsTester.nc_Settings(Path.Combine(Path.Combine(SettingsTester.nc_Settings.appPath(), "update"), "update.ini"));
        }

        public string VersionAvailable()
        {
            if (updateInfo.getValue("version") == null || updateInfo.getValue("version") == "")
            {
                return "0.0.0.0";
            }
            else
            {
                return updateInfo.getValue("version");
            }
        }

        public bool IsUpdateAvailable()
        {
            //Assembly myAsm = Assembly.Load("embroideryReader");
            Assembly myAsm = Assembly.GetCallingAssembly();
            AssemblyName aName = myAsm.GetName();
            //aName.Version.ToString();
            bool isNewerVersion = false;
            char[] sep = { '.' };
            string[] upVersion = VersionAvailable().Split(sep);
            string[] curVersion = aName.Version.ToString().Split(sep);
            for (int i = 0; i < 4; i++)
            {
                if (Convert.ToInt32(upVersion[i]) > Convert.ToInt32(curVersion[i]))
                {
                    isNewerVersion = true;
                    break;
                }
            }
            return isNewerVersion;
        }

        public bool InstallUpdate()
        {
            try
            {
                if (!IsUpdateAvailable())
                {
                    lastError = "No updates available";
                    return false;
                }
                int numFiles = Convert.ToInt32(updateInfo.getValue("files", "number of files"));
                string filename = "";
                if (!Directory.Exists(Path.Combine(SettingsTester.nc_Settings.appPath(), "update")))
                {
                    Directory.CreateDirectory(Path.Combine(SettingsTester.nc_Settings.appPath(), "update"));
                }
                for (int i = 1; i <= numFiles; i++)
                {
                    filename = updateInfo.getValue("files", "file" + i.ToString());
                    downloader.DownloadFile(new Uri(updateURI, filename), Path.Combine(Path.Combine(SettingsTester.nc_Settings.appPath(), "update"), filename));
                }
                if (!System.IO.File.Exists(System.IO.Path.Combine(SettingsTester.nc_Settings.appPath(), "UpdateInstaller.exe")))
                {
                    lastError = "UpdateInstaller.exe not found";
                    return false;
                }
                System.Diagnostics.Process.Start(System.IO.Path.Combine(SettingsTester.nc_Settings.appPath(), "UpdateInstaller.exe"), System.Diagnostics.Process.GetCurrentProcess().Id.ToString());
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                return false;
            }
            return true;
        }

        public string GetLastError()
        {
            return lastError;
        }

        public void ResetLastError()
        {
            lastError = "";
        }
    }
}
