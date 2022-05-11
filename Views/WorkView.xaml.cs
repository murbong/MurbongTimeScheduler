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
    /// WorkView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WorkView : UserControl
    {
        public string Title { get; set; }
        public WorkType WorkType { get; set; }
        public DateTime Date { get; set; }
        public bool IsDone
        {
            get
            {
                return
                  (Date.Date < DateTime.Now.Date) ||
                  (Date.Date <= DateTime.Now.Date) && EndTime < DateTime.Now.TimeOfDay;
            }
        }
        public bool IsInProgress
        {
            get
            {
                return StartTime < DateTime.Now.TimeOfDay && EndTime > DateTime.Now.TimeOfDay && DateTime.Now.Date == Date.Date;
            }
        }

        private TimeSpan startTime;
        public TimeSpan StartTime
        {
            get { return startTime; }
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
            get { return endTime; }
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
            var sch = new Schedule(ID, Date, StartTime, EndTime, Title);
            Global.ScheduleDB.Schedules[WorkType.None].Add(sch);
            Global.ScheduleDB.Save("save");
            SetBackground();
        }
        private void setPos(double x)
        {
            var rat = x / Global.ratio * Global.fullWidth;
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
            var text = (sender as TextBox).Text;
            Title = text;
            Global.ScheduleDB.Schedules[WorkType].First(element => element.ID == ID).Title = text;
            Global.ScheduleDB.Save("save");

        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            DayView.RemoveWorkViewAction(this);
            Global.ScheduleDB.Save("save");

        }

        private void Edit()
        {
            var viewSchedule = Global.ScheduleDB.Schedules[WorkType].Where(element => element.ID == ID).FirstOrDefault();

            var editview = new EditWindow(viewSchedule);
            editview.Owner = Application.Current.MainWindow;
            editview.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            bool? result = editview.ShowDialog();
            if (result == true)
            {
                StartTime = viewSchedule.StartTime;
                EndTime = viewSchedule.EndTime;
                Title = viewSchedule.Title;
            }
            setPos(startTime.TotalMinutes);
            setText();
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
            if (WorkType == WorkType.Week) return;


            var viewSchedule = Global.ScheduleDB.Schedules[WorkType].Where(element => element.ID == ID).FirstOrDefault();
            Global.ScheduleDB.Schedules[WorkType].Remove(viewSchedule);
            WorkType = WorkType.Week;
            viewSchedule.WorkType = WorkType.Week;

            Global.ScheduleDB.Schedules[WorkType.Week].Add(viewSchedule);
            Global.ScheduleDB.Save("save");

        }

        private void MonthPromote_Click(object sender, RoutedEventArgs e)
        {
            if (WorkType == WorkType.Month) return;


            var viewSchedule = Global.ScheduleDB.Schedules[WorkType].Where(element => element.ID == ID).FirstOrDefault();
            Global.ScheduleDB.Schedules[WorkType].Remove(viewSchedule);
            WorkType = WorkType.Month;
            viewSchedule.WorkType = WorkType.Month;

            Global.ScheduleDB.Schedules[WorkType.Month].Add(viewSchedule);
            Global.ScheduleDB.Save("save");

        }
    }
}
