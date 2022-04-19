using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml;

namespace YouTubeTracker
{
    /// <summary>
    /// Class <c>YoutubeTracker</c> stores currently used videos and is a link beetween window app, Youtube data API and database.
    /// </summary>
    public partial class YoutubeTracker
    {
        /// <summary>
        /// Search result videos.
        /// </summary>
        private static List<VideoData> LoadedVideosFromSearch;
        /// <summary>
        /// Selected playlist videos.
        /// </summary>
        private static List<VideoData> LoadedVideosFromDB;

        /// <summary>
        /// Default constructor.
        /// </summary>
        private YoutubeTracker()
        {
            LoadedVideosFromSearch = new List<VideoData>();
            LoadedVideosFromDB = new List<VideoData>();
        }
        /// <summary>
        /// Static method which returns search result videos.
        /// </summary>
        /// <returns>List of search result videos</returns>
        public static List<VideoData> GetLoadedVideosFromSearch()
        {
            if (LoadedVideosFromSearch == null) LoadedVideosFromSearch = new List<VideoData>();
            return LoadedVideosFromSearch;
        }
        /// <summary>
        /// Static method which returns selected playlist videos.
        /// </summary>
        /// <returns>List of selected playlist videos.</returns>
        public static List<VideoData> GetLoadedVideosFromDB()
        {
            if (LoadedVideosFromDB == null) LoadedVideosFromDB = new List<VideoData>();
            return LoadedVideosFromDB;
        }
        /// <summary>
        /// Static method for accesing Google api key.
        /// Reads api key from file <c>API_key.txt</c> in main project directory.
        /// When fails message box is shown.
        /// </summary>
        /// <returns>API key when suceed otherwise <c>null</c>.</returns>
        public static string GetApiKey()
        {
            string ApiKey;
            try
            {
                ApiKey = System.IO.File.ReadAllText(@"..\..\API_key.txt");
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show("Error: " + ex.Message + " Add API_key.txt with google api key in project directory.");
                return null;
            }
            return ApiKey;
        }
        /// <summary>
        /// Static method for executing youtube search request.
        /// Result can be accesed by <c>GetLoadedVideosFromSearch</c> method.
        /// Calls internal classes <c>YoutubeSearch</c>, <c>YoutubeVideo</c>.
        /// </summary>
        /// <param name="keyword">Phrase to be search on youtube</param>
        /// <param name="max_results">Maximum amount of results</param>
        /// <returns><c>true</c> if suceed otherwise <c>false</c></returns>
        public static bool Search(string keyword, uint max_results = 10)
        {
            var search = new YoutubeSearch();
            var yt_video = new YoutubeVideo();
            var search_result = search.Search(keyword, max_results);
            if (search_result == null) return false;

            var videos_with_all_data = yt_video.VideoList(
                search_result.videos.Select(video => video.id).ToList()
            ).videos_data;
            if (videos_with_all_data == null) return false;

            if (LoadedVideosFromSearch == null) LoadedVideosFromSearch = new List<VideoData>();
            LoadedVideosFromSearch.AddRange(videos_with_all_data);
            return true;
        }
        /// <summary>
        /// Static method for loading database videos to internal list.
        /// </summary>
        /// <param name="db_videos">List of videos from database request</param>
        /// <returns><c>true</c> if suceed otherwise <c>false</c>.</returns>
        public static bool LoadVideosFromDB(List<DataBase.DBVideo> db_videos)
        {
            if (LoadedVideosFromDB == null) LoadedVideosFromDB = new List<VideoData>();
            LoadedVideosFromDB.Clear();
            foreach (var db_video in db_videos)
            {
                var video = new VideoData()
                {
                    title = db_video.Title,
                    thumbnail_url = db_video.ThumbnailURL,
                    id = db_video.YTID,
                    embed_html = db_video.EmbedHTML,
                    duration = ""
                };
                LoadedVideosFromDB.Add(video);
            }
            return true;
        }
        /// <summary>
        /// Static method for converting video duration in ISO 8601 format to seconds(int) format.
        /// </summary>
        /// <param name="yt_duration">Duration in ISO 8601 format</param>
        /// <returns>Duration in seconds.</returns>
        public static int YTDuration2seconds(string yt_duration)
        {
            TimeSpan duration = XmlConvert.ToTimeSpan(yt_duration);
            return (int)duration.TotalSeconds;
        }
    }
}
