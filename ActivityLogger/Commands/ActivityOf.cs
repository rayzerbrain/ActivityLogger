using CommandSystem;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            
       
            List<PlayerActivity> foundPlayers = new List<PlayerActivity>();
            List<string> possibleNames = new List<string>();
            List<string> rolesLowercase = new List<string>();
            bool wasBadgeHidden = asker.BadgeHidden;
            asker.BadgeHidden = false;
            foreach (string role in PluginMain.Instance.Config.Allowed_Roles)
            {
                rolesLowercase.Add(role.ToLower());
            }
            if(arguments.Count<1|| arguments.Count>1)
            {
                response = "Error, usage is: activityof [playerNickname]";
                return false;
            }
            if (!rolesLowercase.Contains(asker.RankName.ToLower()))
            {
                response = "Error, you do not have permission to use this command";
                return false;
            }
            asker.BadgeHidden = wasBadgeHidden;
            foreach(KeyValuePair<string, PlayerActivity> record in PluginMain.Instance.PlayerActivityDictionary)
            {
                if(record.Value.SteamName.ToLower().Equals(arguments.At(0).ToLower()))
                {
                    foundPlayers.Add(record.Value);
                }
                else if( record.Value.SteamName.ToLower().Contains(arguments.At(0).ToLower()) 
                    ||record.Value.NameAliases.Contains(arguments.At(0).ToLower()))
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
                    response += "\nPLAYER " + player.SteamName + " FOUND. Data will be updated when player ends session(leaves).\n\nDATA: " + player.GetInfo();
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
