using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Text.RegularExpressions;


namespace YouTubeTracker
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdatePlaylistComboBox();
        }

        public void UpdatePlaylistComboBox()
        {
            var context = App.Context;
            var playlist_names = context.DBPlaylists
                                        .ToList()
                                        .Select(p => p.Name)
                                        .ToList();
            playlistsCB.ItemsSource = playlist_names;
        }

        public void UpdateVideoListBox(List<VideoData> videos)
        {
            videoListBox.Items.Clear();
            foreach (var video in videos)
            {
                videoListBox.Items.Add(new ListBoxItem(video.title, video.thumbnail_url));
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            uint max_results;
            if (!UInt32.TryParse(maxSearchResults.Text, out max_results))
            {
                max_results = 10;
            }
            
            if (videoPhrase.Text.Length < 5)
            {
                MessageBox.Show("Search phrase is too short. Enter a least 5 characters.");
                return;
            }
            if (YoutubeTracker.Search(videoPhrase.Text, max_results)
                && searchtRadioButton.IsChecked.Value)
            { 
                UpdateVideoListBox(YoutubeTracker.GetLoadedVideosFromSearch());
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            var idx = videoListBox.SelectedIndex;
            if (idx == -1)
            {
                MessageBox.Show("Error: video not selected.");
                return;
            }

            List<VideoData> loaded_videos = new List<VideoData>();
            if (searchtRadioButton.IsChecked.Value)
            {
                loaded_videos = YoutubeTracker.GetLoadedVideosFromSearch();
            }
            else if (playlistRadioButton.IsChecked.Value)
            {
                loaded_videos = YoutubeTracker.GetLoadedVideosFromDB();
            }
            var embed_html = loaded_videos[idx].embed_html;
            var id = loaded_videos[idx].id;

            string html = "<html><head>" +
                "<meta content='chrome=1,IE=Edge' http-equiv='X-UA-Compatible'/>" +
                embed_html.Insert(embed_html.IndexOf("//www"), "https:") +
                "<body style=\"background-color:black;\"></body>" +
                "</head></html>";
            videoWeb.NavigateToString(string.Format(html, id));
        }

        private void AddToPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            if (playlistRadioButton.IsChecked.Value)
            {
                MessageBox.Show("Error: can't add video from playlist view." +
                    " Switch view to search.");
                return;
            }
            var idx = videoListBox.SelectedIndex;
            if (idx == -1)
            {
                MessageBox.Show("Error: video not selected.");
                return;
            }
            var selected_playlist = playlistsCB.SelectedItem as string;
            if (String.IsNullOrEmpty(selected_playlist))
            {
                MessageBox.Show("Error: playlist not selected.");
                return;
            }

            var loaded_videos = YoutubeTracker.GetLoadedVideosFromSearch();
            var context = App.Context;
               
            var playlist = context.DBPlaylists.Where(x => x.Name == selected_playlist)
                                              .FirstOrDefault();

            var yt_id = loaded_videos[idx].id; 
            if (context.DBVideos
                 .Where(x => (x.DBPlaylistID == playlist.ID) && (x.YTID == yt_id))
                 .Any()
                )
            { 
                MessageBox.Show("Error: video is already in playlist.");
                return;
            }

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

            if (playlistRadioButton.IsChecked.Value)
            {
                YoutubeTracker.LoadVideosFromDB(
                    context.DBVideos
                    .Where(x => x.DBPlaylistID == playlist.ID)
                    .ToList()
                    );
                UpdateVideoListBox(YoutubeTracker.GetLoadedVideosFromDB());
            }
        }

        private void RemoveFromPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            var idx = videoListBox.SelectedIndex;
            if (idx == -1)
            {
                MessageBox.Show("Error: video not selected.");
                return;
            }
            var selected_playlist = playlistsCB.SelectedItem as string;
            if (String.IsNullOrEmpty(selected_playlist))
            {
                MessageBox.Show("Error: playlist not selected.");
                return;
            }
            if (searchtRadioButton.IsChecked.Value)
            {
                MessageBox.Show("Error: can't remove video from search view." +
                    " Switch view to playlist.");
                return;
            }

            var target_video_ytid = YoutubeTracker.GetLoadedVideosFromDB()[idx].id;
            var context = App.Context;
            var target_video_in_db = context.DBVideos
                                            .Where(x => x.YTID == target_video_ytid)
                                            .FirstOrDefault();
            context.DBVideos.Remove(target_video_in_db);
            context.SaveChanges();

            YoutubeTracker.GetLoadedVideosFromDB().RemoveAt(idx);
            UpdateVideoListBox(YoutubeTracker.GetLoadedVideosFromDB());
        }

        private void AddPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            var playlist_name = videoPhrase.Text;
            if (String.IsNullOrEmpty(playlist_name))
            {
                MessageBox.Show("Error: invalid playlist name." +
                    " Enter playlist name in phrase box.");
                return;
            }

            var context = App.Context;
            if (context.DBPlaylists.Where( x => x.Name == playlist_name).Any())
            {
                MessageBox.Show("Error: invalid playlist name." +
                    " Playlist of given name already exists.");
                return;
            }

            context.DBPlaylists.Add(new DataBase.DBPlaylist() { Name = playlist_name });
            context.SaveChanges();

            UpdatePlaylistComboBox();
        }

        private void RemovePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            var selected_playlist = playlistsCB.SelectedItem as string;
            if (String.IsNullOrEmpty(selected_playlist))
            {
                MessageBox.Show("Error: playlist not selected.");
                return;
            }

            var context = App.Context;
            var delete_target = context.DBPlaylists
                                       .Where(x => x.Name == selected_playlist)
                                       .FirstOrDefault();
            context.DBPlaylists.Remove(delete_target);
            context.SaveChanges();

            UpdatePlaylistComboBox();
        }


        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = maxSearchResults.Text.Length >= 2 || regex.IsMatch(e.Text);
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

        private void SearchRB_Checked(object sender, RoutedEventArgs e)
        {
            UpdateVideoListBox(YoutubeTracker.GetLoadedVideosFromSearch());
        }

        private void PlaylistRB_Checked(object sender, RoutedEventArgs e)
        {
            var selected_playlist = playlistsCB.SelectedItem as string;
            if (String.IsNullOrEmpty(selected_playlist))
            {
                MessageBox.Show("Error: playlist not selected.");
                playlistRadioButton.IsChecked = false;
                searchtRadioButton.IsChecked = true;
                return;
            }

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
            UpdateVideoListBox(YoutubeTracker.GetLoadedVideosFromDB());
        }

        private void PlaylistComboBox_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (!playlistRadioButton.IsChecked.Value) return;

            var context = App.Context;
            var playlist_name = playlistsCB.SelectedItem as string;
            var playlist_id = context.DBPlaylists
                                     .Where(x => x.Name == playlist_name)
                                     .First()
                                     .ID;
            YoutubeTracker.LoadVideosFromDB(context.DBVideos
                                                   .Where(x => x.DBPlaylistID == playlist_id)
                                                   .ToList()
                                                   );
            UpdateVideoListBox(YoutubeTracker.GetLoadedVideosFromDB());
        }
    }
}
