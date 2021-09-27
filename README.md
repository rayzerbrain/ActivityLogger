# ActivityLogger
SCP:SL Exiled plugin that logs a specific players total time on a server. Built for Exiled 3.0, might work on prior versions, not sure.
### IMPORTANT
All configs and commands are not case sensitive. This being said, if you do not enter the exact name of the player with the activityof command, it will not work, but it will give you possible names that CONTAIN your input. For example, if you enter in "activityof john doe" and there is only a player named "jon doe", it will not recognize your input at all. However, if you are more broad and type "doe", it will display all names with the text "doe" inside of it (e.g. "moe doe", "doe the deer", "doeaodffawefihaweifuohawsdaflkj")
### In-Depth Information
This plugin logs all players' total time spent on the server and can also tell you time spent in the last x days, where x is a number of your choosing.
Plugin uses IP as the unique identifier for each player, and it should be able to differentiate between people with the same ip.
There is a separate file for each server (if you have multiple), so to see total time across all servers you will have to manually add them up (located in the exiled configs folder).
### Commands to Use
activityof <player> 
This is the only command and main method of retrieving data of a player. Returns multiple sets of data if players with the same username are found. Also returns names of players that contain the input (see above).
### Configuration
All configuration can be found in the usual file

IsEnabled (true/false): determines whether plugin is active or not. (Player data will remain in file, but commands and new data won't be recognized) (default is true)

Days_Previous_Amount (integer): when viewing player data, gives you information about hours this player has in the last x days, where x is what you can change. (default is 30)

Player_Max_Logs (integer): amount of individual logs a player can have for each session (time logged on and off). 
This only affects the above configuration, so if the Days_Previous_Amount is set higher, this probably should be as well. 

Allowed_Roles (list of words): names of roles you wish to be able to use the activityof command. Role names are listed in the config_remoteadmin.txt file as the BADGE of the role. Example default values are given, but these can be removed. Create new entries like so:

allowed_roles:

~ yourRankHere

~ anotherRankHere
   
