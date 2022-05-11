using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MurbongTimeScheduler.Views
{

    /// <summary>
    /// MonthView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MonthView : UserControl
    {
        public string GetWeek(DayOfWeek week)
        {
            switch (week)
            {
                case DayOfWeek.Sunday:
                    return "일";
                case DayOfWeek.Monday:
                    return "월";
                case DayOfWeek.Tuesday:
                    return "화";
                case DayOfWeek.Wednesday:
                    return "수";
                case DayOfWeek.Thursday:
                    return "목";
                case DayOfWeek.Friday:
                    return "금";
                case DayOfWeek.Saturday:
                    return "토";
                default:
                    return string.Empty;
            }
        }

        public static Action<DateTime> MonthAction;
        public DateTime CurrentDate { get; set; }

        private int totalDays;
        private readonly int dayHeight = 40;
        public MonthView()
        {
            InitializeComponent();
            MonthAction += ChangeMonth;
            Global.ReloadEvent += ChangeMonth;
            CurrentDate = DateTime.Now;
            Task.Run(() => renderThread());

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Draw();
            LoadScheduleDB(CurrentDate);
        }

        private void renderThread()
        {
            while (true)
            {
                DateTime date = DateTime.Now;
                try
                {
                    Dispatcher.Invoke(() => Canvas.SetLeft(CurrentTime, (60.0 * date.Hour + date.Minute) / Global.ratio * Global.fullWidth));
                }
                catch
                {

                }
            }
        }

        private void ChangeMonth(DateTime month)
        {
            CurrentDate = month;
            totalDays = DateTime.DaysInMonth(CurrentDate.Year, CurrentDate.Month);
            Draw();
            LoadScheduleDB(CurrentDate);
        }

        private void LoadScheduleDB(DateTime month)
        {
            IEnumerable<Schedule> monthDatas = Global.ScheduleDB.Schedules[WorkType.None].Where(e => e.Date.Year == month.Year && e.Date.Month == month.Month);
            List<Schedule> weekRoutine = Global.ScheduleDB.Schedules[WorkType.Week];
            List<Schedule> monthRoutine = Global.ScheduleDB.Schedules[WorkType.Month];
            var wom = Global.ScheduleDB.Schedules[WorkType.WeekofMonth];



            IEnumerable<DayView> dayView = MonthGrid.Children.OfType<DayView>().Where(e => e.ParentDate.Month == month.Month);
            foreach (int i in Enumerable.Range(1, totalDays))
            {
                DateTime DTime = new DateTime(month.Year, month.Month, i);


                IEnumerable<Schedule> wr = weekRoutine.Where(e => e.Week == DTime.DayOfWeek);
                IEnumerable<Schedule> mr = monthRoutine.Where(e => e.Day == DTime.Day);
                IEnumerable<Schedule> womr = wom.Where(e => e.WeekNumber == Global.GetWeekNumber(DTime) && e.Week == DTime.DayOfWeek);

                DayView view = dayView.FirstOrDefault(e => e.ParentDate.Day == i);
                IEnumerable<Schedule> dayData = monthDatas.Where(e => e.Date.Day == i);
                view.AddWorkViewRange(wr);
                view.AddWorkViewRange(mr);
                view.AddWorkViewRange(womr);
                view.AddWorkViewRange(dayData);
            }
        }

        private void Draw()
        {
            MonthGrid.Children.Clear();
            MonthGrid.RowDefinitions.Clear();

            int rows = 0;
            for (TimeSpan c = Global.TimeEnd; c > Global.TimeStart; c -= Global.Interval)
            {
                rows++;
            }
            double width = ActualWidth / (rows + 2);



            DayView timeView = new DayView(width, CurrentDate)
            {
                IsLabelOnly = true,
                DrawBackground = false
            };

            timeView.SetValue(Grid.ColumnProperty, 1);
            timeView.SetValue(Grid.RowProperty, 0);

            timeGrid.Children.Add(timeView);

            for (int i = 0; i < totalDays; i++)
            {
                RowDefinition row = new RowDefinition
                {
                    Height = new GridLength(30)
                };

                MonthGrid.RowDefinitions.Add(row);

                DateTime curDate = new DateTime(CurrentDate.Year, CurrentDate.Month, i + 1);
                Label label = new Label { Content = string.Format("{0}일({1}){2}주차", i + 1, GetWeek(curDate.DayOfWeek), Global.GetWeekNumber(curDate)), FontFamily = new FontFamily("#MapleStory") };
                label.SetValue(Grid.ColumnProperty, 0);
                label.SetValue(Grid.RowProperty, i);
                label.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Right);
                label.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
                MonthGrid.Children.Add(label);
                DayView day = new DayView(width, curDate)
                {
                    IsLabelOnly = false,
                    DrawBackground = true

                };
                day.SetValue(Grid.ColumnProperty, 1);
                day.SetValue(Grid.RowProperty, i);
                day.Height = dayHeight;
                MonthGrid.Children.Add(day);
            }
            CurrentTime.Height = dayHeight * totalDays;
        }


    }
}
