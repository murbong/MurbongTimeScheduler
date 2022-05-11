using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        private int dayHeight = 40;
        public MonthView()
        {
            InitializeComponent();
            MonthAction += ChangeMonth;
            CurrentDate = DateTime.Now;
            Task.Run(() => renderThread());

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Draw();
            LoadScheduleDB(CurrentDate);
        }

        void renderThread()
        {
            while (true)
            {
                var date = DateTime.Now;
                try
                {
                    Dispatcher.Invoke(() => Canvas.SetLeft(CurrentTime, (60.0 * date.Hour + date.Minute) / Global.ratio * Global.fullWidth));
                }
                catch
                {

                }
            }
        }
        void ChangeMonth(DateTime month)
        {
            CurrentDate = month;
            totalDays = DateTime.DaysInMonth(CurrentDate.Year, CurrentDate.Month);
            Draw();
            LoadScheduleDB(CurrentDate);
        }
        void LoadScheduleDB(DateTime month)
        {
            var monthDatas = Global.ScheduleDB.Schedules[WorkType.None].Where(e => e.Date.Year == month.Year && e.Date.Month == month.Month);
            var weekRoutine = Global.ScheduleDB.Schedules[WorkType.Week];
            var monthRoutine = Global.ScheduleDB.Schedules[WorkType.Month];

            Debug.Print(monthDatas.Count() + "");

            var dayView = MonthGrid.Children.OfType<DayView>().Where(e => e.ParentDate.Month == month.Month);
            foreach (var i in Enumerable.Range(1, totalDays))
            {
                var DTime = new DateTime(month.Year, month.Month, i);

                var wr = weekRoutine.Where(e => e.Week == DTime.DayOfWeek);
                var mr = monthRoutine.Where(e => e.Day == DTime.Day);

                Debug.WriteLine(wr.Count());

                var view = dayView.FirstOrDefault(e => e.ParentDate.Day == i);
                var dayData = monthDatas.Where(e => e.Date.Day == i);
                view.AddWorkViewRange(wr);
                view.AddWorkViewRange(mr);
                view.AddWorkViewRange(dayData);
            }
        }
        void Draw()
        {
            MonthGrid.Children.Clear();
            MonthGrid.RowDefinitions.Clear();

            var rows = 0;
            for (TimeSpan c = Global.TimeEnd; c > Global.TimeStart; c -= Global.Interval)
            {
                rows++;
            }
            var width = ActualWidth / (rows+2);
            Debug.WriteLine("width : " + width);



            var timeView = new DayView(width, CurrentDate)
            {
                IsLabelOnly = true,
                DrawBackground = false
            };

            timeView.SetValue(Grid.ColumnProperty, 1);
            timeView.SetValue(Grid.RowProperty, 0);

            timeGrid.Children.Add(timeView);

            for (var i = 0; i < totalDays; i++)
            {
                var row = new RowDefinition();
                row.Height = new GridLength(30);

                MonthGrid.RowDefinitions.Add(row);

                var curDate = new DateTime(CurrentDate.Year, CurrentDate.Month, i + 1);
                var label = new Label { Content = string.Format("{0}일({1})", i + 1, GetWeek(curDate.DayOfWeek)), FontFamily = new FontFamily("#MapleStory") };
                label.SetValue(Grid.ColumnProperty, 0);
                label.SetValue(Grid.RowProperty, i);
                label.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Right);
                label.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
                MonthGrid.Children.Add(label);
                var day = new DayView(width, curDate)
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
