# ActivityLogger
SCP:SL Exiled plugin that logs a specific players total time on the server
### IMPORTANT
All configs and commands are not case sensitive. This being said, if you do not enter the exact name of the player with the activityof command, it will not work, but it will give you possible names that CONTAIN your input. For example, if you enter in "activityof john doe" and there is only a player named "jon doe", it will not recognize your input at all. However, if you are more broad and type "doe", it will display all names with the text "doe" inside of it (e.g. "moe doe", "doe the deer", "doeaodffawefihaweifuohawsdaflkj")
### In-Depth Information
This plugin logs all players' total time spent on the server and can also tell you time spent in the last x days, where x is a number of your choosing.
Plugin uses IP as the unique identifier for each player, and it should be able to differentiate between people with the same ip.
There is a separate file for each server (if you have multiple), so to see total time across all servers you will have to manually add them up.
### Commands to Use
activityof <player> 
This is the only command and main method of retrieving data of a player. Returns multiple sets of data if players with the same username are found. Also returns names of players that contain the input (see above).
### Configuration
All configuration can be found in the usual file
\nIsEnabled (true/false): determines whether plugin is active or not. (Player data will remain in file, but commands and new data won't be recognized)
\nDays_Previous_Amount (integer): determines how many days 
\nPlayer_Max_Logs (integer):
