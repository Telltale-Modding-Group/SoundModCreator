using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace SoundModCreator
{
    /// <summary>
    /// Main class pertaining to the application settings.
    /// </summary>
    public class AppSettings
    {
        //public
        public AppSettingsFile appSettingsFile;

        //private
        private IOManagement ioManagement;
        private static string systemDocumentsPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string configFile_filename = "SoundModCreator_Config.json";
        private static string configFile_directory_location = systemDocumentsPath + "/SoundModCreator/";
        private static string configFile_file_location = configFile_directory_location + configFile_filename;

        /// <summary>
        /// Application Settings Class, creates an AppSettings object. This is called on application startup.
        /// <para>If there is an existing config file, it will parse the data from it.</para>
        /// <para>If there is not an existing config file, or a TelltaleModLauncher directory, create a new one.</para>
        /// </summary>
        public AppSettings(IOManagement ioManagement)
        {
            this.ioManagement = ioManagement;

            if (File.Exists(configFile_file_location))
            {
                //ReadConfigFile();
            }
            else
            {
                appSettingsFile = new AppSettingsFile();

                if (!Directory.Exists(configFile_directory_location))
                    ioManagement.CreateDirectory(configFile_directory_location);

                WriteToFile();
            }
        }

        /// <summary>
        /// Reads and parses the data from the app config file.
        /// </summary>
        public void ReadConfigFile()
        {
            appSettingsFile = new AppSettingsFile();

            //read the data from the config file
            string jsonText = File.ReadAllText(configFile_file_location);

            //parse the data into a json array
            JObject AppSettingsFile_fromJson = JObject.Parse(jsonText);

            //loop through each property to get the data
            foreach (JProperty property in AppSettingsFile_fromJson.Properties())
            {
                string name = property.Name;

                if (name.Equals(nameof(appSettingsFile.Location_Ttarchext)))
                    appSettingsFile.Location_Ttarchext = (string)property.Value;

                if (name.Equals(nameof(appSettingsFile.UI_LightMode)))
                    appSettingsFile.UI_LightMode = (bool)property.Value;

                //if the property is a mod files array, parse the given files to a list
                if (name.Equals(nameof(appSettingsFile.RecentProjectFiles)))
                {
                    JArray recentProjFileArray = (JArray)AppSettingsFile_fromJson[nameof(appSettingsFile.RecentProjectFiles)];

                    List<string> parsed_RecentProjectFiles = new List<string>();

                    foreach (JValue projFile in recentProjFileArray)
                    {
                        parsed_RecentProjectFiles.Add((string)projFile.Value);
                    }

                    appSettingsFile.RecentProjectFiles = parsed_RecentProjectFiles;
                }
            }
        }

        /// <summary>
        /// Writes existing values of the App Settings objects into the config file.
        /// </summary>
        public void WriteToFile()
        {
            if (File.Exists(configFile_file_location))
                ioManagement.DeleteFile(configFile_file_location);

            //open a stream writer to create the text file and write to it
            using (StreamWriter file = File.CreateText(configFile_file_location))
            {
                //get our json seralizer
                JsonSerializer serializer = new JsonSerializer();

                //seralize the data and write it to the configruation file
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, appSettingsFile);
            }
        }

        /// <summary>
        /// Updates the changes to the app config file by replacing it and writing a new one. (there is a better way of doing it, but this works fine)
        /// </summary>
        public void UpdateChangesToFile()
        {
            ioManagement.DeleteFile(configFile_file_location);
            WriteToFile();
        }

        /// <summary>
        /// Checks the current selected game version values if the following values are assigned/exist.
        /// <para>Game_LocationExe, Game_Location, and Game_Location_Mods</para>
        /// <para>returns false if one or all of these values aren't assigned/exist</para>
        /// </summary>
        /// <returns></returns>
        public bool IsApplicationSetupAndValid()
        {
            if (File.Exists(appSettingsFile.Location_Ttarchext) == false)
                return false;

            return true;
        }

        public bool Get_AppSettings_LightMode()
        {
            return appSettingsFile.UI_LightMode;
        }
        //---------------- GETTERS END ----------------
        //---------------- MODIFIERS ----------------
        public void Set_Current_AppSettings_ttarchextLocation(string location)
        {
            appSettingsFile.Location_Ttarchext = location;
        }

        public void Set_Current_AppSettings_UI_LightMode(bool state)
        {
            appSettingsFile.UI_LightMode = state;
        }
        //---------------- MODIFIERS END ----------------
    }
}
