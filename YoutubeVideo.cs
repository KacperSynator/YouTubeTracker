using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace YouTubeTracker
{
    public struct VideoData
    {
        public string id;
        public string title;
        public string duration;
        public string embed_html;

        public VideoData(string _id, string _title)
        {
            id = _id;
            title = _title;
            duration = "";
            embed_html = "";
        }
        public VideoData(string _id, string _title, string _duration, string _embed_html )
        {
            id = _id;
            title = _title;
            duration = _duration;
            embed_html = _embed_html;
        }
    }

    public class VideoResult
    {
        public List<VideoData> videos_data;

        public VideoResult()
        {
            videos_data = new List<VideoData>();
        }
    }

    public class YoutubeVideo
    {
        public VideoResult VideoList(string video_id)
        {
            var result = new VideoResult();
            try
            {
                result = InternalVideoList(video_id);
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    MessageBox.Show("Error: " + e.Message);
                }
            }
            return result;
        }
        private VideoResult InternalVideoList(string video_id)
        {
            var ApiKey = System.IO.File.ReadAllText(@"..\..\API_key.txt");

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = ApiKey,
                ApplicationName = "YoutubeTracker"
            });

            var videoListRequest = youtubeService.Videos.List("player,contentDetails,statistics,snippet");
            videoListRequest.Id = video_id;

            // Call the video.list method to retrieve results matching the specified query term.
            var videoListResponse = videoListRequest.Execute();

            var result = new VideoResult();

            // Add info result to the result list
            foreach (var videoResult in videoListResponse.Items)
            {
                var video_info = new VideoData(
                    video_id,
                    videoResult.Snippet.Title,
                    videoResult.ContentDetails.Duration,
                    videoResult.Player.EmbedHtml
                    );
                result.videos_data.Add(video_info);
            }

            return result;
        }
    }
}
