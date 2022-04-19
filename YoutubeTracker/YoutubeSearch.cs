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
        /// Class <c>SearchResult</c> models result of youtube Search request.
        /// Result videos, channels, playlists are stored in adequate lists.
        /// </summary>
        public class SearchResult
        {
            /// <summary>
            /// Search videos results.
            /// </summary>
            public List<VideoData> videos;
            /// <summary>
            /// Search channels results.
            /// </summary>
            public List<VideoData> channels;
            /// <summary>
            /// Search playlists results.
            /// </summary>
            public List<VideoData> playlists;

            /// <summary>
            /// Parameterless constructor.
            /// </summary>
            public SearchResult()
            {
                videos = new List<VideoData>();
                channels = new List<VideoData>();
                playlists = new List<VideoData>();
            }
        }

        /// <summary>
        /// Internal class for executing youtube Search request.
        /// </summary>
        private class YoutubeSearch
        {
            /// <summary>
            /// Public method which executes Search request.
            /// Message box with error will be shown if something will go wrong.
            /// </summary>
            /// <param name="keyword">Search phrase.</param>
            /// <param name="max_results">Maximum amount of results.</param>
            /// <returns></returns>
            public SearchResult Search(string keyword, uint max_results = 10)
            {
                var result = new SearchResult();
                try
                {
                    result = InternalSearch(keyword, max_results);
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
            /// Internal method which executes Search request.
            /// Message box with error will be shown if something will go wrong.
            /// </summary>
            /// <param name="keyword">Search phrase.</param>
            /// <param name="max_results">Maximum amount of results.</param>
            /// <returns></returns>
            private SearchResult InternalSearch(string keyword, uint max_results)
            {
                // get api key
                string ApiKey = YoutubeTracker.GetApiKey();
                if (ApiKey == null) return null;
                // create new youtube service instance
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = ApiKey,
                    ApplicationName = "YoutubeTracker"
                });
                // specify request parameters
                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = keyword;
                searchListRequest.MaxResults = max_results;
                // Call the search.list method to retrieve results matching the specified query term.
                SearchListResponse searchListResponse;
                try
                {
                    searchListResponse = searchListRequest.Execute();
                }
                catch (Google.GoogleApiException ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return null;
                }
                // Add each result to the appropriate list
                SearchResult result = new SearchResult();
                foreach (var searchResult in searchListResponse.Items)
                {
                    switch (searchResult.Id.Kind)
                    {
                        case "youtube#video":
                            result.videos.Add(new VideoData(searchResult.Id.VideoId));
                            break;

                        case "youtube#channel":
                            result.channels.Add(new VideoData(searchResult.Id.ChannelId));
                            break;

                        case "youtube#playlist":
                            result.playlists.Add(new VideoData(searchResult.Id.PlaylistId));
                            break;
                    }
                }
                return result;
            }
        }
    }
}
