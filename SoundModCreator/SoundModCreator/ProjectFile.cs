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
        public string Project_FilePath { get; set; }

        public string Project_Name { get; set; }
        public string Project_Author { get; set; }
        public string Project_ModVersion { get; set; }
        public string Project_GameVersion { get; set; }
        public string Project_MainDirectory { get; set; }
        public string Project_BuildDirectory { get; set; }
        public string Project_ImportedDirectory { get; set; }
        public List<string> Project_ExtractedArchiveDirectories { get; set; }
        public List<Item> Project_FileTree { get; set; }
    }
}
