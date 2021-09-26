using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;

namespace ActivityLogger
{
    public class EventHandlers
    {
        public void OnWaitingForPlayers()
        {
            PluginMain.Instance.LoadActivityData();
            
            //load data here
        }
        public void OnRoundStarted()
        {
            Timing.RunCoroutine(SaveDataCoroutine());
        }
        public void OnRoundEnded(RoundEndedEventArgs ev)
        {
            PluginMain.Instance.SaveActivityData();
            //save and unload data here
        }
        public void OnEndingRound(EndingRoundEventArgs ev)
        {
        }
        public void OnRestartingRound()
        {
            PluginMain.Instance.SaveActivityData();
        }
        public void OnVerified(VerifiedEventArgs ev)
        {
            if(!PluginMain.Instance.PlayerActivityDictionary.ContainsKey(ev.Player.UserId))
            {
                new PlayerActivity(ev.Player);
            }
            PlayerActivity loggedPlayer = PluginMain.Instance.PlayerActivityDictionary[ev.Player.UserId];
            if(!loggedPlayer.SteamName.Equals(ev.Player.Nickname)&&!loggedPlayer.NameAliases.Contains(ev.Player.Nickname))
            {
                loggedPlayer.NameAliases.Add( ev.Player.Nickname);
            }
            loggedPlayer.StartLog();
            loggedPlayer.LoggedTimeOnDay.Add(DateTime.Now, loggedPlayer.HoursPlayed);
            if(loggedPlayer.LoggedTimeOnDay.Count>PluginMain.Instance.Config.Player_Max_Logs)
            {
                loggedPlayer.ShortenRecord();
            }
        }
        public void OnLeft(LeftEventArgs ev)
        {
            PluginMain.Instance.PlayerActivityDictionary[ev.Player.UserId].EndLog();
        }
        public void OnBanned(BannedEventArgs ev)
        {
            PluginMain.Instance.PlayerActivityDictionary[ev.Target.UserId].EndLog();
        }
        public void OnKicked(KickedEventArgs ev)
        {
            PluginMain.Instance.PlayerActivityDictionary[ev.Target.UserId].EndLog();
        }
        public IEnumerator<float> SaveDataCoroutine()
        {
            const float secondsWaiting = 10f;
            while(Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(secondsWaiting);
                PluginMain.Instance.SaveActivityData();
                
            }
        }
    }
}
