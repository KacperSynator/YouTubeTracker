using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;

namespace YouTubeTracker
{
    public class ListBoxItem
    {
        public string text { get; set; }
        public Uri image { get; set; }
        public ListBoxItem(string _text, string image_url)
        {
            text = _text;
            image = new Uri(image_url);
        }
    }
}
