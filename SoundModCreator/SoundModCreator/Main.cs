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
    public class Main
    {
        public Item selectedItem;
        public AudioPlayer audioPlayer;

        private Help help;
        private IOManagement ioManagement;
        private AppSettings appSettings;
        private ProjectManager projectManager;

        private MainWindow mainWindow;
        private ProjectSettings projectSettings;

        public Main(MainWindow mainWindow, ProjectSettings projectSettings, IOManagement ioManagement, AppSettings appSettings, Help help, ProjectManager projectManager)
        {
            this.mainWindow = mainWindow;
            this.ioManagement = ioManagement;
            this.projectSettings = projectSettings;
            this.appSettings = appSettings;
            this.help = help;
            this.projectManager = projectManager;

            audioPlayer = new AudioPlayer(mainWindow);
        }

        public void ProjectView_DoubleClick(object selectedValue)
        {
            selectedItem = (Item)selectedValue;

            audioPlayer.LoadAudio(selectedItem.Path);
        }

        public void ProjectView_UpdateFileTree(List<Item> fileTree)
        {
            mainWindow.DataContext = fileTree;
        }

        /// <summary>
        /// Updates the changes to the project file object, if the project file does not exist then it calls Project_SaveProjectAs() for a prompt.
        /// </summary>
        public void Project_SaveProject()
        {
            projectManager.Project_SaveProject();
        }

        /// <summary>
        /// Prompts the user for a save path, and updates the changes to the project file
        /// </summary>
        public void Project_SaveProjectAs()
        {
            projectManager.Project_SaveProjectAs();
        }

        /// <summary>
        /// Used by different scripting objects to return back to the main application window.
        /// </summary>
        public void BackToMainWindow()
        {
            mainWindow.Show();
            mainWindow.Activate();
        }

        public void OpenProjectFolderDirectory()
        {
            /*
            //create a windows explorer processinfo to be exectued
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = projectFile
            processStartInfo.UseShellExecute = true;
            processStartInfo.Verb = "open";

            //start the process
            Process.Start(processStartInfo);
            */
        }

        public void New_ProjectFile()
        {
            projectManager.Open_Create_NewProjectFileWindow();
        }

        public void Open_ProjectFile()
        {
            projectManager.Open_ProjectFile();
        }

        public string UI_GetTheme()
        {
            return "Dark.Blue";
        }
    }
}
