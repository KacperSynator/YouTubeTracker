using System.Collections.Generic;


namespace YouTubeTracker.DataBase
{
    /// <summary>
    /// Class <c>DBPlaylist</c> models a database entity playlist.
    /// </summary>
    public class DBPlaylist
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Name of playlist.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Collection of videos.
        /// </summary>
        public virtual ICollection<DBVideo> DBVideo { get; set; }
    }
}
