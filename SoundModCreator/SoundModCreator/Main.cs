using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using SoundModCreator.FileTree;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace SoundModCreator
{
    public class Main
    {
        public Item selectedItem;
        public AudioPlayer audioPlayer;

        private MainWindow mainWindow;
        private ProjectFile projectFile;
        private IOManagement ioManagement;

        public Main(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            projectFile = new ProjectFile();
            ioManagement = new IOManagement();
            audioPlayer = new AudioPlayer(mainWindow);
        }

        public void ProjectView_DoubleClick(object selectedValue)
        {
            selectedItem = (Item)selectedValue;

            audioPlayer.LoadAudio(selectedItem.Path);
        }

        public void Project_SaveProject()
        {
            if (File.Exists(projectFile.Project_FilePath))
            {
                Project_SaveProjectAs();
                return;
            }

            UpdateProjectFileWithChanges();
        }

        public void Project_SaveProjectAs()
        {
            string newPath = "";
            string extension = ".soundmodproj";

            ioManagement.GetFilePath(ref newPath, "Save your");

            if (string.IsNullOrEmpty(newPath))
                return;

            string newPath_fullPath = Path.GetFullPath(newPath);
            string newPath_extension = Path.GetExtension(newPath);
            string newPath_noExt = newPath_fullPath.Remove(newPath_fullPath.Length - newPath_extension.Length, newPath_extension.Length);
            string finalPath = newPath_noExt + extension;

            projectFile.Project_FilePath = finalPath;
        }

        public ProjectFile Get_NewProjectFile(string filePath)
        {
            //create a new mod object
            ProjectFile newProjectFile = new ProjectFile();

            //read the data from the config file
            string jsonText = File.ReadAllText(filePath);

            //parse the data into a json array
            JObject obj = JObject.Parse(jsonText);

            //loop through each property to get the data
            foreach (JProperty property in obj.Properties())
            {
                string name = property.Name; //get the name of the property from the json object

                if (name.Equals(nameof(projectFile.Project_Directory)))
                    projectFile.Project_Directory = (string)property.Value;

                if (name.Equals(nameof(projectFile.Project_FilePath)))
                    projectFile.Project_FilePath = (string)property.Value;

                if (name.Equals(nameof(projectFile.Project_GameVersion)))
                    projectFile.Project_GameVersion = (string)property.Value;

                if (name.Equals(nameof(projectFile.Project_ModVersion)))
                    projectFile.Project_ModVersion = (string)property.Value;

                if (name.Equals(nameof(projectFile.Project_FileTree)))
                {
                    JArray fileArray = (JArray)obj[nameof(projectFile.Project_FileTree)];
                    List<Item> parsed_FileTree = new List<Item>();

                    foreach (JValue item in fileArray)
                    {
                        parsed_FileTree.Add((Item)item.Value);
                    }

                    newProjectFile.Project_FileTree = parsed_FileTree;
                }
            }

            return newProjectFile;
        }

        public void UpdateProjectFileWithChanges()
        {
            string path = projectFile.Project_FilePath;

            //open a stream writer to create the text file and write to it
            using (StreamWriter file = File.CreateText(path))
            {
                //get our json seralizer
                JsonSerializer serializer = new JsonSerializer();

                //seralize the data and write it to the configruation file
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, projectFile);
            }
        }

        public void MonitorFolder()
        {
            string path = projectFile.Project_FilePath;

            FileSystemWatcher ProjectFolderWatcher = new FileSystemWatcher(path);
            ProjectFolderWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            ProjectFolderWatcher.Changed += ProjectFolderWatcher_Changed;
            ProjectFolderWatcher.IncludeSubdirectories = true;
            ProjectFolderWatcher.EnableRaisingEvents = true;
        }

        private void ProjectFolderWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            string path = projectFile.Project_FilePath;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                GetFilePathsItems(path);
            }));
        }


        public List<Item> GetFilePathsItems(string path)
        {
            var items = new List<Item>();

            var dirInfo = new DirectoryInfo(path);

            foreach (var directory in dirInfo.GetDirectories())
            {
                var item = new DirectoryItem
                {
                    Name = directory.Name,
                    Path = directory.FullName,
                    Items = GetFilePathsItems(directory.FullName)
                };

                items.Add(item);
            }

            foreach (var file in dirInfo.GetFiles())
            {
                var item = new FileItem
                {
                    Name = file.Name,
                    Path = file.FullName
                };

                items.Add(item);
            }

            return items;
        }

        public void New_ProjectFile()
        {

        }

        public void Open_ProjectFile()
        {
            string newPath = "";
            string extension = "Sound Mod Project (*.soundmodproj)|*..soundmodproj";

            ioManagement.GetFilePath(ref newPath, extension, "Select your Project Directory");

            if (string.IsNullOrEmpty(newPath))
                return;

            projectFile.Project_Directory = newPath;
        }

        public void Set_ProjectFile_Directory()
        {
            string newPath = "";

            ioManagement.GetFolderPath(ref newPath, "Select your Project Directory");

            if (string.IsNullOrEmpty(newPath))
                return;

            projectFile.Project_Directory = newPath;
        }

        public void Set_ProjectFile_GameVersion(GameVersion gameVersion)
        {
            projectFile.Project_GameVersion = Enum.GetName(typeof(GameVersion), gameVersion);
        }

        public void Set_ProjectFile_ModVersion(string modVersion)
        {
            projectFile.Project_ModVersion = modVersion;
        }
    }
}
