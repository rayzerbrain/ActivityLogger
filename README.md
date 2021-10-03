# ActivityLogger
SCP:SL Exiled plugin that logs a specific players total time on a server. Built for Exiled 3.0, might work on prior versions, not sure.
### IMPORTANT
As of 3.0.0 this plugin uses exiled permissions. The permission for using the activityof, or aof command is al.get, while the command for wiping (should be more secure) is al.clean. To learn how to add permissions go to the exiled discord or something.
### In-Depth Information
This plugin logs all players' total time spent on the server and can also tell you time spent in the last x days, where x is a number of your choosing.
There is a separate file for each server (if you have multiple), so to see total time across all servers you will have to manually add them up (file located in the exiled configs folder).
### Commands to Use
activityof [playerNickname] 

AKA: aof [playerNickname]   
   
This is the main command and method of retrieving data of a player. Returns multiple sets of data if players with the same username are found. Also returns names of players that contain the input (e.g. if you type "a" it will display all nicknames with "a" in it). IF no parameters are given, it will display a leaderboard of people with the most hours on the server, the length of which is defined in the config file
   
actvityof_wipe
   
AKA: aof_wipe
   
Command that wipes all logs of all players. All recent log data is removed and cannot be retrieved again. Should only be removed if file is getting dangerously thicc. (This does not affect saved time in hours of all players, only affects recent hours shown)
### Configuration
All configuration can be found in the usual file

IsEnabled (true/false): determines whether plugin is active or not. (Player data will remain in file, but commands and new data won't be recognized) (default is true)

Days_Previous_Amount (integer): when viewing player data, gives you information about hours this player has in the last x days, where x is what you can change. (default is 30)

Player_Max_Logs (integer): amount of individual logs a player can have for each session (time logged on and off). 
This only affects the above configuration, so if the Days_Previous_Amount is set higher, this probably should be as well. (default is 45)

Leaderboard_Length (integer): determines length of leaderboard displayed when activityof is used without parameters (default is 10)

