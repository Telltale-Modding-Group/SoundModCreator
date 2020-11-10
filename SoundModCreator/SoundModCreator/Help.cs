using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SoundModCreator
{
    public class Help
    {
        private static string launcherAboutLink = "https://github.com/Telltale-Modding-Group/SoundModCreator/wiki";
        private static string launcherDocumentationLink = "https://github.com/Telltale-Modding-Group/SoundModCreator/wiki/%5BDocumentation%5D";

        public void GetHelp_About()
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = launcherAboutLink,
                UseShellExecute = true
            };

            Process.Start(processStartInfo);
        }

        public void GetHelp_Documentation()
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = launcherDocumentationLink,
                UseShellExecute = true
            };

            Process.Start(processStartInfo);
        }
    }
}
