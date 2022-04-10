﻿using System;
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
            // Uncomment this line of code to allow for debugging
            //while (!System.Diagnostics.Debugger.IsAttached) { System.Threading.Thread.Sleep(100); }

            SDWrapper.Run(args);

            if (args == null)
            {
                Console.WriteLine();
            }

            CreateConfigs();

            Process[] processName = Process.GetProcessesByName("Dota2");
            if (processName.Length == 0)
            {
                Console.WriteLine("Dota 2 is not running. Please start Dota 2.");
                Console.ReadLine();
                Environment.Exit(0);
            }

            _gsi = new GameStateListener(4000);
            _gsi.NewGameState += OnNewGameState;
            
            if (!_gsi.Start())
            {
                Console.WriteLine("GameStateListener could not start. Try running this program as Administrator. Exiting.");
                Console.ReadLine();
                Environment.Exit(0);
            }
            Console.WriteLine("Listening for game integration calls...");
        }
        
        static void OnNewGameState(GameState gs)
        {
            Console.Clear();
            Console.WriteLine("Press ESC to quit");
            Console.WriteLine("Current Dota version: " + gs.Provider.Version);
            Console.WriteLine("Current time as displayed by the clock (in seconds): " + gs.Map.ClockTime);
            Console.WriteLine("Your steam name: " + gs.Player.Name);
            Console.WriteLine("hero ID: " + gs.Hero.ID);
            Console.WriteLine("Health: " + gs.Hero.Health);
            for (int i = 0; i < gs.Abilities.Count; i++)
            {
                Console.WriteLine("Ability {0} = {1}", i, gs.Abilities[i].Name);
            }
            Console.WriteLine("First slot inventory: " + gs.Items.GetInventoryAt(0).Name);
            Console.WriteLine("Second slot inventory: " + gs.Items.GetInventoryAt(1).Name);
            Console.WriteLine("Third slot inventory: " + gs.Items.GetInventoryAt(2).Name);
            Console.WriteLine("Fourth slot inventory: " + gs.Items.GetInventoryAt(3).Name);
            Console.WriteLine("Fifth slot inventory: " + gs.Items.GetInventoryAt(4).Name);
            Console.WriteLine("Sixth slot inventory: " + gs.Items.GetInventoryAt(5).Name);

            Console.WriteLine(gs.Items.InventoryContains("item_blink")
                ? "You have a blink dagger"
                : "You DO NOT have a blink dagger");
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
                Environment.Exit(0);
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
