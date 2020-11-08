using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoundModCreator.FileTree;

namespace SoundModCreator
{
    public class ProjectFile
    {
        public string Project_Extension { get { return ".soundmodproj"; } }

        public string Project_FilePath { get; set; }
        public string Project_Directory { get; set; }
        public string Project_GameVersion { get; set; }
        public string Project_ModVersion { get; set; }
        public List<Item> Project_FileTree { get; set; }
    }
}
