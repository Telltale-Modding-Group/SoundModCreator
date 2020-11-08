using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Timers;
using System.IO;
using Plugin.SimpleAudioPlayer;
using System.ComponentModel;

namespace SoundModCreator
{
    public class AudioPlayer
    {
        private ISimpleAudioPlayer audioPlayer;
        private Timer timer;

        private MainWindow mainWindow;

        public AudioPlayer(MainWindow window)
        {
            audioPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            audioPlayer.PlaybackEnded += AudioPlayer_PlaybackEnded;
            mainWindow = window;

            timer = new Timer(50);
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                mainWindow.UpdateUI_AudioPlayer();
            }));
        }

        private void AudioPlayer_PlaybackEnded(object sender, EventArgs e)
        {
            timer.Stop();
            audioPlayer.Stop();

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                mainWindow.UpdateUI_AudioPlayer();
            }));
        }

        public bool IsAudioFile(string path)
        {
            if (Path.GetExtension(path).Equals(".wav") || Path.GetExtension(path).Equals(".mp3"))
                return true;
            else
                return false;
        }

        public void LoadAudio(string path)
        {
            if(IsAudioFile(path) && !string.IsNullOrEmpty(path))
            {
                using (Stream stream = File.Open(path, FileMode.Open))
                {
                    audioPlayer.Load(stream);
                }
            }
        }

        public void PlayAudio(string audioPath)
        {
            if (audioPlayer.IsPlaying)
                audioPlayer.Stop();

            audioPlayer.Play();
            timer.Start();
        }

        public void PauseAudio()
        {
            audioPlayer.Pause();
            timer.Stop();
        }

        public void StopAudio()
        {
            audioPlayer.Stop();
            timer.Stop();
        }

        public void SeekAudio(double position)
        {
            audioPlayer.Seek(position);
        }

        public double GetTime_FullLength()
        {
            double time = 0;

            try
            {
                time = (int)audioPlayer.Duration;
            }
            catch(OverflowException e)
            {

            }

            return time;
        }
        
        public void SetVolume(double value)
        {
            audioPlayer.Volume = value;
        }

        public double GetTime_CurrentTime()
        {
            double time = 0;

            try
            {
                time = (int)audioPlayer.CurrentPosition;
            }
            catch (OverflowException e)
            {

            }

            return time;
        }

        public bool IsPlaying()
        {
            return audioPlayer.IsPlaying;
        }
    }
}
