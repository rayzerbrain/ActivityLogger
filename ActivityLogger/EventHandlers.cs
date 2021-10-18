using System;
using System.Collections.Generic;
using System.Diagnostics;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;

namespace ActivityLogger
{
    public class EventHandlers
    {
        private readonly PluginMain plugin;
        public EventHandlers(PluginMain plugin) => this.plugin = plugin;
        public void OnWaitingForPlayers()
        {
            plugin.LoadDataFile();
            
            //runs cleaning method if conditions are met
            double limit = plugin.Config.Wipe_Limit;
             //takes value of max file size, if current file size is greater, then it hard cleans. If not, if soft cleans
            if (limit != -1 && plugin.fileSize / 1000000.0 > limit)
            {
                Log.Warn("Activity data file has reached size limit. Cleaning data...");
                RunCleaning(true);
            }
            else RunCleaning(false);
        }
        public void OnVerified(VerifiedEventArgs ev)
        {
            DateTime now = DateTime.UtcNow;
            //If player has never played on server and he is fine with being tracked, he is added to the list
            if (!plugin.ActivityDict.ContainsKey(ev.Player.UserId)&&!ev.Player.DoNotTrack)
            {
                new PlayerActivity(ev.Player);
            }
            //gets the log of the player    
            PlayerActivity loggedPlayer = plugin.ActivityDict[ev.Player.UserId];
            //if name is unrecognized, it is added to the players' list of aliases
            if (!loggedPlayer.Nickname.Equals(ev.Player.Nickname) && !loggedPlayer.Aliases.Contains(ev.Player.Nickname))
            {
                loggedPlayer.Aliases.Add(ev.Player.Nickname);
            }
            //checks if there is a LoggedTimeOnDay for today, makes one if there isn't
            bool loggedToday = false;
            DateTime dt;
            foreach (string record in loggedPlayer.LoggedTimeOnDay.Keys)
            {
                dt = PlayerActivity.StringToDate(record);
                if (dt.Date.Equals(now.Date))
                {
                    loggedToday = true;
                    break;
                }
            }
            if (!loggedToday) loggedPlayer.LoggedTimeOnDay.Add(now.ToShortDateString(), loggedPlayer.HoursPlayed);
        }
        public void OnRoundStarted()
        {
            plugin.SaveDataFile();
            Timing.RunCoroutine(LogTimeCoroutine());
        }
        public void OnRoundEnded(RoundEndedEventArgs ev) 
        {
            plugin.SaveDataFile();
        }
        
        //Method for cleaning player logs to free space, used in cleaan command and on loading, if file is large enough
        public void RunCleaning(bool isHardClean)
        {
            int maxLogs = PlayerActivity.MaxLogs;
            List<string> toRemoveList = new List<string>();
            List<DateTime> toCleanList = new List<DateTime>();
            //cleans data and removes logs of players that are more than x days old, where x is the max amount of logs players can have

            //enumerates through main dict and selects players that need logs cleaned or need to be removed completely
            DateTime dt;
            foreach (KeyValuePair<string, PlayerActivity> plyrRecord in plugin.ActivityDict)
            {
                foreach (string record in plyrRecord.Value.LoggedTimeOnDay.Keys)
                {
                    dt = PlayerActivity.StringToDate(record);
                    if(DateTime.UtcNow.Subtract(dt).TotalDays > maxLogs) toCleanList.Add(dt);
                }
                foreach (DateTime log in toCleanList) plyrRecord.Value.LoggedTimeOnDay.Remove(log.ToShortDateString());
                toCleanList.Clear();
                if(isHardClean)
                {
                    //removes the player from file completely if doesn't have the min amount of hours and hasn't played in the last however many days
                    if(plyrRecord.Value.HoursPlayed < plugin.Config.Min_Hours && plyrRecord.Value.LoggedTimeOnDay.Count == 0)
                    {
                        toRemoveList.Add(plyrRecord.Key);
                    }
                }
            }
            foreach (string playerID in toRemoveList) plugin.ActivityDict.Remove(playerID);
            plugin.SaveDataFile();
        }
        //increases time for players and saves the data at certain intervals
        //doesn't run on the waiting for players screen
        public IEnumerator<float> LogTimeCoroutine()
        {
            const float secs = 20f;
            const int saveEvery = 12; //every iteration is 20 seconds, this makes it save every 12*20 seconds, or 4 minutes
            Stopwatch sw = new Stopwatch();
            for (int i=0; Round.IsStarted; i++)
            {
                //not more efficient, but updates player time more accurately by adding time in between iterations
                
                sw.Restart();
                yield return Timing.WaitForSeconds(secs);
                sw.Stop();
                //if the player is present in the list, adds the interval in hours to total. then saves the data
                foreach (Player plyr in Player.List)
                {
                    if(!plyr.DoNotTrack) plugin.ActivityDict[plyr.UserId].HoursPlayed += (float)sw.Elapsed.TotalHours;
                }
                if (i % saveEvery == 0) plugin.SaveDataFile();
            }
        }
    }
}
