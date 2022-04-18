using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeTracker.DataBase
{
    public class DBPlaylist
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public virtual ICollection<DBVideo> DBVideo { get; set; }
    }
}
