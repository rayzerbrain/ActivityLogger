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
        private static PluginMain Singleton = new PluginMain();
        public static PluginMain Instance => Singleton;
        public override Version Version { get; } = new Version(3, 1, 0);
        public override Version RequiredExiledVersion { get; } = new Version(3, 0, 0);
        public override string Author { get; } = "rayzer";
        public override string Name { get; } = "ActivityLogger";

        public EventHandlers EventHandler { get; set; }

        internal static string DataPath { get; set; }
        internal IDeserializer DataLoader { get; set; }
        internal ISerializer DataSaver { get; set; }
        //MAIN DATA SOURCE
        //Keeps track of all players and their logs, is loaded when the server is waiting for players and is unloaded when disabled/server restarts
        public Dictionary<string, PlayerActivity> PlayerActivityDictionary { get; set; } 
        public override void OnEnabled()
        {
            Singleton = this;
            EventHandler = new EventHandlers();
            DataPath = Paths.Configs + "/"+"PlayerActivityData_Port"+Exiled.API.Features.Server.Port+".yml";
            DataLoader = Loader.Deserializer;
            DataSaver = Loader.Serializer;
            LoadActivityData();
            Player.Verified += EventHandler.OnVerified;
            Server.WaitingForPlayers += EventHandler.OnWaitingForPlayers;
            Server.RoundStarted += EventHandler.OnRoundStarted;
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            Player.Verified -= EventHandler.OnVerified;
            Server.WaitingForPlayers -= EventHandler.OnWaitingForPlayers;
            Server.RoundStarted -= EventHandler.OnRoundStarted;
            PlayerActivityDictionary = null;
            DataPath = null;
            DataLoader = null;
            DataSaver = null;
            EventHandler = null;
            Singleton = null;
            base.OnDisabled();
        }
        //Thanks to temmiegamerguy for his examples of yml file storage in his lefthandedplayers plugin
        //I am too smooth brained to figure it out for myself
        public void SaveActivityData()
        {
            using (var writer = new StreamWriter(DataPath))
            {
                DataSaver.Serialize(writer, PlayerActivityDictionary);
            }
        }
        public void LoadActivityData()
        {
            FileStream DataFile = new FileStream(DataPath, FileMode.OpenOrCreate);
            if (File.Exists(DataPath))
            {
                using (var reader = new StreamReader(DataFile))
                {
                    PlayerActivityDictionary = DataLoader.Deserialize<Dictionary<string, PlayerActivity>>(reader);
                }
                
            }
            else
            {
                PlayerActivityDictionary = new Dictionary<string, PlayerActivity>();
            }
        }
    }
}
