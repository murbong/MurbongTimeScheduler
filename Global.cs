using MurbongTimeScheduler.Views;
using System;
using System.Collections.Generic;
using System.Globalization;


namespace MurbongTimeScheduler
{
    public enum WorkType
    {
        None,
        Week,
        Month,
        WeekofMonth
    }

    public static class Global
    {
        public static int GetWeekNumber(DateTime args)
        {
            var firstDate = new DateTime(args.Year, args.Month, 1);
            var offset = -1;
            if(firstDate.DayOfWeek < DayOfWeek.Friday)
            {
                offset = 0;
            }

            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(args, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) -
            CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(firstDate, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1+offset;
        }

        public static DayView EditingDayView { get; set; }
        public static List<WorkView> WorkViews { get; set; }
        public static ScheduleDB ScheduleDB { get; set; }
        public static TimeSpan Interval { get; set; }
        public static TimeSpan TimeStart { get; set; }
        public static TimeSpan TimeEnd { get; set; }
        public static int ratio = 1440;
        public static double fullWidth = 0;
        public static Action<DateTime> ReloadEvent;
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
