using System.Data.Entity;


namespace YouTubeTracker.DataBase
{
    /// <summary>
    /// Class <c>YTrackerDBContext</c> models a context of database.
    /// </summary>
    public class YTrackerDBContext : DbContext
    {
        /// <summary>
        /// Parameterless constructor
        /// </summary>
        public YTrackerDBContext()
            : base("name=YTrackerDBContext")
        {
            Configuration.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// Videos entity
        /// </summary>
        public DbSet<DBVideo> DBVideos { get; set; }
        /// <summary>
        /// Playlists entity
        /// </summary>
        public DbSet<DBPlaylist> DBPlaylists { get; set; }

    }
}
