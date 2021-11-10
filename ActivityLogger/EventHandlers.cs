using System;
using System.Collections.Generic;
using System.Diagnostics;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using ActivityLogger.API;

namespace ActivityLogger
{
    public class EventHandlers
    {
        //I definitely did not copy this
        private readonly PluginMain plugin;
        public EventHandlers(PluginMain plugin) => this.plugin = plugin;
        public void OnWaitingForPlayers()
        {
            plugin.LoadDataFile();
            
            //runs cleaning method if conditions are met
            double limit = plugin.Config.WipeLimit;
             //takes value of max file size, if current file size is greater, then it hard cleans. If not, if soft cleans
            if (limit != -1 && plugin.fileSize / 1000000.0 > limit)
            {
                Log.Warn("Activity data file has reached size limit. Cleaning data...");
                RunCleaning(true);
            }
            else RunCleaning(false);
            CheckServerLogs();
        }
        public void OnVerified(VerifiedEventArgs ev)
        {
            if (ev.Player.DoNotTrack) return;
            DateTime now = DateTime.UtcNow;
            //If player has never played on server and he is fine with being tracked, he is added to the list
            if (!plugin.ActivityDict.ContainsKey(ev.Player.UserId))
            {
                new ActivityRecord(ev.Player.Nickname, ev.Player.UserId);
            }
            //gets the log of the player    
            ActivityRecord loggedPlayer = plugin.ActivityDict[ev.Player.UserId];
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
                dt = DateTime.Parse(record);
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
            int maxLogs = ActivityRecord.MaxLogs;
            List<string> toRemoveList = new List<string>();
            List<DateTime> toCleanList = new List<DateTime>();
            //cleans data and removes logs of players that are more than x days old, where x is the max amount of logs players can have

            //enumerates through main dict and selects players that need logs cleaned or need to be removed completely
            DateTime dt;
            foreach (KeyValuePair<string, ActivityRecord> plyrRecord in plugin.ActivityDict)
            {
                foreach (string record in plyrRecord.Value.LoggedTimeOnDay.Keys)
                {
                    dt = DateTime.Parse(record);
                    if(DateTime.UtcNow.Subtract(dt).TotalDays > maxLogs) toCleanList.Add(dt);
                    //this is for previous versions which recorded logs with format 10/02/2021 instead of 10/2/201
                    //for any previous versions of the plugin this is needed to reformat them to not cause KeyNotFoundExceptions
                    int daysIndex = record.IndexOf("/");
                    if (record.Substring(daysIndex + 1, 1).Equals("0"))
                    {
                        record.Remove(3, 1);
                    }
                }
                
                foreach (DateTime log in toCleanList) plyrRecord.Value.LoggedTimeOnDay.Remove(log.ToShortDateString());
                toCleanList.Clear();
                if(isHardClean)
                {
                    //removes the player from file completely if doesn't have the min amount of hours and hasn't played in the last however many days
                    if(plyrRecord.Value.HoursPlayed < plugin.Config.MinHours && plyrRecord.Value.LoggedTimeOnDay.Count == 0)
                    {
                        toRemoveList.Add(plyrRecord.Key);
                    }
                }
            }
            foreach (string playerID in toRemoveList) plugin.ActivityDict.Remove(playerID);
            plugin.SaveDataFile();
        }
        public void CheckServerLogs()
        {
            //checks if server log needs new specific logs to be made and the total hours to be updated along side them
            //only makes a new log every {periodLength} days
            int periodLength = PluginMain.Instance.Config.DataPeriodLength;
            ActivityRecord serverLog = PluginMain.Instance.serverLog;
            DateTime now = DateTime.UtcNow;

            if (now.Subtract(PluginMain.Instance.FirstLogDate).TotalDays < periodLength) return;

            //returns if any already existing log made by the server isn't older than {periodLength} days ago
            DateTime serverLogDate = DateTime.MinValue;
            foreach(string log in serverLog.LoggedTimeOnDay.Keys)
            {
                serverLogDate = DateTime.Parse(log);
                if (now.Subtract(serverLogDate).TotalDays < periodLength) return;
            }

            DateTime dateFloor;
            DateTime dateCeiling;
            //"if" clause runs when no logs for server are found
            if (serverLogDate.Equals(DateTime.MinValue))
            {
                dateFloor = now.AddDays(-ActivityRecord.MaxLogs);
                //if the date of the first log is not longer ago than the oldest date a player log could be, it becomes the first date floor
                //else, the date floor remains as the farthest back the logs can go, which is (now-maxLogs)
                if (plugin.FirstLogDate.CompareTo(dateFloor) > 0) dateFloor = plugin.FirstLogDate;
                serverLog.FirstJoin = dateFloor.ToShortDateString();
            }
            else dateFloor = serverLogDate.AddDays(1);
            dateCeiling = dateFloor.AddDays(periodLength);

            int logsToMake = 0;
            try { logsToMake = (int)now.Subtract(dateFloor).TotalDays / periodLength; }
            catch (DivideByZeroException) { Log.Error("Your Period_Length configuration is set to zero. Please change it to a positive number"); }

            for(int i=0;i<logsToMake;i++)
            {
                MakeServerLog(dateFloor, dateCeiling);
                dateFloor = dateFloor.AddDays(periodLength);
                dateCeiling = dateCeiling.AddDays(periodLength);
            }
        }
        //makes a new specific log for the server record, this stores total unique player count for the specific period
        //also adds hours of everyone within the time period (inclusive, inclusive)
        public void MakeServerLog(DateTime startDate, DateTime endDate)
        {
            float startHours;
            float endHours = 0;
            float totalHours = 0;
            int playerCount = 0;
            DateTime logDate;
            foreach(ActivityRecord playerRecord in PluginMain.Instance.ActivityDict.Values)
            {
                //goes through logs of each player and records the first and last logs within the period of time
                //adds hours played in the time period by the player to total played time on the server
                //also increments playerCount if player played within the time period
                startHours = -1;
                foreach (KeyValuePair<string, float> log in playerRecord.LoggedTimeOnDay)
                {
                    logDate = DateTime.Parse(log.Key);
                    if (startHours == -1 && logDate.CompareTo(startDate) >= 0 && logDate.CompareTo(endDate)<=0)
                    {
                        startHours = log.Value;
                        playerCount++;
                    }
                    endHours = log.Value;
                    if (logDate.CompareTo(endDate) >= 0) break;
                }
                if(startHours!=-1)totalHours += endHours - startHours; 
            }
            PluginMain.Instance.serverLog.LoggedTimeOnDay.Add(endDate.ToShortDateString(), playerCount);
            PluginMain.Instance.serverLog.HoursPlayed += totalHours;
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
