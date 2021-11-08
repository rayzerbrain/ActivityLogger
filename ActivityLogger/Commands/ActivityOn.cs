using System;
using System.Collections.Generic;
using System.Text;
using CommandSystem;
using Exiled.Permissions.Extensions;
using ActivityLogger.API;

namespace ActivityLogger.Commands
{   [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ActivityOn : ICommand
    {
        public string Command { get; } = "activityon";

        public string[] Aliases { get; } = { "aon" };

        public string Description { get; } = "Displays information about activity on a certain day or all days logged";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if(!sender.CheckPermission("al.server"))
            {
                response = "Error, you do not have permission to use this command.";
                return false;
            }
            int period = PluginMain.Instance.Config.DataPeriodLength;
            // 😂
            switch (arguments.Count)
            {
                case 0:
                    //displays information about each period of time recorded for the server
                    StringBuilder generalInfo = new StringBuilder();
                    generalInfo.Append($"Showing unique player count for each {period} day period\n" +
                        $"Note: General server log information only updates every {period} days\n");
                    string startDateString = PluginMain.Instance.serverLog.FirstJoin;
                    string endDateString;
                    foreach (KeyValuePair<string, float> log in PluginMain.Instance.serverLog.LoggedTimeOnDay)
                    {
                        endDateString = log.Key;
                        generalInfo.Append($"From {startDateString} to {endDateString}: {log.Value} unique players\n");
                        //sets the start date for the next log to the this log plus a day
                        startDateString = DateTime.Parse(log.Key).AddDays(1).ToShortDateString();
                    }
                    float hours = PluginMain.Instance.serverLog.HoursPlayed;
                    generalInfo.Append($"Total combined hours played on the server: {hours}");
                    response = generalInfo.ToString();
                    return true;
                case 1:
                    //displays information about a specific day within %MaxLogs% days ago
                    DateTime userDate;
                    //checks if parsing succeeded
                    if(!DateTime.TryParse(arguments.At(0), out userDate))
                    {
                        response = $"Error, Invalid date: {arguments.At(0)}\nUse format mm/dd/yyyy";
                        return false;
                    }
                    //checks if specific data is recorded for day entered
                    if(DateTime.UtcNow.Subtract(userDate).TotalDays > ActivityRecord.MaxLogs)
                    {
                        response = "Error, date is out of range. To see generalized player activity for older dates, type aon";
                        return false;
                    }
                    StringBuilder players = new StringBuilder();
                    int playerCount = 0;
                    foreach(ActivityRecord record in PluginMain.Instance.ActivityDict.Values)
                    {
                        foreach(string date in record.LoggedTimeOnDay.Keys)
                        {
                            if (date.Equals(userDate.ToShortDateString()))
                            {
                                playerCount++;
                                players.Append(record.Nickname);
                                players.Append(", ");
                                continue;
                            }
                        }
                    }
                    response = $"There were {playerCount} people who played on this day. They were: \n" + players;
                    if (playerCount == 0) response = "No players played on this day.";
                    return true;
                default:
                    response = "Error, usage is: activityon [day]";
                    return false;
            }
        }
    }
}
