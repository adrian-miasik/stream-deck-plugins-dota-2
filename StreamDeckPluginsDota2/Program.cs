using System;
using System.Diagnostics;
using System.IO;
using System.Timers;
using BarRaider.SdTools;
using Dota2GSI;
using Microsoft.Win32;
using Timer = System.Timers.Timer;

namespace StreamDeckPluginsDota2
{
    static class Program
    {
        public static GameStateListener m_gameStateListener;
        public static bool m_hasGSIStarted;
        private static GameState m_gameState;

        private static Timer m_applicationTimer; // Used for checking if dota process is active.
        private static Process[] m_dotaProcesses;
        private static bool isDotaRunning;

        static void Main(string[] args)
        {
            Setup();
            
            // Uncomment this line of code to allow for debugging
            //while (!System.Diagnostics.Debugger.IsAttached) { System.Threading.Thread.Sleep(100); }
            
            SDWrapper.Run(args);
        }

        static void Setup()
        {
            CreateConfigs();

            Process[] processName = Process.GetProcessesByName("Dota2");
            if (processName.Length == 0)
            {
                Console.WriteLine("Dota 2 is not running. Please start Dota 2.");
            }
            // Otherwise dota is running...
            else
            {
                Console.WriteLine("Dota 2 is running!");
                
                if (!m_hasGSIStarted)
                {
                    InitializeGSI();
                }
                
                // Sanity check
                if (m_gameState == null)
                {
                    Console.WriteLine("Null game state.");
                }
                else
                {
                    Console.WriteLine(m_gameState.Map.GameState.ToString());
                }
            }
            
            // Create and start application timer to track dota active process for GSI
            m_applicationTimer = new Timer();
            m_applicationTimer.Elapsed += Tick;
            m_applicationTimer.AutoReset = true;
            m_applicationTimer.Interval = 1000; // 1 tick per second
            m_applicationTimer.Start();

            // Process debugProcess = new Process();
            // ProcessStartInfo startInfo = new ProcessStartInfo
            // {
            //     FileName = "cmd.exe",
            //     Arguments = "/K echo 'StreamDeckPlugins - Dota 2': Live Debugger is Ready.",
            // };
            // debugProcess.StartInfo = startInfo;
            // debugProcess.Start();

            // IMPORTANT: TOGGLE THE LINE BELOW FOR DEBUGGING
            // COMMENTED FOR RELEASE, UNCOMMENTED FOR DEBUGGING.
            // Console.ReadLine();
        }

        public static bool InitializeGSI()
        {
            // Init GSI
            m_gameStateListener = new GameStateListener(4000);
            m_gameStateListener.NewGameState += OnNewGameState;

            if (m_gameStateListener.Start())
            {
                // Mark init flag
                m_hasGSIStarted = true;
                
                Console.WriteLine("GSI has started successfully.");
                return true;
            }

            Console.WriteLine("GSI was unable to start.");
            return false;
        }

        private static void OnNewGameState(GameState gamestate)
        {
            Console.WriteLine("New Game State Received");
            m_gameState = gamestate;
        }

        /// <summary>
        /// Update method that ticks once per second.
        /// </summary>
        private static void Tick(object sender, ElapsedEventArgs e)
        {
            m_dotaProcesses = Process.GetProcessesByName("Dota2");
            isDotaRunning = m_dotaProcesses.Length > 0;

            if (!isDotaRunning)
            {
                Console.WriteLine("Dota 2 is not running. Please start Dota 2.");
            }
            // Otherwise dota is running...
            else
            {
                // Console.WriteLine("Dota 2 is running!");
                
                if (!m_hasGSIStarted)
                {
                    InitializeGSI();
                }

                // Sanity check
                if (m_gameState == null)
                {
                    Console.WriteLine("Null game state.");
                    return;
                }
                
                Console.WriteLine(m_gameState.Map.GameState.ToString());
            }
        }

        public static bool IsDotaRunning()
        {
            return isDotaRunning;
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
            
            // Doesn't overwrite if file exists
            if (!File.Exists(cfgFile))
            {
                string[] contentsOfCFGFile =
                {
                    "bind \"F13\" \"+dota_camera_center_on_hero\";",
                    "bind \"F14\" \"dota_camera_set_lookatpos -1620 950\";", // Top Rune
                    "bind \"F15\" \"dota_camera_set_lookatpos 1200 -1400\";", // Bot Rune
                    "bind \"F16\" \"dota_pause\";", // Pause Toggle
                    "echo \"Dota 2 - Stream Deck Keybindings Loaded Successfully!"
                };
                    
                File.WriteAllLines(cfgFile, contentsOfCFGFile);
            }
        }
        
        /// <summary>
        /// Returns a formatted time string (such as '0:00') using the provided totalSeconds variable.
        /// </summary>
        /// <param name="totalSeconds"></param>
        /// <returns></returns>
        public static string GetFormattedString(int totalSeconds)
        {
            string prefix = string.Empty;
            
            // If negative...
            if (totalSeconds < 0)
            {
                prefix = "-";
            }

            // Force positive
            totalSeconds = Math.Abs(totalSeconds);
            int totalMinutes = totalSeconds / 60;

            // If total seconds is not past a minute...
            if (totalMinutes == 0)
            {
                return prefix + totalMinutes + ":" + totalSeconds.ToString("00");
            }
            
            // Else, total seconds needs minutes removed from itself.
            return prefix + totalMinutes + ":" + (totalSeconds - totalMinutes * 60).ToString("00");
        }
    }
}
