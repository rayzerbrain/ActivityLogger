using Exiled.API.Interfaces;
using System.ComponentModel;

namespace ActivityLogger
{
    public class Config : IConfig
    {
        [Description("TRUE/FALSE: Determines if the plugin is enabled or not.")]
        public bool IsEnabled { get; set; } = true;
        [Description("INTEGER: Determines how many days in the past you want to count when determining the" +
            " recent activity of a player. (Default = 30)")]
        public int DaysCounted { get; set; } = 30;
        [Description("INTEGER: Determines the length of the time period in which server activity is recorded. For example, "+
            "if the value is 7, server log will store total unique player count of each 7-day period. MUST be greater than 1")]
        public int DataPeriodLength { get; set; } = 15;
        
        [Description("INTEGER: Determines the length of the leaderboard shown when using the activityof command." +
            " without parameters. (Default = 10)")]
        public int LeaderboardLength { get; set; } = 10;
        [Description("INTEGER: Number that determines the minimum amount of hours needed to keep a players' log from being" +
            " deleted when HARD cleaning. (Default = 10)")]
        public int MinHours { get; set; } = 10;
        [Description("DECIMAL: Determines how large (in megabytes) the activity file needs to be before hard data cleaning" +
            " occurs on round start. Set to -1 to disable (not recommended, file may get very large). (Default is 5)")]
        public float WipeLimit { get; set; } = 5;
    }
}
