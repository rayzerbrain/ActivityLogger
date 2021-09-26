using Exiled.API.Features;
using Exiled.API.Extensions;
using System;
using System.Collections.Generic;
using static System.Math;

namespace ActivityLogger
{
    public class PlayerActivity
    {
        public PlayerActivity() { }
        //dictionaries are cool
        

        public Dictionary<DateTime, double> LoggedTimeOnDay { get; set; } = new Dictionary<DateTime, double>();
        
        //I think this is specific to each player?
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
        
        public DateTime StartSessionTime { get; set; }

        public double HoursPreviousDays()
        {
            TimeSpan minimumTime = new TimeSpan(PluginMain.Instance.Config.Days_Previous_Amount, 0, 0);
            DateTime targetDate = new DateTime(1, 1, 1);
            foreach (KeyValuePair<DateTime, double> record in LoggedTimeOnDay)
            {
                if(DateTime.Now.Subtract(record.Key).TotalDays< DateTime.Now.Subtract(targetDate).TotalDays)
                {
                    targetDate = record.Key;
                    if (DateTime.Now.Subtract(targetDate).TotalDays<minimumTime.TotalDays)
                    {
                        break;
                    }
                    
                }
            }
            return HoursPlayed-LoggedTimeOnDay[targetDate];
        }
        public void ShortenRecord()
        {
            while(LoggedTimeOnDay.Count>PluginMain.Instance.Config.Player_Max_Logs)
            {
                DateTime maxTime = new DateTime(0);
                foreach (DateTime time in LoggedTimeOnDay.Keys)
                {
                    if(time.Ticks>maxTime.Ticks)
                    {
                        maxTime = time;
                    }
                }
                LoggedTimeOnDay.Remove(maxTime);
            }
        }
        public void StartLog()
        {
            this.StartSessionTime = DateTime.Now;
        }
        public void EndLog()
        {
            
            this.HoursPlayed += DateTime.Now.Subtract(this.StartSessionTime).TotalHours;
            this.StartSessionTime = new DateTime(0);
        }
        public string GetInfo()
        {
            const int decimalPlaces = 5;
            string sessionData;
            string aliasData = "";
            if(this.NameAliases.Count!=0)
            {
                aliasData = "(has gone by: ";
                foreach (string alias in this.NameAliases)
                { aliasData += alias + ", "; }
                aliasData += ")";
            }
            if(this.StartSessionTime.Ticks==0)
            {sessionData = "The player is not in the server right now."; }
            else
            {sessionData = "The player has played " + Math.Round(DateTime.Now.Subtract(this.StartSessionTime).TotalHours, decimalPlaces)+ " hours in his/her current session";}
            return "Player " + this.SteamName + aliasData+" has played " + Math.Round(this.HoursPlayed, 2) + " hours total on this server, and " + Math.Round(this.HoursPreviousDays(), 2) + " hours in the last " + PluginMain.Instance.Config.Days_Previous_Amount + " days. "+sessionData;
        }
        public PlayerActivity(Player plyr)
        {
            this.Identifier = plyr.UserId;
            this.SteamName = plyr.Nickname;
            PluginMain.Instance.PlayerActivityDictionary.Add(Identifier, this);
        }
        //call me a redditor for typing "this" when I dont need to
    }
}
