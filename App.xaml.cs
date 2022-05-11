using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
                Interval = TimeSpan.FromSeconds(1)
            };
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var sch = Global.ScheduleDB.Schedules[WorkType.None];
            var now = DateTime.Now;

            if (sch.Count == 0) return;

            var days = sch.Where(r => r.Date.Date == now.Date && r.EndTime > now.TimeOfDay && r.AlarmCount == false);
            if (days.Count() == 0) return;

            var first = days.OrderBy(r => r.StartTime).First();
            if (first.StartTime <= now.TimeOfDay)
            {
                first.WakeAlarmApplication();
            }

            Global.WorkViews.ForEach(view => view.SetBackground());
        }



        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            ToastNotificationManagerCompat.OnActivated += toastArgs =>
            {
                var args = ToastArguments.Parse(toastArgs.Argument);

                ValueSet userinput = toastArgs.UserInput;

                Debug.WriteLine(args.Get("action").Equals("off"));
            };

        }
    }
}
