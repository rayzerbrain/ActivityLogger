using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityLogger.API
{
    public class ActivityRecord
    {
        public ActivityRecord()
        {
            //default constructor go brrrr
        }
        
        //dictionaries are cool
        public Dictionary<string, float> LoggedTimeOnDay { get; set; } = new Dictionary<string, float>();
        
        public string Nickname { get; set; }
        public List<string> Aliases { get; set; } = new List<string>();
        public float HoursPlayed { get; set; } = 0;
        public string FirstJoin { get; set; }

        //main constructor, intializes user id, nickname, and date of first join
        //the nickname present when player joins for first time is the only name that will work in the aof command,
        //but entering aliases will result in the original nickname being shown as an alternative
        public ActivityRecord(string name, string id)
        {
            Nickname = name;
            FirstJoin = DateTime.UtcNow.ToShortDateString();
            PluginMain.Instance.ActivityDict.Add(id, this);
        }
        public static bool IncompleteData()
        {
            DateTime loggingStartDate = PluginMain.Instance.FirstLogDate;
            return DateTime.UtcNow.Subtract(loggingStartDate).TotalDays > PluginMain.Instance.Config.DaysCounted;
        }
        //returns hours in the past x days, x is determined by config
        public float HoursPreviousDays()
        {
            if (LoggedTimeOnDay.Count == 0) return 0;
            DateTime now = DateTime.UtcNow;
            DateTime targetDate = DateTime.MinValue;
            DateTime testDate;
            foreach (string logDate in LoggedTimeOnDay.Keys)
            {
                testDate = DateTime.Parse(logDate);
                TimeSpan targetTimeSpan = now.Subtract(targetDate); 
                if (now.Subtract(testDate).TotalDays < targetTimeSpan.TotalDays)
                {
                    targetDate = testDate;
                    if(targetTimeSpan.TotalDays < PluginMain.Instance.Config.DaysCounted) break; 
                }
            }
            return HoursPlayed-LoggedTimeOnDay[targetDate.ToShortDateString()];
        }

        //This is what is shown when using the activityof command
        public string GetInfo()
        {
            const int decimalPlaces = 3;
            StringBuilder aliasData = new StringBuilder();
            string completeDataInfo = "";
            //if player has gone by other names, data shows that
            if(Aliases.Count!=0)
            {
                aliasData.Append(" (has gone by: ");
                foreach (string alias in Aliases)
                {
                    aliasData.Append(alias + ", "); 
                }
                aliasData.Append(")");
            }
            //if earliest player log isn't older than x days ago, data shows that too
            string data = $"Player {Nickname}{aliasData} has played {Math.Round(HoursPlayed, decimalPlaces)}" +
                $" hours total on this server, and {Math.Round(HoursPreviousDays(), decimalPlaces)}" +
                $" hours in the last {PluginMain.Instance.Config.DaysCounted} days. Player first joined on {FirstJoin}" +
                $", and has played {LoggedTimeOnDay.Count} days out of the last {MaxLogs} days.  \n";
            //executes if data isn't found from player more than x days ago
            if (IncompleteData())
            {
                completeDataInfo = $"Note: Data in past {MaxLogs} days may be incomplete. This could be because: Plugin " +
                    $"has only beein collecting data since {PluginMain.Instance.FirstLogDate.ToShortDateString()}. ";
            }
            return data + completeDataInfo;
        }

        //finds DateTime of earliest log
        public DateTime FindEarliestLog()
        {
            DateTime log = DateTime.MaxValue;
            DateTime dt;
            foreach (string logDate in LoggedTimeOnDay.Keys)
            {
                dt = DateTime.Parse(logDate);
                if (dt.Ticks < log.Ticks) log = dt;
            }
            return log;
        }

        public static int MaxLogs => (int)(PluginMain.Instance.Config.DaysCounted * 1.5);
    }
}
