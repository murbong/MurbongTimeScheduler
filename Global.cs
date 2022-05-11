using MurbongTimeScheduler.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace MurbongTimeScheduler
{
    public enum WorkType
    {
        None,
        Week,
        Month
    }

    public static class Global
    {
        public static DayView EditingDayView { get; set; }
        public static List<WorkView> WorkViews { get; set; }
        public static ScheduleDB ScheduleDB { get; set; }
        public static TimeSpan Interval { get; set; }
        public static TimeSpan TimeStart { get; set; }
        public static TimeSpan TimeEnd { get; set; }
        public static int ratio = 1440;
        public static double fullWidth = 0;
        static Global()
        {
            Interval = new TimeSpan(3, 0, 0);
            TimeStart = new TimeSpan(0, 0, 0);
            TimeEnd = new TimeSpan(1, 0, 0, 0);
            ScheduleDB = new ScheduleDB();
            WorkViews = new List<WorkView>();
        }
    }
}
