using Exiled.API.Features;
using Exiled.Loader;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;
using ActivityLogger.API;

namespace ActivityLogger
{
    
    public class PluginMain : Plugin<Config>
    {
        static PluginMain Singleton;
        //what the heck is a =>
        public static PluginMain Instance => Singleton;
        public override Version Version => new Version(4, 3, 0);
        public override Version RequiredExiledVersion => new Version(3, 0, 0);
        public override string Author => "rayzer";
        public override string Name => "ActivityLogger";

        internal EventHandlers EventHandler;

        internal static string DataPath;
        internal IDeserializer DataLoader;
        internal ISerializer DataSaver;
        //MAIN DATA SOURCE - Keeps track of all players and their logs, loaded when waiting for players and saved & unloaded on restarts/shutdowns
        internal Dictionary<string, ActivityRecord> ActivityDict;
        internal DateTime FirstLogDate => DateTime.Parse(ActivityDict.ToList()[0].Value.FirstJoin);
        //new as of 4.2.0, logs server wide information.
        //saves and loads along side of ActivityDict, but is separate from it so other code doesn't confuse it with a player log
        internal ActivityRecord serverLog;
        internal long fileSize;
        public override void OnEnabled()
        {
            Singleton = this;
            EventHandler = new EventHandlers(this);
            ActivityDict = new Dictionary<string, ActivityRecord>();
            string folderPath = Paths.Configs + "/Player_Activity_Data";
            DataPath = folderPath+"/Port"+Exiled.API.Features.Server.Port+".yml";
            DataLoader = Loader.Deserializer;
            DataSaver = Loader.Serializer;
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            if(!File.Exists(DataPath))
            {
                Log.Warn("Activity File missing, creating...");
                SaveDataFile();
            }
            RegisterEvents();
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            SaveDataFile();
            UnregisterEvents();
            DataPath = null;
            DataLoader = null;
            DataSaver = null;
            serverLog = null;
            ActivityDict = null;
            EventHandler = null;
            Singleton = null;
            base.OnDisabled();
        }

        public void RegisterEvents()
        {
            Player.Verified += EventHandler.OnVerified;
            Server.WaitingForPlayers += EventHandler.OnWaitingForPlayers;
            Server.RoundStarted += EventHandler.OnRoundStarted;
            Server.RoundEnded += EventHandler.OnRoundEnded;
        }
        public void UnregisterEvents()
        {
            Player.Verified -= EventHandler.OnVerified;
            Server.WaitingForPlayers -= EventHandler.OnWaitingForPlayers;
            Server.RoundStarted -= EventHandler.OnRoundStarted;
            Server.RoundEnded -= EventHandler.OnRoundEnded;
        }
        
        
        //Thanks to temmiegamerguy for his examples of yml file storage in his lefthandedplayers plugin
        //I am too smooth brained to figure it out for myself

        //Serializes data to the data path
        public void SaveDataFile()
        {
            if (ActivityDict == null)
            {
                Log.Error("The data file is null, not saving yet");
                return;
            }
            ActivityDict.Add("server", serverLog);
            using (StreamWriter writer = new StreamWriter(DataPath))
            {
                DataSaver.Serialize(writer, ActivityDict);
            }
            ActivityDict.Remove("server");
        }
        //Loads data from the path
        public void LoadDataFile()
        {
            if (File.Exists(DataPath))
            {
                FileStream dataFile = new FileStream(DataPath, FileMode.OpenOrCreate);
                fileSize = dataFile.Length;
                using (StreamReader reader = new StreamReader(dataFile))
                {
                    //Either warns of no data, or loads data successfully
                    if (dataFile.Length == 0) Log.Warn("File has no recorded players, no data will be loaded");
                    else
                    {
                        ActivityDict = DataLoader.Deserialize<Dictionary<string, ActivityRecord>>(reader);
                        if (!ActivityDict.ContainsKey("server")) serverLog = new ActivityRecord("server", "server");
                        else serverLog = ActivityDict["server"];
                        ActivityDict.Remove("server");
                    }
                }
            }
            else Log.Error("No activity file was found. Try restarting the server");
        }
    }
}
