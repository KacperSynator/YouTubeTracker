using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Xml;

namespace YouTubeTracker
{
    public partial class YoutubeTracker
    {
        private static List<VideoData> LoadedVideosFromSearch;
        private static List<VideoData> LoadedVideosFromDB;

        private YoutubeTracker()
        {
            LoadedVideosFromSearch = new List<VideoData>();
            LoadedVideosFromDB = new List<VideoData>();
        }

        public static List<VideoData> GetLoadedVideosFromSearch()
        {
            if (LoadedVideosFromSearch == null) LoadedVideosFromSearch = new List<VideoData>();
            return LoadedVideosFromSearch;
        }

        public static List<VideoData> GetLoadedVideosFromDB()
        {
            if (LoadedVideosFromDB == null) LoadedVideosFromDB = new List<VideoData>();
            return LoadedVideosFromDB;
        }

        public static string GetApiKey()
        {
            string ApiKey = null;
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

        public static int YTDuration2seconds(string yt_duration)
        {
            TimeSpan duration = XmlConvert.ToTimeSpan(yt_duration);
            return (int)duration.TotalSeconds;
        }
    }
}
