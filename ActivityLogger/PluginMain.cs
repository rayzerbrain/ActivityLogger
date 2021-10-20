using Exiled.API.Features;
using Exiled.Loader;
using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;

namespace ActivityLogger
{
    public class PluginMain : Plugin<Config>
    {
        private static PluginMain Singleton;
        //what the heck is a =>
        public static PluginMain Instance => Singleton;
        public override Version Version => new Version(4, 1, 0);
        public override Version RequiredExiledVersion => new Version(3, 0, 0);
        public override string Author => "rayzer";
        public override string Name => "ActivityLogger";

        internal EventHandlers EventHandler;

        internal static string DataPath;
        internal IDeserializer DataLoader;
        internal ISerializer DataSaver;
        //MAIN DATA SOURCE - Keeps track of all players and their logs, loaded when waiting for players and saved & unloaded on restarts/shutdowns
        internal Dictionary<string, PlayerActivity> ActivityDict;
        internal long fileSize;
        public override void OnEnabled()
        {
            Singleton = this;
            EventHandler = new EventHandlers(this);
            ActivityDict = new Dictionary<string, PlayerActivity>();
            string folderPath = Paths.Configs + "/Player_Activity_Data";
            DataPath = folderPath+"/Port"+Exiled.API.Features.Server.Port+".yml";
            DataLoader = Loader.Deserializer;
            DataSaver = Loader.Serializer;
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            if (!File.Exists(DataPath))
            {
                Log.Warn("Activity File missing, creating...");
                SaveDataFile();
            }
            RegisterEvents();
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Log.Info("on disabled called");
            SaveDataFile();
            UnregisterEvents();
            DataPath = null;
            DataLoader = null;
            DataSaver = null;
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
            using (StreamWriter writer = new StreamWriter(DataPath))
            {
                DataSaver.Serialize(writer, ActivityDict);
            }
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
                    else ActivityDict = DataLoader.Deserialize<Dictionary<string, PlayerActivity>>(reader);
                    
                }
            }
            else Log.Error("No file was found. Try restarting the server");
        }
    }
}
