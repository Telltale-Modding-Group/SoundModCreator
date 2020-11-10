using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ControlzEx.Theming;

namespace SoundModCreator
{
    /// <summary>
    /// Interaction logic for ProjectSettings.xaml
    /// </summary>
    public partial class ProjectSettings
    {
        private bool newProjectMode; //true = createProjectMode. false = saveSettingsMode

        private Main main;
        private ProjectFile projectFile;
        private ProjectManager projectManager;
        private IOManagement ioManagement;

        public ProjectSettings(IOManagement ioManagement)
        {
            InitializeComponent();

            this.ioManagement = ioManagement;
        }

        public void Set_ProjectFile(ProjectFile projectFile)
        {
            this.projectFile = projectFile;
        }

        public void Set_MainObjects(Main main, ProjectManager projectManager)
        {
            this.projectManager = projectManager;
            this.main = main;

            UpdateUI();
        }

        public void ClearWindow()
        {
            ui_projectsettings_projectname_textbox.Text = "";
            ui_projectsettings_author_textbox.Text = "";
            ui_projectsettings_projectversion_textbox.Text = "";
            ui_projectsettings_gameversion_combobox.SelectedItem = null;
        }

        public void UpdateUI()
        {
            ThemeManager.Current.ChangeTheme(this, main.UI_GetTheme());

            var GameVersions_ToStringList = Enum.GetValues(typeof(GameVersion)).Cast<GameVersion>();

            ui_projectsettings_createproject_tile.IsEnabled = newProjectMode;
            ui_projectsettings_savesettings_tile.IsEnabled = !newProjectMode;
            ui_projectsettings_gameversion_combobox.ItemsSource = GameVersions_ToStringList;

            if (projectFile != null)
            {
                ui_projectsettings_projectname_textbox.Text = projectFile.Project_Name;
                ui_projectsettings_gameversion_combobox.SelectedItem = projectFile.Project_GameVersion;
                ui_projectsettings_projectversion_textbox.Text = projectFile.Project_ModVersion;
                ui_projectsettings_author_textbox.Text = projectFile.Project_Author;
            }
        }

        public void OpenWindow_AsNewProject()
        {
            newProjectMode = true;
            UpdateUI();
        }

        public void OpenWindow_AsExistingProject()
        {
            newProjectMode = false;
            UpdateUI();
        }

        public ProjectFile NewProjectFile()
        {
            projectFile = new ProjectFile();

            projectFile.Project_Name = ui_projectsettings_projectname_textbox.Text;
            projectFile.Project_Author = ui_projectsettings_author_textbox.Text;
            projectFile.Project_ModVersion = ui_projectsettings_projectversion_textbox.Text;
            projectFile.Project_GameVersion = ui_projectsettings_gameversion_combobox.SelectedItem.ToString();

            return projectFile;
        }

        public ProjectFile SaveProjectFile(ProjectFile projectFile)
        {
            projectFile.Project_Name = ui_projectsettings_projectname_textbox.Text;
            projectFile.Project_Author = ui_projectsettings_author_textbox.Text;
            projectFile.Project_ModVersion = ui_projectsettings_projectversion_textbox.Text;
            projectFile.Project_GameVersion = ui_projectsettings_gameversion_combobox.SelectedItem.ToString();

            return projectFile;
        }

        public bool ProjectValuesValid()
        {
            string finalMessage = "";

            if(string.IsNullOrEmpty(ui_projectsettings_projectname_textbox.Text))
            {
                finalMessage += String.Format("Project Name is Empty! ");
            }

            if (string.IsNullOrEmpty(ui_projectsettings_author_textbox.Text))
            {
                finalMessage += String.Format("Project Author is Empty! ");
            }

            if (string.IsNullOrEmpty(ui_projectsettings_projectversion_textbox.Text))
            {
                finalMessage += String.Format("Project Version is Empty! ");
            }

            if (ui_projectsettings_gameversion_combobox.SelectedItem == null)
            {
                finalMessage += String.Format("Project Game Version not set! ");
            }

            if(string.IsNullOrEmpty(finalMessage) == false)
            {
                MessageBox.Show(finalMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }

            return true;
        }

        private void ui_projectsettings_savesettings_tile_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectValuesValid() == false)
                return;

            projectManager.SetProjectFile(projectFile);
            main.Project_SaveProject();

            this.Hide();
            main.BackToMainWindow();
        }

        private void ui_projectsettings_createproject_tile_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectValuesValid() == false)
                return;

            ProjectFile newProjectFile = NewProjectFile();
            projectManager.Create_NewProject(newProjectFile);

            this.Hide();
            main.BackToMainWindow();
        }

        /// <summary>
        /// Checks for any changes against the project file.
        /// <para>If there are changes, returns true.</para>
        /// <para>If there are no changes, returns false.</para>
        /// </summary>
        /// <returns></returns>
        public bool CheckForChanges()
        {
            if (ui_projectsettings_projectname_textbox.Text != projectFile.Project_Name)
                return true;

            if (ui_projectsettings_author_textbox.Text != projectFile.Project_Author)
                return true;

            if (ui_projectsettings_projectversion_textbox.Text != projectFile.Project_ModVersion)
                return true;

            if (ui_projectsettings_gameversion_combobox.SelectedItem.ToString() != projectFile.Project_GameVersion)
                return true;

            return false;
        }

        private void ProjectWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;

            if (projectFile == null)
            {
                this.Hide();
                main.BackToMainWindow();
                return;
            }

            if(ProjectValuesValid() == false)
            {
                this.Hide();
                main.BackToMainWindow();
                return;
            }

            if (CheckForChanges())
            {
                if (ioManagement.MessageBox_Confirmation("You've mades changes that haven't been saved, Save Now?", "Save Changes"))
                {
                    projectManager.SetProjectFile(projectFile);
                    projectManager.Project_SaveProject();
                }
            }

            this.Hide();
            main.BackToMainWindow();
        }
    }
}
