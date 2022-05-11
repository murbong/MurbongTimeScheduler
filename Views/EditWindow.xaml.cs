using System.Windows;

namespace MurbongTimeScheduler.Views
{
    /// <summary>
    /// EditWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EditWindow : Window
    {
        public Schedule EditSchedule;
        public EditWindow()
        {
            InitializeComponent();
        }
        public EditWindow(Schedule schedule) : this()
        {
            EditSchedule = schedule;
            TitleTxt.Text = EditSchedule.Title;
            StartTimePicker.TimeSpanValue = EditSchedule.StartTime;
            EndTimePicker.TimeSpanValue = EditSchedule.EndTime;
            AlarmCount.IsChecked = EditSchedule.AlarmCount;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EditSchedule.StartTime = StartTimePicker.TimeSpanValue.Value;
            EditSchedule.EndTime = EndTimePicker.TimeSpanValue.Value;
            EditSchedule.Title = TitleTxt.Text;
            EditSchedule.AlarmCount = AlarmCount.IsChecked.Value;
            DialogResult = true;
            Global.ScheduleDB.Save("save");
            Close();
        }
    }
}
