using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityLogger
{
    public class PlayerActivity
    {
        public PlayerActivity()
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
        public PlayerActivity(Player plyr)
        {
            Nickname = plyr.Nickname;
            FirstJoin = DateTime.UtcNow.ToShortDateString();
            PluginMain.Instance.ActivityDict.Add(plyr.UserId, this);
        }

        public bool IncompleteData()
        {
            if(DateTime.UtcNow.Subtract(FindEarliestLog()).TotalDays < PluginMain.Instance.Config.Days_Counted) return true;
            return false;
        }
        //returns hours in the past x days, x is determined by config
        //if no record can be found earlier than x days ago, flags data as incomplete
        public float HoursPreviousDays()
        {
            if (LoggedTimeOnDay.Count == 0) return 0;
            DateTime now = DateTime.UtcNow;
            DateTime targetDate = DateTime.MinValue;
            DateTime dt;
            foreach (string record in LoggedTimeOnDay.Keys)
            {
                Log.Info(record);
                dt = StringToDate(record);
                TimeSpan targetTimeSpan = now.Subtract(targetDate);
                if (now.Subtract(dt).TotalDays < targetTimeSpan.TotalDays)
                {
                    targetDate = dt;
                    if(targetTimeSpan.TotalDays < PluginMain.Instance.Config.Days_Counted) break; 
                }
            }
            Log.Info(targetDate.ToShortDateString());
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
                    {aliasData.Append(alias + ", "); }
                aliasData.Append(")");
            }
            //if earliest player log isn't older than x days ago, data shows that too

            string data = $"Player {Nickname}{aliasData} has played {Math.Round(HoursPlayed, decimalPlaces)}" +
                $" hours total on this server, and {Math.Round(HoursPreviousDays(), decimalPlaces)}" +
                $" hours in the last {PluginMain.Instance.Config.Days_Counted} days. Player first joined on {FirstJoin}" +
                $", and has played {LoggedTimeOnDay.Count} days out of the last {MaxLogs} days.  \n";
            //executes if data isn't found from player more than x days ago
            if (IncompleteData())
            {
                int days = PluginMain.Instance.Config.Days_Counted;
                DateTime log = FindEarliestLog();
                completeDataInfo = $"Note: Data in past {MaxLogs} days may be incomplete. This could be because" +
                    $": Player's most recent log is less than {days} days ago,  or the plugin has only recently been " +
                    $"collecting data. Earliest available data for this player: {log.ToShortDateString()}";
            }
            return data + completeDataInfo;
        }

        //finds DateTime of earliest log
        public DateTime FindEarliestLog()
        {
            DateTime log = DateTime.MaxValue;
            DateTime dt;
            foreach (string record in LoggedTimeOnDay.Keys)
            {
                dt = StringToDate(record);
                if (dt.Ticks < log.Ticks) log = dt;
            }
            return log;
        }

        public static int MaxLogs 
        { get { return (int)(PluginMain.Instance.Config.Days_Counted * 1.5); }  }
        public static DateTime StringToDate(string str)
        {
            string[] strPieces = str.Split('/');
            int month = int.Parse(strPieces[0]);
            int day = int.Parse(strPieces[1]);
            int year = int.Parse(strPieces[2]);
            return new DateTime(year, month, day);
        }
    }
}
