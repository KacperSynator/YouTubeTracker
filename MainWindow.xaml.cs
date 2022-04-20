using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using Microsoft.Web.WebView2.Core;


namespace YouTubeTracker
{
    /// <summary>
    /// Interaction logic for class MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes components and populates ComboBox: <c>playListCB</c>.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            UpdatePlaylistComboBox();
        }

        /// <summary>
        /// Populates ComboBox: <c>playListCB</c> with database playlists' names.
        /// </summary>
        public void UpdatePlaylistComboBox()
        {
            var context = App.Context;
            var playlist_names = context.DBPlaylists
                                        .ToList()
                                        .Select(p => p.Name)
                                        .ToList();
            playlistsCB.ItemsSource = playlist_names;
        }

        /// <summary>
        /// Updates ListBox: <c>videoListBox</c> with given list of videos.
        /// </summary>
        /// <param name="videos">List of videos</param>
        public void UpdateVideoListBox(List<VideoData> videos)
        {
            videoListBox.Items.Clear();
            foreach (var video in videos)
            {
                videoListBox.Items.Add(new ListBoxItem(video.title, video.thumbnail_url));
            }
        }

        /// <summary>
        /// Update TextBlock: <c>playlistInfo</c> with given playlist info.
        /// </summary>
        /// <param name="playlist_id">Playlist id</param>
        public void UpdatePlaylistInfoTextBlock(int playlist_id)
        {
            var context = App.Context;
            if (context.DBVideos.Where(x => x.DBPlaylistID == playlist_id).Any())
            {
                var duration = context.DBVideos
                    .Where(x => x.DBPlaylistID == playlist_id)
                    .Sum(x => x.Duration);
                var count = context.DBVideos.
                    Where(x => x.DBPlaylistID == playlist_id)
                    .Count();
                playlistInfo.Text = "Videos count: " + count.ToString() +
                "         Playlist duration: " + (duration / 60).ToString() + " min.";
            }
            else // empty playlist
            {
                playlistInfo.Text = "Videos count: " + 0.ToString() +
                "         Playlist duration: " + 0.ToString() + " min.";
            }
        }

        /// <summary>
        /// Click logic for button: <c>searchButton</c>. 
        /// Executes youtube search using search phrase (at least 5 characters)
        /// and max results (default 10) specified in app window,
        /// then updates ListBox <c>videoListBox</c> with results if search view is selected.
        /// Search phrase is read from TextBox: <c>videoPhrase</c>.
        /// Max search results is read from TextBox: <c>maxSearchResults</c> or set to 10 if box is empty.
        /// Fails and shows adequate message box if search phrase is shorter than 5 characters.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            // check if max search reults is given otherwise set to 10
            if (!UInt32.TryParse(maxSearchResults.Text, out uint max_results))
            {
                max_results = 10;
            }
            // validate search phrase input
            if (videoPhrase.Text.Length < 5)
            {
                MessageBox.Show("Search phrase is too short. Enter a least 5 characters.");
                return;
            }
            // execute youtube search and update ListBox if search view is selected
            if (YoutubeTracker.Search(videoPhrase.Text, max_results)
                && searchtRadioButton.IsChecked.Value)
            { 
                UpdateVideoListBox(YoutubeTracker.GetLoadedVideosFromSearch());
            }
        }

        /// <summary>
        /// Click logic for button: <c>playButton</c>. 
        /// Plays a selected video from ListBox in WebBrowser: <c>videoWeb</c>.
        /// Fails and shows message box If video is not selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            // check if video is selected in ListBox
            var idx = videoListBox.SelectedIndex;
            if (idx == -1)
            {
                MessageBox.Show("Error: video not selected.");
                return;
            }
            // get list of loaded videos depending on current view 
            List<VideoData> loaded_videos = new List<VideoData>();
            if (searchtRadioButton.IsChecked.Value)
            {
                loaded_videos = YoutubeTracker.GetLoadedVideosFromSearch();
            }
            else if (playlistRadioButton.IsChecked.Value)
            {
                loaded_videos = YoutubeTracker.GetLoadedVideosFromDB();
            }
            // play youtube video in WebView2
            var id = loaded_videos[idx].id;
            string html = "<html><head>" +
                "<meta content='chrome=1,IE=Edge' http-equiv='X-UA-Compatible'/>" +
                 "<iframe id='video' src= 'https://www.youtube.com/embed/{0}'" +
                "  style=\"overflow: hidden; overflow - x:hidden; overflow - y:hidden;" +
                " height: 100 %; width: 100 %; position: absolute; top: 0px; left: 0px;" +
                " right: 0px; bottom: 0px\" width='100%' height='100%' frameborder='0' " +
                "allow = \"autoplay; encrypted-media\" allowFullScreen></iframe>" +
                "<body style=\"background-color:black;\"></body>" +
                "</head></html>";
            videoWeb.CoreWebView2.NavigateToString(string.Format(html, id));
        }

        /// <summary>
        /// Click logic for button: <c>addToPlaylistButton</c>. 
        /// Adds selected video to selected playlist, search view required, updates database.
        /// Fails and shows adequate message box if:
        /// 1. Video/playlist is not selected. 
        /// 2. Not in search view.
        /// 3. Video is already in playlist.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddToPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            // check if in search view
            if (playlistRadioButton.IsChecked.Value)
            {
                MessageBox.Show("Error: can't add video from playlist view." +
                    " Switch view to search.");
                return;
            }
            // check if video is selected in ListBox
            var idx = videoListBox.SelectedIndex;
            if (idx == -1)
            {
                MessageBox.Show("Error: video not selected.");
                return;
            }
            // check if playlist is selected in ComboBox
            var selected_playlist = playlistsCB.SelectedItem as string;
            if (String.IsNullOrEmpty(selected_playlist))
            {
                MessageBox.Show("Error: playlist not selected.");
                return;
            }
            // get target playlist and target video's youtube id
            var loaded_videos = YoutubeTracker.GetLoadedVideosFromSearch();
            var context = App.Context;
            var playlist = context.DBPlaylists.Where(x => x.Name == selected_playlist)
                                              .FirstOrDefault();

            var yt_id = loaded_videos[idx].id;
            // check if video is akready in playlist
            if (context.DBVideos
                 .Where(x => (x.DBPlaylistID == playlist.ID) && (x.YTID == yt_id))
                 .Any()
                )
            { 
                MessageBox.Show("Error: video is already in playlist.");
                return;
            }
            // add video to playlist, database
            var video = new DataBase.DBVideo()
            {
                DBPlaylistID = playlist.ID,
                YTID = loaded_videos[idx].id,
                Duration = YoutubeTracker.YTDuration2seconds(loaded_videos[idx].duration),
                EmbedHTML = loaded_videos[idx].embed_html,
                ThumbnailURL = loaded_videos[idx].thumbnail_url,
                Title = loaded_videos[idx].title
            };
            context.DBVideos.Add(video);
            context.SaveChanges();
            // update ListBox if playlist view is selected
            if (playlistRadioButton.IsChecked.Value)
            {
                YoutubeTracker.LoadVideosFromDB(
                    context.DBVideos
                    .Where(x => x.DBPlaylistID == playlist.ID)
                    .ToList()
                    );
                UpdateVideoListBox(YoutubeTracker.GetLoadedVideosFromDB());
            }
            // print playlist duration and videos count
            UpdatePlaylistInfoTextBlock(playlist.ID);
        }

        /// <summary>
        /// Click logic for button: <c>removeFromPlaylistButton</c>. 
        /// Remove selected video from selected playlist, playlist view required, updates database.
        /// Fails and shows adequate message box if:
        /// 1. Video/playlist is not selected. 
        /// 2. Not in playlist view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveFromPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            // check if video is selected in ListBox
            var idx = videoListBox.SelectedIndex;
            if (idx == -1)
            {
                MessageBox.Show("Error: video not selected.");
                return;
            }
            // check if playlist is selected in ListBox
            var selected_playlist = playlistsCB.SelectedItem as string;
            if (String.IsNullOrEmpty(selected_playlist))
            {
                MessageBox.Show("Error: playlist not selected.");
                return;
            }
            // check if in playlist view
            if (searchtRadioButton.IsChecked.Value)
            {
                MessageBox.Show("Error: can't remove video from search view." +
                    " Switch view to playlist.");
                return;
            }
            // remove video from playlist, database
            var target_video_ytid = YoutubeTracker.GetLoadedVideosFromDB()[idx].id;
            var context = App.Context;
            var target_video_in_db = context.DBVideos
                                            .Where(x => x.YTID == target_video_ytid)
                                            .FirstOrDefault();
            context.DBVideos.Remove(target_video_in_db);
            context.SaveChanges();
            // update ListBox
            YoutubeTracker.GetLoadedVideosFromDB().RemoveAt(idx);
            UpdateVideoListBox(YoutubeTracker.GetLoadedVideosFromDB());
            // print playlist duration and videos count
            var playlist_id = context.DBPlaylists
                .Where(x => x.Name == selected_playlist)
                .First()
                .ID;
            UpdatePlaylistInfoTextBlock(playlist_id);
        }

        /// <summary>
        /// Click logic for button: <c>createPlaylisButton</c>. 
        /// Adds new playlist, name is read from TextBox: <c>phraseText</c>, updated database.
        /// Fails and shows adequate message box if:
        /// 1. Playlist name was not given.
        /// 2. Playlist with given name exists.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreatePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            // validate given playlist name
            var playlist_name = videoPhrase.Text;
            if (String.IsNullOrEmpty(playlist_name))
            {
                MessageBox.Show("Error: invalid playlist name." +
                    " Enter playlist name in phrase box.");
                return;
            }
            // check if playlist with given name exists
            var context = App.Context;
            if (context.DBPlaylists.Where( x => x.Name == playlist_name).Any())
            {
                MessageBox.Show("Error: invalid playlist name." +
                    " Playlist of given name already exists.");
                return;
            }
            // add playlist to database
            context.DBPlaylists.Add(new DataBase.DBPlaylist() { Name = playlist_name });
            context.SaveChanges();
            // update playlist ComboBox
            UpdatePlaylistComboBox();
        }

        /// <summary>
        /// Click logic for button: <c>deletePlaylisButton</c>. 
        /// Removes selected playlist, updates database.
        /// Fails and shows message box if playlist is not selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeletePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            // check if playlist is selected
            var selected_playlist = playlistsCB.SelectedItem as string;
            if (String.IsNullOrEmpty(selected_playlist))
            {
                MessageBox.Show("Error: playlist not selected.");
                return;
            }
            // delete playlist, update database
            var context = App.Context;
            var delete_target = context.DBPlaylists
                                       .Where(x => x.Name == selected_playlist)
                                       .FirstOrDefault();
            context.DBPlaylists.Remove(delete_target);
            context.SaveChanges();
            // update playlist ComboBox
            UpdatePlaylistComboBox();
        }

        /// <summary>
        /// Input validation for TextBox: <c>maxSearchResults</c>.
        /// Allows only numeric values from range [1, 50].
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // check if input is numeric
            Regex regex = new Regex("[^0-9]+");
            e.Handled = maxSearchResults.Text.Length >= 2 || regex.IsMatch(e.Text);
            // check if input is in range [1, 50]
            if (UInt32.TryParse(maxSearchResults.Text + e.Text, out uint num))
            {
                if (num >= 50)
                {
                    e.Handled = true;
                    maxSearchResults.Text = "50";
                } else if (num == 0)
                {
                    e.Handled = true;
                    maxSearchResults.Text = "1";
                } 
            }
        }

        /// <summary>
        /// Check logic for RadioButton: <c>searchRadioButton</c>.
        /// Switches view to search.
        /// Displays search results in ListBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchRB_Checked(object sender, RoutedEventArgs e)
        {
            // show search videos in ListBox
            UpdateVideoListBox(YoutubeTracker.GetLoadedVideosFromSearch());
        }

        /// <summary>
        /// Check logic for RadioButton: <c>playlistRadioButton</c>.
        /// Switches view to playlist. 
        /// Displays selected playlist videos in ListBox.
        /// Fails and shows message box if playlist is not selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaylistRB_Checked(object sender, RoutedEventArgs e)
        {
            // check if playlist is selected
            var selected_playlist = playlistsCB.SelectedItem as string;
            if (String.IsNullOrEmpty(selected_playlist))
            {
                MessageBox.Show("Error: playlist not selected.");
                playlistRadioButton.IsChecked = false;
                searchtRadioButton.IsChecked = true;
                return;
            }
            // load playlist videos
            var context = App.Context;
            var playlist_id = context.DBPlaylists
                                .Where(x => x.Name == selected_playlist)
                                .FirstOrDefault()
                                .ID;
            YoutubeTracker.LoadVideosFromDB(
                 context.DBVideos
                .Where(x => x.DBPlaylistID == playlist_id)
                .ToList()
                );
            // show playlist videos in ListBox
            UpdateVideoListBox(YoutubeTracker.GetLoadedVideosFromDB());
        }

        /// <summary>
        /// Change logic for ComboBox: <c>playlistCB</c>.
        /// Loads and shows (in playlist view) playlist videos when new playist is selected.
        /// Prints playlist videos count and duration in minutes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlaylistComboBox_Changed(object sender, SelectionChangedEventArgs e)
        {
            // load selected playlist videos
            var context = App.Context;
            var playlist_name = playlistsCB.SelectedItem as string;
            if (String.IsNullOrEmpty(playlist_name)) return;
            var playlist_id = context.DBPlaylists
                .Where(x => x.Name == playlist_name)
                .First()
                .ID;
            // check if in playlist view
            if (playlistRadioButton.IsChecked.Value)
            {
                YoutubeTracker.LoadVideosFromDB(
                    context.DBVideos
                    .Where(x => x.DBPlaylistID == playlist_id)
                    .ToList()
                );
                // update video ListBox
                UpdateVideoListBox(YoutubeTracker.GetLoadedVideosFromDB());
            }
            // print playlist duration and videos count
            UpdatePlaylistInfoTextBlock(playlist_id);
        }
    }
}
