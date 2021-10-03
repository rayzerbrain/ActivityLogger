using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ActivityLogger
{
    public class Config : IConfig
    {
        [Description("TRUE/FALSE: Determines if the plugin is enabled or not.")]
        public bool IsEnabled { get; set; } = true;
        [Description("INTEGER: Determines how many days in the past you want to count when determining the " +
            "recent activity of a player. (Default = 30)")]
        public int Days_Previous_Amount { get; set; } = 30;
        [Description("INTEGER: Determines the max amount of logs a player can have(one for each day)." +
            " Higher numbers means more data but possible bloated data, value of AT LEAST whatever Days_Previous_Amount " +
            "is set to is recommended. (This doesn't affect total logged time at all) (Default = 45)")]
        public int Player_Max_Logs { get; set; } = 45;
        [Description("INTEGER: Number that represents the length of the leaderboard displayed when one uses the " +
            "activityof command without any arguments (default is 10)")]
        public int Leaderboard_Length { get; set; } = 10;
    }
}
