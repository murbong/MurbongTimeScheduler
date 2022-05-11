using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;



namespace MurbongTimeScheduler.Views
{
    /// <summary>
    /// WorkView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WorkView : UserControl
    {
        public string Title { get; set; }
        public WorkType WorkType { get; set; }
        public DateTime Date { get; set; }
        public bool IsDone => (Date.Date < DateTime.Now.Date) ||
                  (Date.Date <= DateTime.Now.Date) && EndTime < DateTime.Now.TimeOfDay;
        public bool IsInProgress => StartTime < DateTime.Now.TimeOfDay && EndTime > DateTime.Now.TimeOfDay && DateTime.Now.Date == Date.Date;

        private TimeSpan startTime;
        public TimeSpan StartTime
        {
            get => startTime;
            set
            {
                startTime = value;
                setWidth();
                setPos(startTime.TotalMinutes);
                setText();
            }
        }

        private TimeSpan endTime;
        public TimeSpan EndTime
        {
            get => endTime;
            set
            {
                endTime = value;
                setPos(startTime.TotalMinutes);
                setWidth(); setText();
            }
        }

        public bool Clickable { get; set; }

        public readonly Guid ID;

        public WorkView()
        {
            ID = Guid.NewGuid();
            Clickable = true;
            InitializeComponent();
            Global.WorkViews.Add(this);
            System.Globalization.Calendar calendarCalc = CultureInfo.CurrentCulture.Calendar;
        }
        public WorkView(DateTime date) : this()
        {
            Date = date;
        }
        public WorkView(Schedule schedule)
        {
            ID = schedule.ID;
            WorkType = schedule.WorkType;
            Date = schedule.Date;
            StartTime = schedule.StartTime;
            EndTime = schedule.EndTime;
            Title = schedule.Title;
            Clickable = true;
            Global.WorkViews.Add(this);
            InitializeComponent();
            setText();
            SetBackground();
        }
        public void SetBackground()
        {

        }
        public void FinishPlacement()
        {
            Schedule sch = new Schedule(ID, Date, StartTime, EndTime, Title);
            Global.ScheduleDB.Schedules[WorkType.None].Add(sch);
            Global.ScheduleDB.Save("save");
            SetBackground();
        }
        private void setPos(double x)
        {
            double rat = x / Global.ratio * Global.fullWidth;
            Canvas.SetLeft(this, rat);
            Canvas.SetTop(this, 1);
        }
        private void setWidth()
        {
            Width = Math.Abs((EndTime - StartTime).TotalMinutes / Global.ratio * Global.fullWidth);
        }
        private void setText()
        {
            timeLabel?.Dispatcher.Invoke(() =>
            {
                timeLabel.Text = string.Format("{0:00}:{1:00} ~ {2:00}:{3:00}",
                StartTime.Hours, StartTime.Minutes,
                EndTime.Hours, EndTime.Minutes);
            });
            txt_Title?.Dispatcher.Invoke(() =>
            {
                txt_Title.Text = Title;
            });
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = (sender as TextBox).Text;
            Title = text;
            Global.ScheduleDB.Schedules[WorkType].First(element => element.ID == ID).Title = text;
            Global.ScheduleDB.Save("save");

        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            DayView.RemoveWorkViewAction(this);
            Global.ScheduleDB.Save("save");
            if (WorkType != WorkType.None)
            {
                Global.ReloadEvent.Invoke(Date);
            }
        }

        private void Edit()
        {
            Schedule viewSchedule = Global.ScheduleDB.Schedules[WorkType].Where(element => element.ID == ID).FirstOrDefault();

            EditWindow editview = new EditWindow(viewSchedule)
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            bool? result = editview.ShowDialog();
            if (result == true)
            {
                StartTime = viewSchedule.StartTime;
                EndTime = viewSchedule.EndTime;
                Title = viewSchedule.Title;
            }
            setPos(startTime.TotalMinutes);
            setText();
            if (WorkType != WorkType.None)
            {
                Global.ReloadEvent.Invoke(Date);
            }
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            Edit();
        }

        private void backgroundBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Edit();
            }
        }

        private void WeekPromote_Click(object sender, RoutedEventArgs e)
        {
            if (WorkType == WorkType.Week)
            {
                return;
            }

            Schedule viewSchedule = Global.ScheduleDB.Schedules[WorkType].Where(element => element.ID == ID).FirstOrDefault();
            Global.ScheduleDB.Schedules[WorkType].Remove(viewSchedule);
            WorkType = WorkType.Week;
            viewSchedule.WorkType = WorkType.Week;

            Global.ScheduleDB.Schedules[WorkType.Week].Add(viewSchedule);
            Global.ScheduleDB.Save("save");

            Global.ReloadEvent.Invoke(Date);

        }

        private void MonthPromote_Click(object sender, RoutedEventArgs e)
        {
            if (WorkType == WorkType.Month)
            {
                return;
            }

            Schedule viewSchedule = Global.ScheduleDB.Schedules[WorkType].Where(element => element.ID == ID).FirstOrDefault();
            Global.ScheduleDB.Schedules[WorkType].Remove(viewSchedule);
            WorkType = WorkType.Month;
            viewSchedule.WorkType = WorkType.Month;

            Global.ScheduleDB.Schedules[WorkType.Month].Add(viewSchedule);
            Global.ScheduleDB.Save("save");

            Global.ReloadEvent.Invoke(Date);

        }

        private void WeekOfMonth_Click(object sender, RoutedEventArgs e)
        {
            if (WorkType == WorkType.WeekofMonth)
            {
                return;
            }

            Schedule viewSchedule = Global.ScheduleDB.Schedules[WorkType].Where(element => element.ID == ID).FirstOrDefault();
            Global.ScheduleDB.Schedules[WorkType].Remove(viewSchedule);
            WorkType = WorkType.WeekofMonth;
            viewSchedule.WorkType = WorkType.WeekofMonth;

            Global.ScheduleDB.Schedules[WorkType.WeekofMonth].Add(viewSchedule);
            Global.ScheduleDB.Save("save");

            Global.ReloadEvent.Invoke(Date);

        }
    }
}
