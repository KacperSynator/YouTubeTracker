namespace YouTubeTracker
{
    /// <summary>
    /// Struct <c>VideoData</c> models youtube video's data.
    /// </summary>
    public struct VideoData
    {
        /// <summary>
        /// Youtube vido id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// Title of video
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// Thumbnai url, image
        /// </summary>
        public string thumbnail_url { get; set; }
        /// <summary>
        /// Duration of video
        /// </summary>
        public string duration { get; set; }
        /// <summary>
        /// Videos's embed html for playing a video in web browser.
        /// </summary>
        public string embed_html { get; set; }

        /// <summary>
        /// Constructs a new instance using only id, rest set to empty string.
        /// </summary>
        /// <param name="_id">Video's youtube id</param>
        public VideoData(string _id)
        {
            id = _id;
            title = "";
            thumbnail_url = "";
            duration = "";
            embed_html = "";
        }

        /// <summary>
        /// Full parameter constructor
        /// </summary>
        /// <param name="_id">Video's youtube id</param>
        /// <param name="_title">Video's title</param>
        /// <param name="_duration">Video's duration</param>
        /// <param name="_embed_html">Video's embed html</param>
        /// <param name="_thumbnail_url">Video's thumbnail url</param>
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
