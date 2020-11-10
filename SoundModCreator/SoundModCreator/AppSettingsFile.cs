using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundModCreator
{
    public class AppSettingsFile
    {
        /// <summary>
        /// Whether the UI is using the Light Theme
        /// </summary>
        public bool UI_LightMode { get; set; }

        /// <summary>
        /// The location of the ttarchext executable
        /// </summary>
        public string Location_Ttarchext { get; set; } = "Undefined";

        /// <summary>
        /// The list of recent project files created/opened with the tool
        /// </summary>
        public List<string> RecentProjectFiles { get; set; }
    }
}
