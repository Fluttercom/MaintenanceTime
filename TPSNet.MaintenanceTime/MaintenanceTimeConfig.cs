using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlutterEffect.MaintenanceTime
{
    /// <summary>
    /// Maintenance configuration.
    /// Can contain several From-To periods.
    /// Overlapping is not supported.
    /// </summary>
    public class MaintenanceTimeConfig : List<MaintenanceTimeConfigEntry>
    {
        public DateTime GetNearestWorkingTime(DateTime current)
        {
            DateTime min = DateTime.MinValue;
            foreach (var entry in this)
            {
                if (entry.Disabled)
                    continue;
                var next = entry.GetNextWorkingTime(current);
                if (min == DateTime.MinValue || min > next)
                    min = next;
            }
            return min;
        }

        public DateTime GetNearestMaintTime(DateTime current)
        {
            DateTime min = DateTime.MinValue;
            foreach (var entry in this)
            {
                if (entry.Disabled)
                    continue;
                var next = entry.GetNextMaintTime(current);
                if (min == DateTime.MinValue || min > next)
                    min = next;
            }
            return min;
        }
    }

    public class MaintenanceTimeConfigEntry
    {
        public WeekdayTimeEntry From { get; set; }
        public WeekdayTimeEntry To { get; set; }
        public bool Disabled { get; set; }

        public MaintenanceTimeConfigEntry() {  }

        /// <summary>
        /// Create entry from string in format "Weekday, Timespan"
        /// </summary>
        public MaintenanceTimeConfigEntry(string from ,string to)
        {
            From = new WeekdayTimeEntry(from);
            To = new WeekdayTimeEntry(to);
        }
        
        /// <summary>
        /// Is given time between from and to
        /// </summary>
        public bool IsInside(DateTime current)
        {
            if (From.Weekday == null || To.Weekday == null)
            {
                if (To.Time > From.Time)
                    return current.TimeOfDay >= From.Time && current.TimeOfDay < To.Time;
                else
                    return current.TimeOfDay >= From.Time || current.TimeOfDay < To.Time;
            }
            return GetNextWorkingTime(current) < GetNextMaintTime(current);
        }

        public DateTime GetNextWorkingTime(DateTime current)
        {
            if (From.Weekday != null && To.Weekday != null)
            {
                var dt = current.Date.AddDays(-(int)current.DayOfWeek).AddDays((int)To.Weekday) + To.Time;
                if (dt < current)
                    dt = dt.AddDays(7);
                return dt;
            }
            else
            {
                return To.Time < current.TimeOfDay ? DateTime.Today.AddDays(1) + To.Time : DateTime.Today + To.Time;
            }
        }

        public DateTime GetNextMaintTime(DateTime current)
        {
            if (From.Weekday != null && To.Weekday != null)
            {
                var dt = current.Date.AddDays(-(int)current.DayOfWeek).AddDays((int)From.Weekday) + From.Time;
                if (dt < current)
                    dt = dt.AddDays(7);
                return dt;
            }
            else
            {
                return From.Time < current.TimeOfDay ? DateTime.Today.AddDays(1) + From.Time : DateTime.Today + From.Time;
            }
        }
    }

    
    public class WeekdayTimeEntry
    {
        /// <summary>
        /// Leave null for daily check
        /// </summary>
        public DayOfWeek? Weekday { get; set; }
        public TimeSpan Time { get; set; }

        public WeekdayTimeEntry() { } //empty constructor for json.bind

        /// <param name="time">Time string in timespan format</param>
        /// <param name="weekday">Optional Weekday in System.DayOfWeek format</param>
        public WeekdayTimeEntry(string time, string weekday = null)
        {
            Time = TimeSpan.Parse(time);
            if (weekday != null)
                Weekday = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), weekday);
        }

        public WeekdayTimeEntry(TimeSpan time, DayOfWeek? weekday)
        {
            Time = time;
            Weekday = weekday;
        }

        /// <summary>
        /// Create entry from string in format "Weekday, Timespan"
        /// </summary>
        public WeekdayTimeEntry(string weekAndTime)
        {
            string[] ss = weekAndTime.Split(",");
            if (ss.Length == 1)
                Time = TimeSpan.Parse(weekAndTime);
            else if (ss.Length == 2)
            {
                Time = TimeSpan.Parse(ss[1].Trim());
                Weekday = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), ss[0]);
            }
        }
    }
}
