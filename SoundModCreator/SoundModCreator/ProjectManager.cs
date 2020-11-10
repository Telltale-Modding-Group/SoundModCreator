using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Windows;
using SoundModCreator.FileTree;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Newtonsoft.Json.Serialization;


namespace SoundModCreator
{
    public class ProjectManager
    {
        public ProjectFile projectFile;

        private Main main;
        private IOManagement ioManagement;
        private ProjectSettings projectSettings;

        private FileSystemWatcher ProjectFolderWatcher;

        public ProjectManager(IOManagement ioManagement, ProjectSettings projectSettings)
        {
            this.ioManagement = ioManagement;
            this.projectSettings = projectSettings;
        }

        public void Set_MainObject(Main main)
        {
            this.main = main;
        }

        public void Open_Create_NewProjectFileWindow()
        {
            projectSettings.OpenWindow_AsNewProject();
            projectSettings.Show();
            projectSettings.Activate();
            projectSettings.ClearWindow();
        }

        public void Create_NewProject(ProjectFile newProjectFile)
        {
            string newPath = "";
            string extensionFilter = "Sound Mod Project (*.soundmodproj)|*.soundmodproj";

            ioManagement.SaveFilePath(ref newPath, extensionFilter, "Save Your Project");

            if (string.IsNullOrEmpty(newPath))
                return;

            string projectDirectory = Path.GetDirectoryName(newPath);

            newProjectFile.Project_FilePath = newPath;
            SetupProjectFolders(projectDirectory, ref newProjectFile);
            projectFile = newProjectFile;
            projectFile.Project_FileTree = GetFilePathsItems(projectFile.Project_MainDirectory);
            MonitorFolder();
            main.ProjectView_UpdateFileTree(projectFile.Project_FileTree);
            UpdateProjectFileWithChanges();
        }

        public void MonitorFolder()
        {
            if (ProjectFolderWatcher != null)
                ProjectFolderWatcher.Dispose();

            string path = projectFile.Project_MainDirectory;

            ProjectFolderWatcher = new FileSystemWatcher(path);
            ProjectFolderWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            ProjectFolderWatcher.Changed += ProjectFolderWatcher_Changed;
            ProjectFolderWatcher.IncludeSubdirectories = true;
            ProjectFolderWatcher.EnableRaisingEvents = true;
        }

        private void ProjectFolderWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            string path = projectFile.Project_MainDirectory;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                projectFile.Project_FileTree = GetFilePathsItems(path);
                main.ProjectView_UpdateFileTree(projectFile.Project_FileTree);
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

        /// <summary>
        /// Updates the changes to the project file object, if the project file does not exist then it calls Project_SaveProjectAs() for a prompt.
        /// </summary>
        public void Project_SaveProject()
        {
            if (File.Exists(projectFile.Project_FilePath) == false)
            {
                Project_SaveProjectAs();
                return;
            }

            UpdateProjectFileWithChanges();
        }

        public void DuplicateProject()
        {

        }

        public void SetupProjectFolders(string mainProjectPath, ref ProjectFile projectFile)
        {
            string mainFolderName = Path.GetFileNameWithoutExtension(projectFile.Project_FilePath);
            string mainDir = mainProjectPath + "/" + mainFolderName + "/";
            string buildDir = mainDir + "/builds/";
            string importedDir = mainDir + "/imported/";

            ioManagement.CreateDirectory(mainDir);
            ioManagement.CreateDirectory(buildDir);
            ioManagement.CreateDirectory(importedDir);

            //mainDir = Path.GetFullPath(mainDir);
            //buildDir = Path.GetFullPath(buildDir);
            //importedDir = Path.GetFullPath(importedDir);

            projectFile.Project_MainDirectory = mainDir;
            projectFile.Project_BuildDirectory = buildDir;
            projectFile.Project_ImportedDirectory = importedDir;
        }

