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

    public class YoutubeVideo
    {
        public List<List<String>> VideoList(string video_id)
        {
            List<List<String>> result = null;
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
        private List<List<String>> InternalVideoList(string video_id)
        {
            var ApiKey = System.IO.File.ReadAllText(@"..\..\API_key.txt");

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = ApiKey,
                ApplicationName = "YoutubeTracker"
            });

            var videoListRequest = youtubeService.Videos.List("player,contentDetails,statistics");
            videoListRequest.Id = video_id;

            // Call the video.list method to retrieve results matching the specified query term.
            var videoListResponse = videoListRequest.Execute();

            List<List<string>> result = new List<List<string>>();

            // Add info result to the result list
            foreach (var videoResult in videoListResponse.Items)
            {
                var video_info = new List<string>();
                video_info.Add(video_id);
                video_info.Add(videoResult.ContentDetails.Duration);
                video_info.Add(videoResult.Player.EmbedHtml);
                result.Add(video_info);
            }

            return result;
        }
    }
}
