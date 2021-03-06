using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using BarRaider.SdTools;
using Dota2GSI;
using Microsoft.Win32;

namespace StreamDeckPluginsDota2
{
    static class Program
    {
        private static GameStateListener _gsi;
        
        static void Main(string[] args)
        {
            InitializeGameStateIntegration();
            
            // Uncomment this line of code to allow for debugging
            //while (!System.Diagnostics.Debugger.IsAttached) { System.Threading.Thread.Sleep(100); }
            
            SDWrapper.Run(args);
        }

        static void InitializeGameStateIntegration()
        {
            CreateConfigs();

            Process[] processName = Process.GetProcessesByName("Dota2");
            if (processName.Length == 0)
            {
                Console.WriteLine("Dota 2 is not running. Please start Dota 2.");
            }

            _gsi = new GameStateListener(4000);
            _gsi.NewGameState += OnNewGameState;
            
            if (!_gsi.Start())
            {
                Console.WriteLine("GameStateListener could not start. Try running this program as Administrator. Exiting.");
            }
        }
        
        static void OnNewGameState(GameState gs)
        {
            // TODO: Implement GameStateIntegration
        }

        private static void CreateConfigs()
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");

            if (regKey != null)
            {
                CreateGSIFile(regKey);
                CreateCFGFile(regKey);
            }
            else
            {
                Console.WriteLine("Registry key for steam not found, cannot create Gamestate Integration / CFG files");
                Console.ReadLine();
            }
        }
        
        /// <summary>
        /// Creates our Game State Integration folder and configuration file if one doesn't exist.
        /// </summary>
        /// <param name="regKey"></param>
        private static void CreateGSIFile(RegistryKey regKey)
        {
            string gsiFolder = regKey.GetValue("SteamPath") +
                               @"\steamapps\common\dota 2 beta\game\dota\cfg\gamestate_integration";
            Directory.CreateDirectory(gsiFolder);
            string gsiFile = gsiFolder + @"\gamestate_integration_stream_deck_plugins_dota_2.cfg";
            if (!File.Exists(gsiFile))
            {
                string[] contentOfGSIFile =
                {
                    "\"Dota 2 Integration Configuration\"",
                    "{",
                    "    \"uri\"           \"http://localhost:4000\"",
                    "    \"timeout\"       \"5.0\"",
                    "    \"buffer\"        \"0.1\"",
                    "    \"throttle\"      \"0.1\"",
                    "    \"heartbeat\"     \"30.0\"",
                    "    \"data\"",
                    "    {",
                    "        \"provider\"      \"1\"",
                    "        \"map\"           \"1\"",
                    "        \"player\"        \"1\"",
                    "        \"hero\"          \"1\"",
                    "        \"abilities\"     \"1\"",
                    "        \"items\"         \"1\"",
                    "    }",
                    "}",

                };

                File.WriteAllLines(gsiFile, contentOfGSIFile);
            }
        }

        /// <summary>
        /// Creates our configuration file for our rune action binds if one doesn't exist.
        /// </summary>
        private static void CreateCFGFile(RegistryKey regKey)
        {
            string cfgFolder = regKey.GetValue("SteamPath") +
                               @"\steamapps\common\dota 2 beta\game\dota\cfg";
    
            string cfgFile = cfgFolder + @"\stream_deck_plugins_dota_2.cfg";
            if (!File.Exists(cfgFile))
            {
                string[] contentsOfCFGFile =
                {
                    "bind \"F13\" \"+dota_camera_center_on_hero\";",
                    "bind \"F14\" \"dota_camera_set_lookatpos -1620 950\";", // Top Rune
                    "bind \"F15\" \"dota_camera_set_lookatpos 1200 -1400\";", // Bot Rune
                    "echo \"Dota 2 - Stream Deck Keybindings Loaded Successfully!"
                };
                    
                File.WriteAllLines(cfgFile, contentsOfCFGFile);
            }
        }
    }
}