        /// <summary>
        /// Prompts the user for a save path, and updates the changes to the project file
        /// </summary>
        public void Project_SaveProjectAs()
        {
            string newPath = "";
            string extension = "Sound Mod Project (*.soundmodproj)|*.soundmodproj";

            ioManagement.SaveFilePath(ref newPath, extension, "Save Your Project");

            if (string.IsNullOrEmpty(newPath))
                return;

            string newPath_fullPath = Path.GetFullPath(newPath);
            string newPath_extension = Path.GetExtension(newPath);
            string newPath_noExt = newPath_fullPath.Remove(newPath_fullPath.Length - newPath_extension.Length, newPath_extension.Length);
            string finalPath = newPath_noExt + extension;

            projectFile.Project_FilePath = finalPath;
        }

        public void Open_ProjectFile()
        {
            string newPath = "";
            string extension = "Sound Mod Project (*.soundmodproj)|*.soundmodproj";

            ioManagement.GetFilePath(ref newPath, extension, "Select your Project Directory");

            if (string.IsNullOrEmpty(newPath))
                return;

            projectFile = Get_NewProjectFile(newPath);
        }

        /// <summary>
        /// Sets the project file object with an existing one.
        /// </summary>
        /// <param name="projectFile"></param>
        public void SetProjectFile(ProjectFile projectFile)
        {
            this.projectFile = projectFile;
        }

        public void Set_ProjectFile_GameVersion(GameVersion gameVersion)
        {
            projectFile.Project_GameVersion = Enum.GetName(typeof(GameVersion), gameVersion);
        }

        public void Set_ProjectFile_ModVersion(string modVersion)
        {
            projectFile.Project_ModVersion = modVersion;
        }

        public string GetProjectFilePath()
        {
            return projectFile.Project_FilePath;
        }

        //------------------ PROJECT FILE READ AND WRITE FUNCTIONS ------------------
        /// <summary>
        /// Writes the current project file values to the file.
        /// </summary>
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

        /// <summary>
        /// Gets a project file from a given filePath
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
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
                //get the name of the property from the json object
                string name = property.Name;

                if (name.Equals(nameof(projectFile.Project_FilePath)))
                    newProjectFile.Project_FilePath = (string)property.Value;

                if (name.Equals(nameof(projectFile.Project_Name)))
                    newProjectFile.Project_Name = (string)property.Value;

                if (name.Equals(nameof(projectFile.Project_Author)))
                    newProjectFile.Project_Author = (string)property.Value;

                if (name.Equals(nameof(projectFile.Project_ModVersion)))
                    newProjectFile.Project_ModVersion = (string)property.Value;

                if (name.Equals(nameof(projectFile.Project_GameVersion)))
                    newProjectFile.Project_GameVersion = (string)property.Value;

                if (name.Equals(nameof(projectFile.Project_MainDirectory)))
                    newProjectFile.Project_MainDirectory = (string)property.Value;

                if (name.Equals(nameof(projectFile.Project_BuildDirectory)))
                    newProjectFile.Project_BuildDirectory = (string)property.Value;

                if (name.Equals(nameof(projectFile.Project_ImportedDirectory)))
                    newProjectFile.Project_ImportedDirectory = (string)property.Value;

                if (name.Equals(nameof(projectFile.Project_ExtractedArchiveDirectories)))
                {
                    JArray extractedArchiveDirectoriesArray = (JArray)obj[nameof(projectFile.Project_ExtractedArchiveDirectories)];
                    List<string> parsed_Project_ExtractedArchiveDirectories = new List<string>();

                    foreach (JValue extractedArchiveDirectory in extractedArchiveDirectoriesArray)
                    {
                        parsed_Project_ExtractedArchiveDirectories.Add((string)extractedArchiveDirectory.Value);
                    }

                    newProjectFile.Project_ExtractedArchiveDirectories = parsed_Project_ExtractedArchiveDirectories;
                }

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
        //------------------ PROJECT FILE READ AND WRITE FUNCTIONS END ------------------
    }
}
