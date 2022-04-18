using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace YouTubeTracker.DataBase
{
    public class DBVideo
    {
        public int ID { get; set; }
        [ForeignKey("DBPlaylist")]
        public int DBPlaylistID { get; set; }
        public string YTID { get; set; }
        public int Duration { get; set; }
        public string Title { get; set; }
        public string EmbedHTML { get; set; }
        public string ThumbnailURL { get; set; }

        public virtual DBPlaylist DBPlaylist { get; set; }

    }
}
