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

namespace SoundModCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public string path = "A:/Work/MODDING/Github/TESTING-DIR/SOUNDMODCREATOR-TEST";
        private string selectedFile = "";
        private AudioPlayer audioPlayer;

        public MainWindow()
        {
            InitializeComponent();
            InitalizeApplication();

            FileSystemWatcher ProjectFolderWatcher = new FileSystemWatcher(path);
            ProjectFolderWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            ProjectFolderWatcher.Changed += ProjectFolderWatcher_Changed; ;
            ProjectFolderWatcher.IncludeSubdirectories = true;
            ProjectFolderWatcher.EnableRaisingEvents = true;

            DataContext = GetFilePathsItems(path);

            UpdateUI();
        }

        private void ProjectFolderWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => 
            { 
                DataContext = GetFilePathsItems(path); 
            }));
        }

        private void InitalizeApplication()
        {
            audioPlayer = new AudioPlayer(this);
        }

        private void UpdateUI()
        {
            
        }

        public void UpdateUI_AudioPlayer()
        {
            Visibility playButtonVisibility = audioPlayer.IsPlaying() ? Visibility.Hidden : Visibility.Visible;
            Visibility pauseButtonVisibility = audioPlayer.IsPlaying() ? Visibility.Visible : Visibility.Hidden;

            ui_audioplayer_play_button.Visibility = playButtonVisibility;
            ui_audioplayer_pause_button.Visibility = pauseButtonVisibility;

            int currentTime_minutes = (int)audioPlayer.GetTime_CurrentTime() / 60;
            int currentTime_seconds = (int)audioPlayer.GetTime_CurrentTime() % 60;
            string currentTime_string = string.Format("{0}:{1}", currentTime_minutes, currentTime_seconds.ToString("D2"));
            ui_audioplayer_currentTime_label.Content = currentTime_string;

            int fullTime_minutes = (int)audioPlayer.GetTime_FullLength() / 60;
            int fullTime_seconds = (int)audioPlayer.GetTime_FullLength() % 60;
            string fullTime_string = string.Format("{0}:{1}", fullTime_minutes, fullTime_seconds.ToString("D2"));
            ui_audioplayer_fullTime_label.Content = fullTime_string;

            ui_audioplayer_seekbar_slider.Maximum = audioPlayer.GetTime_FullLength();
            ui_audioplayer_seekbar_slider.Value = audioPlayer.GetTime_CurrentTime();

            int volumePercent = (int)(audioPlayer.GetVolume() * 100);
            string volumePercent_string = string.Format("{0}%", volumePercent);
            ui_audioplayer_volume_label.Content = volumePercent_string;
            ui_audioplayer_volume_slider.Maximum = 1;
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

        private void ui_projectview_treeview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Item selectedItem = (Item)ui_projectview_treeview.SelectedValue;
            selectedFile = selectedItem.Name;

            ui_audioplayer_audiofile_label.Content = string.Format("Sound: {0}", selectedFile);

            audioPlayer.LoadAudio(selectedItem.Path);
        }

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
            audioPlayer.SeekAudio(ui_audioplayer_seekbar_slider.Value);

            UpdateUI_AudioPlayer();
        }

        private void ui_audioplayer_volume_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = ui_audioplayer_volume_slider.Value;
            audioPlayer.SetVolume(value);
        }
    }
}
