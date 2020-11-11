using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Timers;
using SoundModCreator.FileTree;
using ControlzEx.Theming;

namespace SoundModCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        //main objects
        private Help help;
        private Main main;
        private AudioPlayer audioPlayer;
        private AppSettings appSettings;
        private IOManagement ioManagement;
        private ProjectManager projectManager;

        //xaml window
        private ProjectSettings projectSettings;
        private ApplicaitonSettings_Window applicaitonSettings_Window;

        /// <summary>
        /// XAML Window Initlization
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            InitalizeApplication();

            UpdateUI();
        }

        /// <summary>
        /// Creates and Initalizes all of the objects associated with the application.
        /// </summary>
        private void InitalizeApplication()
        {
            help = new Help();
            ioManagement = new IOManagement();

            appSettings = new AppSettings(ioManagement);
            projectSettings = new ProjectSettings(ioManagement);
            projectManager = new ProjectManager(ioManagement, projectSettings);
            main = new Main(this, projectSettings, ioManagement, appSettings, help, projectManager);
            audioPlayer = new AudioPlayer(this);
            applicaitonSettings_Window = new ApplicaitonSettings_Window();

            projectManager.Set_MainObject(main);
            projectSettings.Set_MainObjects(main, projectManager);
        }

        /// <summary>
        /// Updates the UI for the MainWindow.xaml
        /// </summary>
        private void UpdateUI()
        {
            ThemeManager.Current.ChangeTheme(this, main.UI_GetTheme());

            bool isRecentFilesEmpty = ui_menu_file_openRecentProject_menuItem.ItemsSource != null;
            bool doesProjectExist = projectManager.projectFile != null && Directory.Exists(projectManager.projectFile.Project_MainDirectory);
            ui_projectview_opendirectory_button.IsEnabled = doesProjectExist;
            ui_menu_build_buildMod_menuItem.IsEnabled = doesProjectExist;
            ui_menu_build_configureMod_menuItem.IsEnabled = doesProjectExist;
            ui_menu_file_openRecentProject_menuItem.IsEnabled = isRecentFilesEmpty;

            UpdateUI_AudioPlayer();
        }

        private void ui_projectview_treeview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            main.ProjectView_DoubleClick(ui_projectview_treeview.SelectedValue);

            UpdateUI();
        }

        private void ui_menu_file_newProject_menuItem_Click(object sender, RoutedEventArgs e)
        {
            main.New_ProjectFile();

            UpdateUI();
        }

        private void ui_menu_file_openProject_menuItem_Click(object sender, RoutedEventArgs e)
        {
            main.Open_ProjectFile();

            UpdateUI();
        }

        private void ui_menu_file_save_menuItem_Click(object sender, RoutedEventArgs e)
        {
            main.Project_SaveProject();

            UpdateUI();
        }

        private void ui_menu_file_saveAs_menuItem_Click(object sender, RoutedEventArgs e)
        {
            main.Project_SaveProjectAs();

            UpdateUI();
        }

        private void ui_menu_edit_undo_menuItem_Click(object sender, RoutedEventArgs e)
        {
            UpdateUI();
        }

        private void ui_menu_edit_redo_menuItem_Click(object sender, RoutedEventArgs e)
        {
            UpdateUI();
        }

        private void ui_menu_build_buildMod_menuItem_Click(object sender, RoutedEventArgs e)
        {
            UpdateUI();
        }

        private void ui_menu_build_configureMod_menuItem_Click(object sender, RoutedEventArgs e)
        {
            UpdateUI();
        }

        private void ui_menu_options_applicationSettings_menuItemm_Click(object sender, RoutedEventArgs e)
        {
            applicaitonSettings_Window.Show();
            applicaitonSettings_Window.Activate();

            UpdateUI();
        }

        private void ui_projectview_opendirectory_button_Click(object sender, RoutedEventArgs e)
        {
            UpdateUI();
        }

        private void ui_menu_help_about_menuItem_Click(object sender, RoutedEventArgs e)
        {
            help.GetHelp_About();

            UpdateUI();
        }

        private void ui_menu_help_documentation_menuItem_Click(object sender, RoutedEventArgs e)
        {
            help.GetHelp_Documentation();

            UpdateUI();
        }

        //--------------------------- AUDIO PLAYER FUNCTIONS ---------------------------
        /// <summary>
        /// Updates the UI for the Audio Player UI elements.
        /// </summary>
        public void UpdateUI_AudioPlayer()
        {
            Visibility playButtonVisibility = main.audioPlayer.IsPlaying() ? Visibility.Hidden : Visibility.Visible;
            Visibility pauseButtonVisibility = main.audioPlayer.IsPlaying() ? Visibility.Visible : Visibility.Hidden;

            ui_audioplayer_play_button.Visibility = playButtonVisibility;
            ui_audioplayer_pause_button.Visibility = pauseButtonVisibility;

            int currentTime_minutes = (int)main.audioPlayer.GetTime_CurrentTime() / 60;
            int currentTime_seconds = (int)main.audioPlayer.GetTime_CurrentTime() % 60;
            string currentTime_string = string.Format("{0}:{1}", currentTime_minutes, currentTime_seconds.ToString("D2"));
            ui_audioplayer_currentTime_label.Content = currentTime_string;

            int fullTime_minutes = (int)main.audioPlayer.GetTime_FullLength() / 60;
            int fullTime_seconds = (int)main.audioPlayer.GetTime_FullLength() % 60;
            string fullTime_string = string.Format("{0}:{1}", fullTime_minutes, fullTime_seconds.ToString("D2"));
            ui_audioplayer_fullTime_label.Content = fullTime_string;

            ui_audioplayer_seekbar_slider.Maximum = main.audioPlayer.GetTime_FullLength();
            ui_audioplayer_seekbar_slider.Value = main.audioPlayer.GetTime_CurrentTime();

            int volumePercent = (int)(ui_audioplayer_volume_slider.Value * 100);
            string volumePercent_string = string.Format("{0}%", volumePercent);
            ui_audioplayer_volume_label.Content = volumePercent_string;
            ui_audioplayer_volume_slider.Maximum = 1;

            if (main.selectedItem == null)
                ui_audioplayer_audiofile_label.Content = string.Format("Sound: {0}", "No Sound Selected.");
            else
                ui_audioplayer_audiofile_label.Content = string.Format("Sound: {0}", main.selectedItem.Name);
        }

        //--------------------------- AUDIO PLAYER FUNCTIONS END ---------------------------
        //--------------------------- AUDIO PLAYER XAML ---------------------------
        private void ui_audioplayer_play_button_Click(object sender, RoutedEventArgs e)
        {
            Item selectedItem = (Item)ui_projectview_treeview.SelectedValue;
            string selectedFilePath = selectedItem.Path;

            audioPlayer.PlayAudio(selectedFilePath);
            UpdateUI_AudioPlayer();
        }

        private void ui_audioplayer_pause_button_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.PauseAudio();
            UpdateUI_AudioPlayer();
        }

        private void ui_audioplayer_stop_button_Click(object sender, RoutedEventArgs e)
        {
            audioPlayer.StopAudio();
            UpdateUI_AudioPlayer();
        }

        private void ui_audioplayer_repeat_button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ui_audioplayer_seekbar_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (audioPlayer == null)
                return;

            audioPlayer.SeekAudio(ui_audioplayer_seekbar_slider.Value);
            UpdateUI_AudioPlayer();
        }

        private void ui_audioplayer_volume_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (audioPlayer == null)
                return;

            double value = ui_audioplayer_volume_slider.Value;
            audioPlayer.SetVolume(value);
            UpdateUI_AudioPlayer();
        }
        //--------------------------- AUDIO PLAYER XAML END ---------------------------
    }
}
