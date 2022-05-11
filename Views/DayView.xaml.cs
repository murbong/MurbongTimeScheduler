using System;
using System.Collections;
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
    /// ScheduleView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DayView : UserControl
    {
        public static Action<WorkView> RemoveWorkViewAction;
        public DateTime ParentDate { get; set; }
        private bool isLabelOnly;
        public bool IsLabelOnly { get { return isLabelOnly; } set { isLabelOnly = value; Draw(); } }

        private bool drawBackground = false;
        public bool DrawBackground { get { return drawBackground; } set { drawBackground = value; Draw(); } }

        private double intervalWidth;
        private WorkView selectedworkView;
        private double anchor;
        public double IntervalWidth
        {
            get => intervalWidth;
            set
            {
                intervalWidth = value;
            } 
        }
        public DayView()
        {
            InitializeComponent();

        }

        public DayView(double intervalWidth, DateTime pd): this()
        {
            ParentDate = pd;
            IntervalWidth = intervalWidth;
            RemoveWorkViewAction += removeWorkView;
            Draw();
        }
        private void Draw()
        {
            TimeGrid.Children.Clear();
            UpdateLayout();
            var rows = 0;
            for (TimeSpan c = Global.TimeEnd; c > Global.TimeStart; c -= Global.Interval)
            {
                rows++;
            }
            Global.fullWidth = rows * IntervalWidth;
            var time = Global.TimeStart;
            var borderbrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#d280ff"));
            var background = FindResource("MyVisualBrush") as VisualBrush;
            if (IsLabelOnly)
            {
                rows++;

            }

            if (DrawBackground)
            {
                //TimeGrid.Background = background;
                TimeGrid.Width = Global.fullWidth;

            }
            else
            {
                TimeGrid.Width = Global.fullWidth+100;

            }

            for (var i = 0; i < rows; i++)
            {
                //Border 추가
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(IntervalWidth);
                TimeGrid.ColumnDefinitions.Add(col);

                if (IsLabelOnly == false)
                {
                    var border = new Border {
                        BorderBrush = borderbrush,
                        BorderThickness = (i >= rows - 1) ? new Thickness(1, 1, 1, 0) : new Thickness(1, 1, 0, 0)
                    };
                    border.SetValue(Grid.ColumnProperty, i);
                    TimeGrid.Children.Add(border);
                }
                else
                {
                    var label = new Label
                    {
                        Content = string.Format("{0:00}:{1:00}", time.Hours, time.Minutes),
                        FontFamily = new FontFamily("#MapleStory")
                    };
                    label.SetValue(Grid.ColumnProperty, i);
                    label.SetValue(Grid.RowProperty, 0);
                    label.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Left);
                    label.SetValue(VerticalAlignmentProperty, VerticalAlignment.Bottom);
                    //label.Padding = new Thickness(0, 0, 25, 0);
                    TimeGrid.Children.Add(label);
                    time = time.Add(Global.Interval);
                }
            }
        }
        private void TimeMouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(TimeGrid);
            var clamp = Math.Min(Math.Max((position.X), 0), Global.fullWidth);
            if (e.LeftButton == MouseButtonState.Pressed && selectedworkView != null && Global.EditingDayView == this)
            {
                var offset = position.X - (double)selectedworkView.GetValue(Canvas.LeftProperty);

                var time = TimeSpan.FromMinutes((int)(clamp / Global.fullWidth * Global.ratio));
                var anchorTime = TimeSpan.FromMinutes((int)(anchor / Global.fullWidth * Global.ratio));

                if (time > anchorTime)
                {
                    selectedworkView.StartTime = anchorTime;
                    selectedworkView.EndTime = time;
                }
                else
                {
                    selectedworkView.StartTime = time;
                    selectedworkView.EndTime = anchorTime;
                }
            }
            else
            {
                if(selectedworkView != null)
                {
                    if (selectedworkView.StartTime == selectedworkView.EndTime)
                    {
                        TimeCanvas.Children.Remove(selectedworkView);
                    }
                    else
                    {
                        selectedworkView.FinishPlacement();
                    }
                }
                selectedworkView = null;
                Global.EditingDayView = null;
            }
        }
        private void TimeMouseDown(object sender, MouseButtonEventArgs e)
        {
            if(IsLabelOnly == true)
            {
                return;
            }
            Point position = e.GetPosition(TimeGrid);
            anchor = position.X;
            if (Global.EditingDayView == null)
            {
                Global.EditingDayView = this;
            }
            if (e.LeftButton == MouseButtonState.Pressed && Global.EditingDayView == this && position.X < Global.fullWidth)
            {
                //시작지점 선택
                var clamp = Math.Min(Math.Max((position.X / Global.fullWidth), 0), 1);
                var time = TimeSpan.FromMinutes((int)(clamp * Global.ratio));
                var TempWork = new WorkView(ParentDate);
                TempWork.StartTime = time;
                TempWork.EndTime = time.Add(new TimeSpan(0, 15, 0));
                selectedworkView = TempWork;
                TimeCanvas.Children.Add(TempWork);
            }
        }
        public void AddWorkView(Schedule schedule)
        {
            if (IsLabelOnly==true) { return; }
            var work = new WorkView(schedule);
            TimeCanvas.Children.Add(work);
        }
        public void AddWorkViewRange(IEnumerable<Schedule> schedules)
        {
            foreach(var i in schedules)
            {
                AddWorkView(i);
            }
        }
        private void removeWorkView(WorkView workview)
        {
            if (workview != null)
            {
                var id = workview.ID;
                var work = Global.ScheduleDB.Schedules[workview.WorkType].Where(s => s.ID == id).FirstOrDefault();
                TimeCanvas.Children.Remove(workview);
                Global.ScheduleDB.Schedules[workview.WorkType].Remove(work);
                Global.WorkViews.Remove(workview);
                Global.ScheduleDB.Save("save");
            }
        }
    }
}
