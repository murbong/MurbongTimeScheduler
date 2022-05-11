using System;
using System.Windows.Controls;

namespace MurbongTimeScheduler.Views
{
    /// <summary>
    /// TimePicker.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TimePicker : UserControl
    {
        private TimeSpan timeSpanValue;
        public TimeSpan? TimeSpanValue
        {
            get => timeSpanValue;
            set
            {
                timeSpanValue = value.Value;
                HourTxt.Text = timeSpanValue.Hours.ToString("00");
                MinuteTxt.Text = timeSpanValue.Minutes.ToString("00");
            }
        }
        public TimePicker()
        {
            InitializeComponent();
        }

        private void HourTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            TimeSpan temp = timeSpanValue;
            int hour = int.Parse(txt.Text);
            hour = Math.Min(Math.Max(0, hour), 23);
            txt.Text = hour.ToString("0");

            timeSpanValue = new TimeSpan(int.Parse(txt.Text), temp.Minutes, temp.Seconds);
        }

        private void MinuteTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txt = sender as TextBox;
            TimeSpan temp = timeSpanValue;
            int min = int.Parse(txt.Text);
            min = Math.Min(Math.Max(0, min), 59);
            txt.Text = min.ToString("0");
            timeSpanValue = new TimeSpan(temp.Hours, min, temp.Seconds);
        }
    }
}
