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
    public partial class YoutubeTracker
    {
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
            public VideoResult VideoList(List<string> videos_id)
            {
                var result = new VideoResult();
                try
                {
                    result = InternalVideoList(videos_id);
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
            private VideoResult InternalVideoList(List<string> videos_id)
            {
                string ApiKey = YoutubeTracker.GetApiKey();
                if (ApiKey == null) return null;

                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = ApiKey,
                    ApplicationName = "YoutubeTracker"
                });

                var videoListRequest = youtubeService.Videos.List("player,contentDetails,statistics,snippet,id");
                videoListRequest.Id = String.Join(",", videos_id);

                // Call the video.list method to retrieve results matching the specified query term.
                VideoListResponse videoListResponse = null;
                try
                {
                    videoListResponse = videoListRequest.Execute();
                }
                catch (Google.GoogleApiException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return null;
                }


                var result = new VideoResult();

                // Add info result to the result list
                foreach (var videoResult in videoListResponse.Items)
                {
                    var video_info = new VideoData(
                        videoResult.Id,
                        videoResult.Snippet.Title,
                        videoResult.ContentDetails.Duration,
                        videoResult.Player.EmbedHtml,
                        videoResult.Snippet.Thumbnails.Default__.Url
                        );
                    result.videos_data.Add(video_info);
                }

                return result;
            }
        }
    }
}
