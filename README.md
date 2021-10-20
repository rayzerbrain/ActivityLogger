# ActivityLogger
SCP:SL Exiled plugin that logs a specific players total time on a server and saves that information to a file, which can be utilized with a command. (Made for server: The Crypt)
### IMPORTANT
As of 3.0.0 this plugin uses exiled permissions. The permission for using the activityof, or aof command is al.get. To learn how to add permissions or the actual plugin go to the exiled discord or something.
### In-Depth Information
This plugin logs all players' total time spent on the server and can also tell you time spent in the last x days, where x is a number of your choosing.
There is a separate file for each server (if you have multiple), so to see total time across all servers you will have to manually add them up (folder with all files located in the exiled configs folder).
### Commands to Use
<b>activityof [playerNickname/userId]

AKA: aof [playerNickname/userId]</b>   
   
This is the main command and method of retrieving data of a player. Returns multiple sets of data if players with the same username are found. Also returns names of players that contain the input (e.g. if you type "a" it will display all nicknames with "a" in it, up to 20). IF no parameters are given, it will display a leaderboard of people with the most hours on the server, the length of which is defined in the config file.

<b>.myactivity

AKA: .ma</b>

Client command that runs similarly to the aof command, but only returns the specific player's activity information, allowing anyone to use it

### Configuration
All configuration can be found in the usual file

|Config name|Data type|Default value|Description|
|-----------|---------|-------------|-----------|
|Is_Enabled|Boolean|true|Determines if the plugin is enabled or not|
|Days_Counted|Integer|30|Days counted when determining the recent activity of a player|
|Leaderboard_Length|Integer|10|Length of the leaderboard of players with the most time|
|Min_Hours|Integer|10|Minimum amount of hours a player must have to not get hard cleaned (see below)|
|Wipe_Limit|Decimal|5|Maximum file size (in megabytes) that the data file must be before automatically hard cleaning (see below)
### Automatic Data Cleaning
As of version 4.0.0 data clean automatically on the waiting for players screen. Data will SOFT clean if the size of the data file is less than the Wipe_Limit config value, and HARD clean if the size is greater than that value. On SOFT cleaning, all logs older than 1.5 times the Days_Counted config value will be removed permanently for each recorded player. On HARD cleaning, the soft cleaning process will occur, AND the records of all players whose total time is less than the value of Min_Hours and do NOT have any recorded logs (as a result of soft cleaning) will be removed entirely. Be sure to adjust the values of Min_Hours and Days_Counted accordingly.
