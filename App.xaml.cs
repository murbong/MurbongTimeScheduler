using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Windows.Foundation.Collections;

namespace MurbongTimeScheduler
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {

        public DispatcherTimer Timer { get; set; }
        public App()
        {
            Timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {

            List<Schedule> sch = Global.ScheduleDB.Schedules[WorkType.None];
            List<Schedule> weeksch = Global.ScheduleDB.Schedules[WorkType.Week];
            List<Schedule> monthsch = Global.ScheduleDB.Schedules[WorkType.Month];
            List<Schedule> womsch = Global.ScheduleDB.Schedules[WorkType.WeekofMonth];

            DateTime now = DateTime.Now;

            var alarmfalse = (monthsch.Concat(weeksch).Concat(womsch)).Where(r => r.EndTime < now.TimeOfDay && r.AlarmCount == true);

            foreach (var i in alarmfalse)
            {
                i.AlarmCount = false;
                Global.ScheduleDB.Save();
            }

            if (sch.Count + weeksch.Count + monthsch.Count + womsch.Count == 0)
            {
                return;
            }

            IEnumerable<Schedule> days = sch.Where(r => r.Date.Date == now.Date && r.EndTime > now.TimeOfDay && r.AlarmCount == false);
            IEnumerable<Schedule> wr = weeksch.Where(r => r.Week == now.DayOfWeek && r.EndTime > now.TimeOfDay);
            IEnumerable<Schedule> mr = monthsch.Where(r => r.Day == now.Day && r.EndTime > now.TimeOfDay);
            IEnumerable<Schedule> womr = womsch.Where(r => r.WeekNumber == Global.GetWeekNumber(now) && r.Week == now.DayOfWeek && r.EndTime > now.TimeOfDay);
            if (days.Count() + wr.Count() + mr.Count() + womr.Count() == 0)
            {
                return;
            }

            var all = days.Concat(wr).Concat(mr).Concat(womr);

            Schedule first = all.OrderBy(r => r.StartTime).First();

            if (first.StartTime <= now.TimeOfDay)
            {
                first.WakeAlarmApplication();
            }


        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                ToastArguments args = ToastArguments.Parse(toastArgs.Argument);

                ValueSet userinput = toastArgs.UserInput;
            };

        }
    }
}
