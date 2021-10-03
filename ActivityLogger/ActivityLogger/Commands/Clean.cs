using CommandSystem;
using Exiled.Permissions.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityLogger.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class Clean : ICommand
    {
        
        public string Command { get; set; } = "activityof_clean";

        public string[] Aliases { get; set; } = { "aof_clean" };

        public string Description { get; set; } = "Removes all logs of players to free storage, " +
            "but loses recent activity data as a result. (Total hours is still kept track of)";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("al.clean"))
            {
                response = "Error, you do not have permission to use this command";
                return false;
            }
            else if (arguments.Count > 1)
            {
                response = "Error, usage is aof_clean";
                return false;
            }
            else if (arguments.Count == 0)
            {
                response = "Are you sure you want to wipe all players' logs? (Total hours will still be saved)\n" +
                    "If so, type in aof_clean confirm";
                return false;
            }
            else if (arguments.At(0).Equals("confirm"))
            {
                foreach (KeyValuePair<string, PlayerActivity> plyrRecord in PluginMain.Instance.PlayerActivityDictionary)
                {
                    plyrRecord.Value.LoggedTimeOnDay.Clear();
                }
                PluginMain.Instance.SaveActivityData();
                response = "Data Cleaned.";
                return true;
            }
            else
            {
                response = "Error, usage is aof_clean confirm";
                return false;
            }
        }
    }
}
