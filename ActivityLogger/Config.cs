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
        public int Days_Counted { get; set; } = 30;
        [Description("INTEGER: Determines the length of the leaderboard shown when using the activityof command" +
            " without parameters. (Default = 10)")]
        public int Leaderboard_Length { get; set; } = 10;
        [Description("INTEGER: Number that determines the minimum amount of hours needed to keep a players' log from being" +
            " deleted when HARD cleaning. (Default = 10)")]
        public int Min_Hours { get; set; } = 10;
        [Description("DECIMAL: Determines how large (in megabytes) the activity file needs to be before hard data cleaning" +
            " occurs on round start. Set to -1 to disable (not recommended, file may get very large). (Default is 5)")]
        public int Wipe_Limit { get; set; } = 5;
    }
}
