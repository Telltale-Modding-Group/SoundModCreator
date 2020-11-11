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

namespace SoundModCreator
{
    /// <summary>
    /// Interaction logic for ApplicaitonSettings_Window.xaml
    /// </summary>
    public partial class ApplicaitonSettings_Window
    {
        public ApplicaitonSettings_Window()
        {
            InitializeComponent();
        }

        private void AppSettingsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
