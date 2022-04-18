using System;


namespace YouTubeTracker
{
    /// <summary>
    /// Class <c>ListBoxItem</c> models an item of custom ListBox.
    /// </summary>
    public class ListBoxItem
    {
        /// <summary>
        /// Text shown in TextBlock.
        /// </summary>
        public string text { get; set; }
        /// <summary>
        /// Image displayed in Image.
        /// </summary>
        public Uri image { get; set; }
        /// <summary>
        /// Constructor for creating new item from text string and image url string.
        /// </summary>
        /// <param name="_text">Text</param>
        /// <param name="image_url">Image url</param>
        public ListBoxItem(string _text, string image_url)
        {
            text = _text;
            image = new Uri(image_url);
        }
    }
}
