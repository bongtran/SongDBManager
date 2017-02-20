using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SongsDB
{
    public class Song
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Name_non { get; set; }
        public string File_name { get; set; }
        public string Url1 { get; set; }
        public string Startwith { get; set; }
        public string StartPhrase { get; set; }

        public Song()
        {
            this.ID = 0;
            this.File_name = string.Empty;
            this.Name = string.Empty;
            this.Name_non = string.Empty;
            this.Startwith = string.Empty;
            this.Url1 = string.Empty;
        }

        public Song(int id, string name, string url)
        {
            this.ID = id;
            this.File_name = string.Empty;
            this.Name = name;
            this.Name_non = string.Empty;
            this.Startwith = string.Empty;
            this.Url1 = url;
        }
    }
}
