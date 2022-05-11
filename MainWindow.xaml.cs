using MurbongTimeScheduler.Views;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MurbongTimeScheduler
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public DateTime CurrentDate { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            CurrentDate = DateTime.Now;
            Global.ScheduleDB.Read("save");
            SetDate(CurrentDate);
        }

        public void SetDate(DateTime date)
        {
            YearnMonth.Text = string.Format("{0}-{1}", date.Year, date.Month);
            MonthView.MonthAction.Invoke(CurrentDate);
        }
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CalendarPopup.IsOpen = true;
        }
        private void CalendarControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void CalendarControl_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Calendar date = sender as Calendar;
            CurrentDate = date.SelectedDate.Value;
            SetDate(CurrentDate);
            CalendarPopup.IsOpen = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CurrentDate = CurrentDate.AddMonths(-1);
            SetDate(CurrentDate);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CurrentDate = CurrentDate.AddMonths(1);
            SetDate(CurrentDate);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("hello");
            Environment.Exit(0);
        }
    }
}
