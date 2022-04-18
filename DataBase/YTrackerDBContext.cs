using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Collections.ObjectModel;


namespace YouTubeTracker.DataBase
{
    public class YTrackerDBContext : DbContext
    {
        public YTrackerDBContext()
            : base("name=YTrackerDBContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<DBVideo> DBVideos { get; set; }
        public DbSet<DBPlaylist> DBPlaylists { get; set; }

    }
}
