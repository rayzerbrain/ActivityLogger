using CommandSystem;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using Exiled.Permissions.Extensions;

namespace ActivityLogger.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class ActivityOf : ICommand
    {
        public string Command { get; set; } = "activityof";

        public string[] Aliases { get; set; } = { "aof" };

        public string Description { get; set; } = "Returns the activity of the player(s) with a specfied nickname." +
            " If no exact name is found it will display similar names or players that have gone by that name before.";

        //sigh, more frontend
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            //Data loaded here

            Player asker = Player.Get((CommandSender)sender);
            if (!((CommandSender)sender).CheckPermission("al.get"))
            {
                response = "Error, you do not have permission to use this command";
                return false;
            }
            List<PlayerActivity> foundPlayers = new List<PlayerActivity>();
            List<string> possibleNames = new List<string>();

            if(arguments.Count==0)
            {
                response = "Showing Leaderboard: ";
                List<PlayerActivity> topPlayers = new List<PlayerActivity>();
                for(int i=0;i<PluginMain.Instance.Config.Leaderboard_Length;i++)
                {
                    double maxHours = 0;
                    PlayerActivity maxHourPlayer = new PlayerActivity();
                    foreach (KeyValuePair<string, PlayerActivity> plyr in PluginMain.Instance.PlayerActivityDictionary)
                    {
                        if(plyr.Value.HoursPlayed>maxHours&&!topPlayers.Contains(plyr.Value))
                        {
                            maxHourPlayer = plyr.Value;
                            maxHours = maxHourPlayer.HoursPlayed;
                        }
                    }
                    if(maxHourPlayer!=null)
                    {
                        topPlayers.Add(maxHourPlayer);
                    }
                }
                for(int i=0;i<topPlayers.Count;i++)
                {
                    response += $"\n{i + 1}: {topPlayers[i].SteamName} ({Math.Round(topPlayers[i].HoursPlayed, 2)} hours played)";
                }
                return true;
            }
            string input = "";
            foreach(string arg in arguments)
            {
                input += arg + " ";

            }
            input = input.Substring(0, input.Length-1);
            foreach(KeyValuePair<string, PlayerActivity> record in PluginMain.Instance.PlayerActivityDictionary)
            {
                if((record.Value.SteamName.ToLower()).Equals(input.ToLower()))
                {
                    foundPlayers.Add(record.Value);
                }
                else if(record.Value.NameAliases.Contains(input.ToLower()))
                {
                    possibleNames.Add(record.Value.SteamName);
                }
                else if (record.Value.SteamName.ToLower().Contains(input.ToLower()) && possibleNames.Count < 20)
                {
                    possibleNames.Add(record.Value.SteamName);
                }
            }

            response = "No players found.";
            if(foundPlayers.Count!=0)
            {
                response = "";
                foreach (PlayerActivity player in foundPlayers)
                {
                    response += $"\nPLAYER {player.SteamName} FOUND OUT OF " +
                        $"{PluginMain.Instance.PlayerActivityDictionary.Count} RECORDED PLAYERS.\n\nDATA: {player.GetInfo()}";
                }
            }
            if (possibleNames.Count != 0)
            {
                response += " \n\nYou could've meant: ";
                foreach (string name in possibleNames)
                {
                    response += name+", ";
                }
            }
            else
            {
                response += " \n\nNo similar names found.";
            }
            return true;
        }
    }
}
