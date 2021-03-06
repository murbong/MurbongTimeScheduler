using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MurbongTimeScheduler
{
    public class ScheduleDB
    {
        public Dictionary<WorkType, List<Schedule>> Schedules { get; set; }
        public ScheduleDB()
        {
            Schedules = new Dictionary<WorkType, List<Schedule>>
            {
                { WorkType.None, new List<Schedule>() },
                { WorkType.Week, new List<Schedule>() },
                { WorkType.Month, new List<Schedule>() },
                { WorkType.WeekofMonth, new List<Schedule>() }
            };
        }
        public void Save(string filename = "save")
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
            }
        }
        public void Read(string filename)
        {
            if (!File.Exists(filename))
            {
                return;
            }

            using (StreamReader sw = new StreamReader(filename))
            {

                ScheduleDB obj = JsonConvert.DeserializeObject<ScheduleDB>(sw.ReadToEnd());
                Schedules = obj.Schedules;
            }
        }
    }
    public class Schedule
    {
        public Guid ID { get; set; }
        public WorkType WorkType { get; set; }
        public DateTime Date { get; set; }
        public DayOfWeek Week => Date.DayOfWeek;
        public int Day => Date.Day;
        public int WeekNumber => Global.GetWeekNumber(Date);
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Title { get; set; }
        public bool AlarmCount { get; set; }
        public bool IsDone => (Date.Date < DateTime.Now.Date) ||
                  (Date.Date <= DateTime.Now.Date) && EndTime < DateTime.Now.TimeOfDay;
        public bool IsInProgress => StartTime < DateTime.Now.TimeOfDay && EndTime > DateTime.Now.TimeOfDay && DateTime.Now.Date == Date.Date;
        public Schedule(Guid id, DateTime date, TimeSpan startTime, TimeSpan endTime, string title)
        {
            ID = id;
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
            Title = title;
        }
        public void WakeAlarmApplication()
        {
            if (IsDone) {return; }
            if (AlarmCount == false)
            {
                AlarmCount = true;
                Global.ScheduleDB.Save("save");

                new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)
                .AddText($"스케줄 {Title} 알림입니다.").AddButton(
                    new ToastButton().
                    SetContent("알람 끄기").
                    AddArgument("action", "off").SetBackgroundActivation()
                    ).
                    SetToastDuration(ToastDuration.Long)
                .Show();

            }
        }
    }
}
