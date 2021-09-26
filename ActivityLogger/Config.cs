using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityLogger
{
    public class Config : IConfig
    {
        [Description("TRUE/FALSE: Determines if the plugin is enabled or not. If disabled the plugin WILL still keep the data, but won't make new logs.")]
        public bool IsEnabled { get; set; } = true;
        [Description("INTEGER: Determines how many days in the past you want to count when determining the recent activity of a player. (Default = 30)")]
        public int Days_Previous_Amount { get; set; } = 30;
        [Description("INTEGER: Determines the max amount of logs a player can have(one for each log-in log-out session." +
            " Higher numbers means more data but possible slower response times, default value is recommended." +
            " (This doesn't affect totatl logged time at all) (Default = 500)")]
        public int Player_Max_Logs { get; set; } = 500;
        [Description("ROLE[]: List of roles that can use this command. Use the EXACT name of the role(but case doesn't matter. Example given, feel free to delete)")]
        public string[] Allowed_Roles { get; set; } =
        {
            "Owner",
            "Manager"
        };
    }
}
