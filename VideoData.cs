using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeTracker
{
    public struct VideoData
    {
        public string id { get; set; }
        public string title { get; set; }
        public string thumbnail_url { get; set; }
        public string duration { get; set; }
        public string embed_html { get; set; }

        public VideoData(string _id)
        {
            id = _id;
            title = "";
            thumbnail_url = "";
            duration = "";
            embed_html = "";
        }
        public VideoData(string _id, string _title, string _duration, string _embed_html, string _thumbnail_url)
        {
            id = _id;
            title = _title;
            duration = _duration;
            embed_html = _embed_html;
            thumbnail_url = _thumbnail_url;
        }
    }
}
