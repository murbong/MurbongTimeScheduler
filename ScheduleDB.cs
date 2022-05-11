using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Media;

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
                { WorkType.Month, new List<Schedule>() }
            };
        }
        public void Save(string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
            }
        }
        public void Read(string filename)
        {
            if (!File.Exists(filename)) return;
            using (StreamReader sw = new StreamReader(filename))
            {

                var obj = JsonConvert.DeserializeObject<ScheduleDB>(sw.ReadToEnd());
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
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Title { get; set; }
        public bool AlarmCount { get; set; }
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
            if (!IsDone && AlarmCount == false)
            {
                AlarmCount = true;

                new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)
                .AddText($"스케줄 {Title} 알림입니다.").AddButton(
                    new ToastButton().
                    SetContent("알람 끄기").
                    AddArgument("action", "off").SetBackgroundActivation())
                .Show();

            }
        }
    }
}
