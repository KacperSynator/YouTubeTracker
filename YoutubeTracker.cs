using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;

namespace YouTubeTracker
{
    public partial class YoutubeTracker
    {
        private static YoutubeTracker instance;
        private static List<VideoData> LoadedVideos;
        private YoutubeTracker()
        {
            LoadedVideos = new List<VideoData>();
        }

        public static YoutubeTracker Get
        {
            get
            {
                if (instance == null)
                {
                    instance = new YoutubeTracker();
                }
                return instance;
            }
        }

        public static List<VideoData> GetLoadedVideos()
        {
            return LoadedVideos;
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

            if (LoadedVideos == null) LoadedVideos = new List<VideoData>();
            LoadedVideos.AddRange(videos_with_all_data);
            return true;
        }
    }
}
