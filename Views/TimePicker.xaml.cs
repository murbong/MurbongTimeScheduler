using System;
using System.Collections.Generic;
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
    /// TimePicker.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TimePicker : UserControl
    {
        private TimeSpan timeSpanValue;
        public TimeSpan? TimeSpanValue
        {
            get
            {
                return timeSpanValue;
            }
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
            var txt = sender as TextBox;
            var temp = timeSpanValue;
            var hour = int.Parse(txt.Text);
            hour = Math.Min(Math.Max(0, hour), 23);
            txt.Text = hour.ToString("0");

            timeSpanValue = new TimeSpan(int.Parse(txt.Text), temp.Minutes, temp.Seconds);
        }

        private void MinuteTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = sender as TextBox;
            var temp = timeSpanValue;
            var min = int.Parse(txt.Text);
            min = Math.Min(Math.Max(0, min), 59);
            txt.Text = min.ToString("0");
            timeSpanValue = new TimeSpan(temp.Hours, min, temp.Seconds);
        }
    }
}
