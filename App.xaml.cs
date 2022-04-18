using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace YouTubeTracker
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static DataBase.YTrackerDBContext DBContext { get; set; }

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
