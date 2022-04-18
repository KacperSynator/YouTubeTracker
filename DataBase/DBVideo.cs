using System.ComponentModel.DataAnnotations.Schema;

namespace YouTubeTracker.DataBase
{
    /// <summary>
    /// Class <c>DBPlaylist</c> models a database entity video.
    /// </summary>
    public class DBVideo
    {
        /// <summary>
        /// Primaty key
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Foreign key, id of playlist to which viedo belongs.
        /// </summary>
        [ForeignKey("DBPlaylist")]
        public int DBPlaylistID { get; set; }
        /// <summary>
        /// Youtube's video id
        /// </summary>
        public string YTID { get; set; }
        /// <summary>
        /// Video's duration
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// Video's title
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Videos's embed html for playing a video in web browser.
        /// </summary>
        public string EmbedHTML { get; set; }
        /// <summary>
        /// Videos's thumbnail url, image
        /// </summary>
        public string ThumbnailURL { get; set; }
        /// <summary>
        /// Playlist to which viedo belongs.
        /// </summary>
        public virtual DBPlaylist DBPlaylist { get; set; }

    }
}
