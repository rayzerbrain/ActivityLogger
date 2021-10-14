using CommandSystem;
using System;
using System.Collections.Generic;
using Exiled.Permissions.Extensions;
using System.Text;

namespace ActivityLogger.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class ActivityOf : ICommand
    {
        public string Command { get; set; } = "activityof";

        public string[] Aliases { get; set; } = { "aof" };

        public string Description { get; set; } = "Returns the activity of the player(s) with a specfied nickname.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("al.get"))
            {
                response = "Error, you do not have permission to use this command";
                return false;
            }
            
            if (PluginMain.Instance.ActivityDict.Count==0)
            {
                response = "Error, there are no recorded players. Try restarting your server";
                return false;
            }

            //No arguments returns a leaderboard
            if (arguments.Count==0)
            {
                StringBuilder leaderboard = new StringBuilder();
                leaderboard.Append("Showing Leaderboard: ");
                
                List<PlayerActivity> topPlayers = new List<PlayerActivity>();
                
                //unused algorithm, may or may not be more efficient, but keeping just in case
                /*double maxHours;
                PlayerActivity maxHourPlayer;
                for (int i=0;i<PluginMain.Instance.Config.Leaderboard_Length;i++)
                {
                    maxHours = 0;
                    maxHourPlayer = new PlayerActivity();
                    foreach (PlayerActivity plyrRecord in PluginMain.Instance.PlayerActivityDictionary.Values)
                    {
                        if(plyrRecord.HoursPlayed>maxHours&&!topPlayers.Contains(plyrRecord))
                        {
                            maxHourPlayer = plyrRecord;
                            maxHours = maxHourPlayer.HoursPlayed;
                        }
                    }
                    topPlayers.Add(maxHourPlayer);
                }*/

                //fills the topPlayers list with dummy values
                for(int i = 0;i<PluginMain.Instance.Config.Leaderboard_Length;i++)
                {
                    topPlayers.Add(new PlayerActivity());
                }
                foreach (PlayerActivity plyrRecord in PluginMain.Instance.ActivityDict.Values)
                {
                    foreach (PlayerActivity topPlayer in topPlayers)
                    {
                        //if hours of the topplayer is less than the current plyrRecord,
                        //it inserts where the topPlayer was and moves everything down, and deletes last player
                        if (topPlayer.HoursPlayed<plyrRecord.HoursPlayed)
                        {
                            topPlayers.Insert(topPlayers.IndexOf(topPlayer), plyrRecord);
                            topPlayers.RemoveAt(topPlayers.Count - 1);
                            break;
                        }
                    }
                }
                const int deciPlaces = 2;
                
                for (int i=0;i<topPlayers.Count;i++)
                {
                    leaderboard.Append( $"\n{i + 1}: {topPlayers[i].Nickname} ({Math.Round(topPlayers[i].HoursPlayed, deciPlaces)} hours played)");
                }
                response = leaderboard.ToString();
                return true;
            }
            
            StringBuilder input = new StringBuilder();
            foreach(string arg in arguments) input.Append (arg + " ");

            const int maxShownNames = 20;
            string inputString = input.ToString().Substring(0, input.Length - 1).ToLower();
            bool aliasFound = false;
            Dictionary<string, PlayerActivity> foundPlayers = new Dictionary<string, PlayerActivity>();
            List<string> possibleNames = new List<string>();
            List<string> lowerAliases = new List<string>();

            foreach (KeyValuePair<string, PlayerActivity> record in PluginMain.Instance.ActivityDict)
            {
                if (record.Value.Aliases.Count != 0)
                {
                    lowerAliases.Clear();
                    foreach (string alias in record.Value.Aliases) lowerAliases.Add(alias.ToLower());
                }
                //to not be case sensitive of course
                if (record.Key.Equals(inputString) || record.Value.Nickname.ToLower().Equals(inputString))
                {
                    foundPlayers.Add(record.Key, record.Value);
                }
                else if (lowerAliases.Contains(inputString))
                {
                    possibleNames.Add(record.Value.Nickname);
                    aliasFound = true;
                    lowerAliases.Clear();
                }
                else if (record.Value.Nickname.ToLower().Contains(inputString) && possibleNames.Count < maxShownNames)
                {
                    possibleNames.Add(record.Value.Nickname);
                }
            }
            //default response
            StringBuilder result = new StringBuilder("No players found.");
            if (foundPlayers.Count!=0)
            {
                result.Clear();
                //In the case of multiple players of same name, returns all
                foreach (KeyValuePair<string, PlayerActivity> plyrRecord in foundPlayers)
                {
                    result.Append( $"\nPLAYER {plyrRecord.Value.Nickname} FOUND OUT OF " +
                        $"{PluginMain.Instance.ActivityDict.Count} RECORDED PLAYERS WITH ID OF" +
                        $" {plyrRecord.Key}.\n\nDATA: {plyrRecord.Value.GetInfo()}");
                }
            }

            if (possibleNames.Count != 0)
            {
                //If the original nickname the data has stored is different from the one they are using now, it notifies that
                if(aliasFound) result.Append("\nNOTE: One or more of the following players have gone by this name before");
                result.Append(" \n\nYou could've meant: ");
                
                foreach (string name in possibleNames)
                {
                    result.Append(name+", ");
                }
            }
            else
            {
                result.Append(" \n\nNo similar names found.");
            }
            response = result.ToString();
            return true;
        }
    }
}
