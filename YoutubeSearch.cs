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
    public class SearchResult
    {
        public List<VideoData> videos;
        public List<VideoData> channels;
        public List<VideoData> playlists;

        public SearchResult()
        {
            videos = new List<VideoData>();
            channels = new List<VideoData>();
            playlists = new List<VideoData>();
        }

    }
    
    public class YoutubeSearch
    {
        public SearchResult Search(string keyword, uint max_results = 5)
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
        private SearchResult InternalSearch(string keyword, uint max_results)
        {
            string ApiKey = null;
            try
            {
                ApiKey = System.IO.File.ReadAllText(@"..\..\API_key.txt");
            }
            catch(System.IO.IOException ex)
            {
                MessageBox.Show("Error: "+ ex.Message +" Add API_key.txt with google api key in project directory.");
                return null;
            }
           

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = ApiKey,
                ApplicationName = "YoutubeTracker"
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = keyword;
            searchListRequest.MaxResults = max_results;

            // Call the search.list method to retrieve results matching the specified query term.
            SearchListResponse searchListResponse = null;
            try
            {
                searchListResponse = searchListRequest.Execute();
            }
            catch(Google.GoogleApiException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                return null;
            }

            SearchResult result = new SearchResult();

            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        result.videos.Add(new VideoData(searchResult.Id.VideoId, searchResult.Snippet.Title));
                        break;

                    case "youtube#channel":
                        result.channels.Add(new VideoData(searchResult.Id.ChannelId, searchResult.Snippet.Title));
                        break;

                    case "youtube#playlist":
                        result.playlists.Add(new VideoData(searchResult.Id.PlaylistId, searchResult.Snippet.Title));
                        break;
                }
            }

            /* MessageBox.Show(String.Format("Videos:\n{0}\n", string.Join("\n", videos)));
            MessageBox.Show(String.Format("Channels:\n{0}\n", string.Join("\n", channels)));
            MessageBox.Show(String.Format("Playlists:\n{0}\n", string.Join("\n", playlists))); */

            return result;
        }
    }
}
