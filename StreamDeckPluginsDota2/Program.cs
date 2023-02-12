using System;
using System.Diagnostics;
using System.Drawing;
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
        // Dota 2 - Game State Integration
        public static GameStateListener m_gameStateListener;
        private static bool m_hasGSIStarted;
        private static GameState m_gameState;

        // Used to check 'Dota 2' process
        private static Timer m_applicationTimer; // Update method
        private static Process[] m_dotaProcesses;
        private static bool isDotaRunning;
        
        // Used to check the currently focused window
        /// <summary>
        /// Retrieves a handle to the foreground window (the window with which the user is currently working).
        /// Note: The foreground can be NULL in certain instances, such as when a window is losing activation.
        /// </summary>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        /// <summary>
        /// Retrieves the identifier of the thread that created the specified window and, optionally,
        /// the identifier of the process that created the window.
        /// </summary>
        /// <param name="hWnd">A handle to the window.</param>
        /// <param name="lpdwProcessId">A pointer to the process identifier.*</param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern Int32 GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        
        // Generic State Images
        public static Image m_red;
        public static Image m_yellow;
        public static Image m_green;

        static void Main(string[] args)
        {
            // Parse launch arguments
            bool isDebugging = false;
            foreach (string s in args)
            {
                if (s == "-debug")
                {
                    isDebugging = true;
                }
            }

            // Initialize our logic
            Initialize(isDebugging);
            
            // Initialize the Stream Deck wrapper logic
            SDWrapper.Run(args);
        }

        /// <summary>
        /// Creates the appropriate config files (GSI config and CFG hotkey config), checks the state of any active
        /// dota processes, initializes our GSI to be ready for listening, console log the game state, and finally
        /// creates and starts our application tick/update method so we can re-check GSI state.
        /// </summary>
        /// <param name="debugMode"></param>
        static void Initialize(bool debugMode = false)
        {
            if (debugMode)
            {
                Console.WriteLine("DEBUG MODE ENABLED.");
            }

            CreateConfigs();
            CreateImages();

            m_dotaProcesses = Process.GetProcessesByName("Dota2");
            isDotaRunning = m_dotaProcesses.Length > 0;
            
            if (!isDotaRunning)
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
            m_applicationTimer.Elapsed += ApplicationTick;
            m_applicationTimer.AutoReset = true;
            m_applicationTimer.Interval = 1000; // 1 tick per second
            m_applicationTimer.Start();

            if (debugMode)
            {
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Attempts to start the 'Dota 2 Game State Listener'.
        /// Returns a boolean indicating if the start was successful or not.
        /// </summary>
        /// <returns>Was initialization successful?</returns>
        public static bool InitializeGSI()
        {
            // Init GSI
            m_gameStateListener = new GameStateListener(4000);
            m_gameStateListener.NewGameState += OnNewGameState;

            if (m_gameStateListener.Start())
            {
                // Dirty flag
                m_hasGSIStarted = true;
                
                Console.WriteLine("GSI has started successfully.");
                return true;
            }

            // Clean flag
            m_hasGSIStarted = false;
            
            Console.WriteLine("GSI was unable to start.");
            return false;
        }

        /// <summary>
        /// Has the 'Game State Listener' been able to start/initialize?
        /// </summary>
        /// <returns></returns>
        public static bool isGSIInitialized()
        {
            return m_hasGSIStarted;
        }

        /// <summary>
        /// Invoked when the GSI listener receives a new GameState.
        /// </summary>
        /// <param name="gamestate"></param>
        private static void OnNewGameState(GameState gamestate)
        {
            // Console.WriteLine("Program: New Game State Received");
            m_gameState = gamestate;
        }

        /// <summary>
        /// Update method that ticks once per second.
        /// Intended for checking the Dota process and checking/verifying/initializing GSI.
        /// </summary>
        private static void ApplicationTick(object sender, ElapsedEventArgs e)
        {
            m_dotaProcesses = Process.GetProcessesByName("Dota2");
            isDotaRunning = m_dotaProcesses.Length > 0;

            // Init GSI if it hasn't been initialized...
            if (isDotaRunning)
            {
                if (!m_hasGSIStarted)
                {
                    InitializeGSI();
                }

                // Sanity check
                if (m_gameState == null)
                {
                    Console.WriteLine("Null game state.");
                    
                    // Early exit
                    return;
                }
                
                Console.WriteLine(m_gameState.Map.GameState.ToString());
            }
        }

        /// <summary>
        /// If a Dota 2 process is running, this will return True. Otherwise, this will return False.
        /// </summary>
        /// <returns></returns>
        public static bool IsDotaRunning()
        {
            return isDotaRunning;
        }

        /// <summary>
        /// Is Dota running and is the currently focused window?
        /// </summary>
        /// <returns>
        /// If a Dota 2 process is not running, this will return False.
        /// If a Dota 2 process is running but is not focused, this will return False.
        /// If a Dota 2 process is running and is focused, this will return True.
        /// </returns>
        public static bool IsDotaFocused()
        {
            if (!IsDotaRunning())
            {
                return false;
            }

            IntPtr focusedWindow = GetForegroundWindow();
            if (focusedWindow == null)
            {
                // Nothing is currently focused.
                return false;
            }

            uint focusedWindowProcessID;
            GetWindowThreadProcessId(focusedWindow, out focusedWindowProcessID);

            foreach (Process dotaProcess in m_dotaProcesses)
            {
                // If the current focused ID matches any of our cached dota processes...
                if (focusedWindowProcessID == dotaProcess.Id)
                {
                    // Current focused window is Dota.
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Set's the current user's foreground window to be Dota 2.
        /// </summary>
        public static void FocusDota()
        {
            if (m_dotaProcesses.Length > 0)
            {
                Process dotaProcess = m_dotaProcesses[0];
                SetForegroundWindow(dotaProcess.MainWindowHandle);
            }
        }

        /// <summary>
        /// Creates the appropriate GSI and CFG files for this plugin. (GSI for reading game state, CFG for setting
        /// hotkeys)
        /// </summary>
        private static void CreateConfigs()
        {
            // TODO: Create popup dialogs showing where config files were saved...
            
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
        /// Generates our generic state images that can be used throughout the app.
        /// </summary>
        private static void CreateImages()
        {
            // Generate assets and cache images
            m_red = GenerateSolidColorImage(144, 144, Color.FromArgb(210, 40, 40));
            m_yellow = GenerateSolidColorImage(144, 144, Color.FromArgb(255, 193, 50));
            m_green = GenerateSolidColorImage(144, 144, Color.FromArgb(42, 168, 67));
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
                    "bind \"F13\" \"+dota_camera_center_on_hero\";",                // Rune Key Release
                    "bind \"F14\" \"dota_camera_set_lookatpos -1620 950\";",        // Top Rune
                    "bind \"F15\" \"dota_camera_set_lookatpos 1200 -1400\";",       // Bot Rune
                    "bind \"F16\" \"dota_pause\";",                                 // Pause Toggle
                    "echo \"Dota 2 - Stream Deck Keybindings Loaded Successfully!"  // Source Console Log
                };
                    
                File.WriteAllLines(cfgFile, contentsOfCFGFile);
            }
        }
        
        /// <summary>
        /// Generates and returns an Image component using the provided dimensions and solid color.
        /// </summary>
        /// <param name="width">What width (in pixels) should the image be?</param>
        /// <param name="height">What height (in pixels) should the image be?</param>
        /// <param name="color">What color should this image be?</param>
        /// <returns></returns>
        private static Image GenerateSolidColorImage(int width, int height, Color color)
        {
            // Generate bitmap (image)
            Bitmap bitmap = new Bitmap(width, height);
            
            // Iterate through each pixel...
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    // Set color
                    bitmap.SetPixel(x, y, color);
                }
            }
            
            return new Bitmap(bitmap);
        }

        /// <summary>
        /// Returns a formatted time string (such as '0:00') using the provided totalSeconds int.
        /// </summary>
        /// <example>Input: 62 | Output: '1:02'</example>
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
