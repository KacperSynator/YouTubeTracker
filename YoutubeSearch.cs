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
    
    public class YoutubeSearch
    {
        public List<List<(String, String)>> Search(string keyword, uint max_results = 5)
        {
            List<List<(String, String)>> result = null;
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
        private List<List<(String, String)>> InternalSearch(string keyword, uint max_results)
        {
            var ApiKey = System.IO.File.ReadAllText(@"..\..\API_key.txt");

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = ApiKey,
                ApplicationName = "YoutubeTracker"
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = keyword;
            searchListRequest.MaxResults = max_results;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = searchListRequest.Execute();

            List<(string, string)> videos = new List<(string, string)>();
            List<(string, string)> channels = new List<(string, string)>();
            List<(string, string)> playlists = new List<(string, string)>();

            // Add each result to the appropriate list, and then display the lists of
            // matching videos, channels, and playlists.
            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        videos.Add((searchResult.Snippet.Title, searchResult.Id.VideoId));
                        break;

                    case "youtube#channel":
                        channels.Add((searchResult.Snippet.Title, searchResult.Id.ChannelId));
                        break;

                    case "youtube#playlist":
                        playlists.Add((searchResult.Snippet.Title, searchResult.Id.PlaylistId));
                        break;
                }
            }

            /* MessageBox.Show(String.Format("Videos:\n{0}\n", string.Join("\n", videos)));
            MessageBox.Show(String.Format("Channels:\n{0}\n", string.Join("\n", channels)));
            MessageBox.Show(String.Format("Playlists:\n{0}\n", string.Join("\n", playlists))); */

            return new List<List<(String, String)>> { videos, channels, playlists };
        }
    }
}
