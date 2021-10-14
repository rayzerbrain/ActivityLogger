# ActivityLogger
SCP:SL Exiled plugin that logs a specific players total time on a server. Built for Exiled 3.0, might work on prior versions, not sure.
### IMPORTANT
As of 3.0.0 this plugin uses exiled permissions. The permission for using the activityof, or aof command is al.get, while the command for wiping (should be more secure) is al.clean. To learn how to add permissions or the actual plugin go to the exiled discord or something.
### In-Depth Information
This plugin logs all players' total time spent on the server and can also tell you time spent in the last x days, where x is a number of your choosing.
There is a separate file for each server (if you have multiple), so to see total time across all servers you will have to manually add them up (folder with all files located in the exiled configs folder).
### Commands to Use
activityof [playerNickname/userId] 

AKA: aof [playerNickname/userId]   
   
This is the main command and method of retrieving data of a player. Returns multiple sets of data if players with the same username are found. Also returns names of players that contain the input (e.g. if you type "a" it will display all nicknames with "a" in it, up to 20). IF no parameters are given, it will display a leaderboard of people with the most hours on the server, the length of which is defined in the config file.


actvityof_clean (removed as of 4.0.0, data now cleans automatically)
   
AKA: aof_clean
   
Command that wipes all logs of all players. All recent log data is removed and cannot be retrieved again. Should only be removed if file is getting dangerously thicc. (This does not affect saved time in hours of all players, only affects recent hours shown)

### Automatic Data Cleaning
As of version 4.0.0 data clean automatically on the waiting for players screen. Data will SOFT clean if the size of the data file is less than the Wipe_Limit config value, and HARD clean if the size is greater than that value. On SOFT cleaning, all logs older than 1.5 times the Days_Counted config value will be removed permanently for each recorded player. On HARD cleaning, the soft cleaning process will occur, AND the records of all players whose total time is less than the value of Min_Hours and do NOT have any recorded logs (as a result of soft cleaning) will be removed entirely. Be sure to adjust the values of Min_Hours and Days_Counted accordingly.

### Configuration
All configuration can be found in the usual file

IsEnabled (true/false, default = true): determines whether plugin is active or not. (Player data will remain in file, but commands and new data won't be recognized)

Days_Counted (Days_Previous_Amount prior to 4.0.0) (integer, default = 30): when viewing player data, gives you information about hours this player has in the last x days, where x is what you can change.

Leaderboard_Length (integer, default = 10): determines length of leaderboard displayed when activityof is used without parameters

(removed as of 4.0.0, max logs is automated to the value of 1.5 * Days_Counted) Player_Max_Logs (integer, default = 45): amount of logged days a player can have. (time logged on and off).

(added as of 4.0.0)

Min_Hours (integer, default = 10): minimum amount of hours a player must have to not get removed from the list IF hard cleaning is occurring each round

Wipe_Limit (decimal, default = 5.0): max size (in megabytes) the data file can be before it is hard cleaned each round
