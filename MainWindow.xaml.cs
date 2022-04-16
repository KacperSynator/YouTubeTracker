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
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (YoutubeTracker.Search(videoPhrase.Text, 10))
            {
                foreach (var video in YoutubeTracker.GetLoadedVideos())
                {
                    videoListBox.Items.Add(new ListBoxItem(video.title, video.thumbnail_url));
                }
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            var idx = videoListBox.SelectedIndex;
            if (idx == -1) return;

            var loaded_videos = YoutubeTracker.GetLoadedVideos();
            var embed_html = loaded_videos[videoListBox.SelectedIndex].embed_html;
            var id = loaded_videos[videoListBox.SelectedIndex].id;

            string html = "<html><head>" +
                "<meta content='chrome=1,IE=Edge' http-equiv='X-UA-Compatible'/>" +
                embed_html.Insert(embed_html.IndexOf("//www"), "https:") +
                "<body style=\"background-color:black;\"></body>" +
                "</head></html>";
            videoWeb.NavigateToString(string.Format(html, id));
        }

        private void videoPhrase_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
