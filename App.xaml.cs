using System.Windows;

namespace YouTubeTracker
{
    /// <summary>
    /// Logic interaction for class App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Database context used for accesing database.
        /// </summary>
        private static DataBase.YTrackerDBContext DBContext { get; set; }

        /// <summary>
        /// Returns database context <c>DBContext</c>.
        /// </summary>
        public static DataBase.YTrackerDBContext Context
        {
            get
            {
                if (DBContext == null)
                {
                    DBContext = new DataBase.YTrackerDBContext();
                }
                return DBContext;
            }
        }
    }
}
