using Exiled.API.Features;
using System;
using System.Collections.Generic;

namespace ActivityLogger
{
    public class PlayerActivity
    {
        //I don't know why I need this but I do
        public PlayerActivity() { }
        
        //dictionaries are cool
        public Dictionary<DateTime, double> LoggedTimeOnDay { get; set; } = new Dictionary<DateTime, double>();
        
        public string SteamName { get; set; }
        public List<string> NameAliases { get; set; } = new List<string>();
        public double HoursPlayed { get; set; } = 0;
        //possible identifiers to look into:
        //Player player.NetworkIndentity (is type Mirror.NetworkIndentity
        //Player player.Conenction (is type Mirror.NetworkConnection
        //Player player.
        //Im mega stupid its literally player.UserId
        //-_-
        
        public string Identifier { get; set; }
        
        public DateTime FirstJoin { get; set; }


        public bool IncompleteData()
        {
            if (DateTime.UtcNow.Subtract(this.EarliestLog()).TotalDays < PluginMain.Instance.Config.Days_Previous_Amount)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //returns hours in the past x days, x is determined by config
        //if no record can be found earlier than x days ago, flags data as incomplete
        public double HoursPreviousDays()
        {
            TimeSpan minimumTime = new TimeSpan(PluginMain.Instance.Config.Days_Previous_Amount, 0, 0);
            DateTime targetDate = new DateTime(1, 1, 1);
            if(LoggedTimeOnDay.Count==0)
            {
                return 0;
            }
            foreach (KeyValuePair<DateTime, double> record in LoggedTimeOnDay)
            {
                if(DateTime.UtcNow.Subtract(record.Key).TotalDays< DateTime.UtcNow.Subtract(targetDate).TotalDays)
                {
                    targetDate = record.Key;
                    if (DateTime.UtcNow.Subtract(targetDate).TotalDays<minimumTime.TotalDays)
                    {
                        break;
                    }
                    
                }

            }
            
            return HoursPlayed-LoggedTimeOnDay[targetDate];
        }
        //This is what is shown when using the activityof command
        public string GetInfo()
        {
            double unused = this.HoursPreviousDays();
            const int decimalPlaces = 3;
            string aliasData = "";
            string completeDataInfo = "";
            //if player has gone by other names, data shows that
            if(this.NameAliases.Count!=0)
            {
                aliasData = "(has gone by: ";
                foreach (string alias in this.NameAliases)
                { aliasData += alias + ", "; }
                aliasData += ")";
            }
            //if earliest player log isn't older than x days ago, data shows that too
            
            string data = "Player " + this.SteamName+ $" ({this.Identifier}) " + aliasData + " has played " +
                Math.Round(this.HoursPlayed, decimalPlaces) + " hours total on this server, and " +
                Math.Round(this.HoursPreviousDays(), decimalPlaces) + " hours in the last " +
                PluginMain.Instance.Config.Days_Previous_Amount + $" days. Player first joined on {this.FirstJoin} \n";
            if (this.IncompleteData())
            {
                int days = PluginMain.Instance.Config.Days_Previous_Amount;
                int maxDays = PluginMain.Instance.Config.Player_Max_Logs;
                DateTime earliestLog = this.EarliestLog();
                completeDataInfo = $"Note: Data in past {days} days may " +
                    $"be incomplete. This could be because: Player has not played in between {days} and {maxDays} days ago" +
                    $" or the plugin has only recently been collecting data. Earliest available data for this player: {earliestLog}";
            }
            return data + completeDataInfo;
        }
        
        //finds DateTime of earliest log
        public DateTime EarliestLog()
        {
            DateTime earliestLog = DateTime.MaxValue;
            foreach (KeyValuePair<DateTime, double> rcrd in this.LoggedTimeOnDay)
            {
                if (rcrd.Key.Ticks < earliestLog.Ticks)
                { earliestLog = rcrd.Key; }
            }
            return earliestLog;
        }
        //constructor, intializes user id, nickname, and date of first join
        //the nickname present when player joins for first time is the only name that will work in the aof command, but entering aliases will result in the original nickname being shown
        public PlayerActivity(Player plyr)
        {
            this.Identifier = plyr.UserId;
            this.SteamName = plyr.Nickname;
            this.FirstJoin = DateTime.Now;
            PluginMain.Instance.PlayerActivityDictionary.Add(Identifier, this);
        }
        //call me a redditor for typing "this" when I dont need to
    }
}
