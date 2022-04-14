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

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            var search_result = new YoutubeSearch().Search(videoPhrase.Text, 10);

            if (search_result == null) return;

            textList.Text = String.Format("Videos:\n{0}\n\nChannels:\n{1}\n\nPlaylists:\n{2}\n\n",
                                            string.Join("\n", search_result.videos.Select(x => x.title)),
                                            string.Join("\n", search_result.channels.Select(x => x.title)),
                                            string.Join("\n", search_result.playlists.Select(x => x.title))
                                          );

            var id = search_result.videos[0].id;
            var video_result = new YoutubeVideo().VideoList(new List<string> {id});
            if (video_result == null) return;
            //textList.Text += String.Format("Video info:\n{0}\n\n", string.Join("\n", video_result[0]));

            string html = "<html><head>" +
                "<meta content='chrome=1,IE=Edge' http-equiv='X-UA-Compatible'/>" +
                video_result.videos_data[0].embed_html.Insert(video_result.videos_data[0].embed_html.IndexOf("//www"), "https:") +
                "<body style=\"background-color:black;\"></body>" +
                "</head></html>";
            this.videoWeb.NavigateToString(string.Format(html, id));
        }

        private void videoPhrase_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
