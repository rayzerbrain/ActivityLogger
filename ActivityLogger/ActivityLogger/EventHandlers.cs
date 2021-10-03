using System;
using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;

namespace ActivityLogger
{
    public class EventHandlers
    {
        public void OnWaitingForPlayers()
        {
            Timing.RunCoroutine(LoadDataCoroutine());
            
        }
        public void OnRoundStarted()
        {
            Timing.RunCoroutine(SaveDataCoroutine());
        }
        public void OnVerified(VerifiedEventArgs ev)
        {
            //If player has never played on server, he is added to the list
            if(!PluginMain.Instance.PlayerActivityDictionary.ContainsKey(ev.Player.UserId))
            {
                new PlayerActivity(ev.Player);
            }
            //gets the log of the player
            PlayerActivity loggedPlayer = PluginMain.Instance.PlayerActivityDictionary[ev.Player.UserId];
            //if name is unrecognized, it is added to the players' list of aliases
            if(!loggedPlayer.SteamName.Equals(ev.Player.Nickname)&&!loggedPlayer.NameAliases.Contains(ev.Player.Nickname))
            {
                loggedPlayer.NameAliases.Add( ev.Player.Nickname);
            }
            //checks if there is a LoggedTimeOnDay for today. If so, then a new one is not made. If not, a new one is made
            bool loggedTodayAlready = false;
            foreach(KeyValuePair<DateTime, double> record in loggedPlayer.LoggedTimeOnDay)
            {
                if(record.Key.Date.Equals(DateTime.Now.Date))
                {
                    loggedTodayAlready = true;
                    break;
                }
                
            }
            if(!loggedTodayAlready)
            {
                loggedPlayer.LoggedTimeOnDay.Add(DateTime.Now, loggedPlayer.HoursPlayed);
            }
        }
        //increases time for players and saves the data at certain intervals
        //doesn't run on the waiting for players screen
        public IEnumerator<float> SaveDataCoroutine()
        {
            const float secondsWaiting = 20f;
            for(; ; )
            {
                yield return Timing.WaitForSeconds(secondsWaiting);
                //if the player is present in the list, adds the interval in hours to total. then saves the data
                foreach(Player plyr in Player.List)
                {
                    if(PluginMain.Instance.PlayerActivityDictionary.ContainsKey(plyr.UserId))
                    {
                        PluginMain.Instance.PlayerActivityDictionary[plyr.UserId].HoursPlayed += (secondsWaiting / 3600);
                    }
                }
                PluginMain.Instance.SaveActivityData();

                if(!Round.IsStarted)
                {
                    break;
                }
            }
        }
        public IEnumerator<float> LoadDataCoroutine()
        {
            PluginMain.Instance.LoadActivityData();
            yield return Timing.WaitForSeconds(5f);
            //cleans data and removes logs of players that are more than x days old, where x is the max amount of logs players can have
            foreach (KeyValuePair<string, PlayerActivity> plyrRecord in PluginMain.Instance.PlayerActivityDictionary)
            {
                foreach (KeyValuePair<DateTime, double> log in plyrRecord.Value.LoggedTimeOnDay)
                {
                    if (DateTime.UtcNow.Subtract(log.Key).TotalDays > PluginMain.Instance.Config.Player_Max_Logs)
                    {
                        plyrRecord.Value.LoggedTimeOnDay.Remove(log.Key);
                    }
                }
            }
        }
    }
}
