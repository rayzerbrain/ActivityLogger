using System;
using Exiled.API.Features;
using CommandSystem;

namespace ActivityLogger.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    class MyActivity : ICommand
    {
        public string Command { get; } = "myactivity";

        public string[] Aliases { get; } = { "myact" };

        public string Description { get; } = "Gets info about your activity on this specific server";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player asker = Player.Get(sender);
            if(arguments.Count!=0)
            {
                response = "Error, usage is: .myactivity";
                return false;
            }
            if(PluginMain.Instance.ActivityDict.ContainsKey(asker.UserId))
            {
                response = PluginMain.Instance.ActivityDict[asker.UserId].GetInfo();
                return true;
            }
            response = "Your log wasn't found. (Do you have Do Not Track enabled?)";
            return false;
        }
    }
}
