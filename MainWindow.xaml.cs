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

            textList.Text = String.Format("Videos:\n{0}\n\nChannels:\n{1}\n\nPlaylists:\n{2}\n\n",
                                            string.Join("\n", search_result[0].Select(x => x.Item1)),
                                            string.Join("\n", search_result[1].Select(x => x.Item1)),
                                            string.Join("\n", search_result[2].Select(x => x.Item1))
                                          );

            var id = search_result[0][0].Item2;
            string html = "<html><head>" +
                "<meta content='IE=Edge' http-equiv='X-UA-Compatible'/>" +
                "<iframe id='video' src= 'https://www.youtube.com/embed/{0}'" +
                "  style=\"overflow: hidden; overflow - x:hidden; overflow - y:hidden;" +
                " height: 100 %; width: 100 %; position: absolute; top: 0px; left: 0px;" +
                " right: 0px; bottom: 0px\" width='100%' height='100%' frameborder='0' " +
                "allow = \"autoplay; encrypted-media\" allowFullScreen></iframe>" +
                "<body style=\"background-color:black;\"></body>" +
                "</head></html>";
            this.videoWeb.NavigateToString(string.Format(html, id));
        }

        private void videoPhrase_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
