using System;
using System.Collections.Generic;
using System.Windows;

using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;

namespace YouTubeTracker
{
    public partial class YoutubeTracker
    {
        /// <summary>
        /// Class <c>VideoResult</c> models result of youtube VideoList request.
        /// Result videos are stored in list <c>videos_data</c>.
        /// </summary>
        public class VideoResult
        {
            /// <summary>
            /// Video list result videos.
            /// </summary>
            public List<VideoData> videos_data;

            /// <summary>
            /// Parameterless constructor.
            /// </summary>
            public VideoResult()
            {
                videos_data = new List<VideoData>();
            }
        }

        /// <summary>
        /// Internal class for executing youtube VideoList request.
        /// </summary>
        private class YoutubeVideo
        {
            /// <summary>
            /// Public method which executes VideoList request.
            /// Message box with error will be shown if something will go wrong.
            /// </summary>
            /// <param name="videos_id">List of youtube video ids.</param>.
            /// <returns>Result of youtube VideoList request.</returns>
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
            /// <summary>
            /// Internal method which executes VideoList request.
            /// Message box with error will be shown if something will go wrong.
            /// </summary>
            /// <param name="videos_id">List of youtube video ids.</param>.
            /// <returns>Result of youtube VideoList request.</returns>
            private VideoResult InternalVideoList(List<string> videos_id)
            {
                // get api key
                string ApiKey = YoutubeTracker.GetApiKey();
                if (String.IsNullOrEmpty(ApiKey)) return null;
                // create new youtube service instance
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = ApiKey,
                    ApplicationName = "YoutubeTracker"
                });
                // specify request parameters
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
                // Add info result to the result list
                var result = new VideoResult();
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
