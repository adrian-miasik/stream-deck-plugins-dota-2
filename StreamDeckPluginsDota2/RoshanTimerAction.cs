using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Timers;

namespace StreamDeckPluginsDota2
{
    [PluginActionId("com.adrian-miasik.sdpdota2.roshan-timer")]
    public class RoshanTimerAction : PluginBase
    {
        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                PluginSettings instance = new PluginSettings
                {
                    TotalSeconds = 0,
                    DeathCount = 0,
                    IsRunning = 0,
                    IsPaused = 0,
                    LastVisibleTime = DateTime.Now
                };
                return instance;
            }

            [JsonProperty(PropertyName = "totalSeconds")]
            public int TotalSeconds { get; set; }

            [JsonProperty(PropertyName = "deathCount")]
            public int DeathCount { get; set; }

            [JsonProperty(PropertyName = "isRunning")]
            public int IsRunning { get; set; }

            [JsonProperty(PropertyName = "isPaused")]
            public int IsPaused { get; set; }
            
            [JsonProperty(PropertyName = "lastVisibleTime")]
            public DateTime LastVisibleTime { get; set; }
        }
        
        private readonly PluginSettings m_settings;
        
        private Timer m_applicationTimer; // Used for processing input. Cannot be paused.
        
        // Hold
        private bool m_isKeyHeld;
        private DateTime m_pressedKeyTime;
        private bool m_ignoreKeyRelease;

        // Double Click
        private int m_numberOfPresses;
        private DateTime m_releasedKeyTime;
        private bool m_hasDoubleClicked;
        
        // Roshan
        private Timer m_roshanTimer; // Used for keeping track of roshan's respawn time. Can be paused.

        public RoshanTimerAction(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                m_settings = PluginSettings.CreateDefaultSettings();
                SaveSettings();
            }
            else
            {
                m_settings = payload.Settings.ToObject<PluginSettings>();
            }
            
            if (m_settings.IsPaused == 0 && m_settings.IsRunning == 1)
            {
                int timeSinceLast = Math.Abs((m_settings.LastVisibleTime - DateTime.Now).Seconds);
                Connection.SetTitleAsync("+" + timeSinceLast);
                m_settings.TotalSeconds += timeSinceLast;
                SaveSettings();
            }

            CreateApplicationTimer();
            CreateRoshanTimer(); // This timer doesn't get started until user input
            
            if (m_settings.IsPaused == 0)
            {
                m_roshanTimer.Start();
            }
        }

        /// <summary>
        /// Creates and plays an application timer. The application timer starts when created, and is
        /// responsible for keeping track of holds, and double click actions hence the higher poll rate.
        /// </summary>
        private void CreateApplicationTimer()
        {
            m_applicationTimer = new Timer();
            m_applicationTimer.Elapsed += ApplicationTimerTick;
            m_applicationTimer.AutoReset = true;
            m_applicationTimer.Interval = 100; // 10 ticks per second
            m_applicationTimer.Start();
        }

        /// <summary>
        /// An tick method (Invoked 10 times per second by default) for the application timer.
        /// </summary>
        private void ApplicationTimerTick(object sender, ElapsedEventArgs e)
        {
            if (!m_isKeyHeld)
            {
                return;
            }
            
            // If user held key for longer than 1 second...
            if ((DateTime.Now - m_pressedKeyTime).TotalSeconds > 1)
            {
                // Reset action properties
                m_ignoreKeyRelease = true;
                m_numberOfPresses = 0;
                m_hasDoubleClicked = false;
                
                // Reset settings
                m_settings.DeathCount = 0;
                m_settings.TotalSeconds = 0;
                m_settings.IsRunning = 0;
                m_settings.IsPaused = 0;
                SaveSettings();

                // Reset action context
                Connection.SetImageAsync(Image.FromFile("images\\actions\\roshan-timer@2x.png"));
                Connection.SetTitleAsync(String.Empty);
                
                // Re-create timers
                Dispose();
                CreateApplicationTimer();
                CreateRoshanTimer();
                
                if (m_settings.IsPaused == 0)
                {
                    m_roshanTimer.Start();
                }
            }
        }

        /// <summary>
        /// Creates a Roshan timer in an idle state (Not started until user input).
        /// The Roshan timer starts on first user input, see KeyReleased() 'isRunning' flag.
        /// </summary>
        private void CreateRoshanTimer()
        {
            m_roshanTimer = new Timer();
            m_roshanTimer.Elapsed += RoshanTimerTick;
            m_roshanTimer.AutoReset = true;
            m_roshanTimer.Interval = 1000; // Tick one per second
        }

        /// <summary>
        /// An tick method (Invoked 1 times per second by default) for the Roshan timer timer.
        /// Used for updating the timer, and changing the Roshan art and text depending on the current settings.
        /// </summary>
        private void RoshanTimerTick(object sender, ElapsedEventArgs e)
        {
            if (m_settings.IsRunning == 1)
            {
                m_settings.TotalSeconds++;
                m_settings.LastVisibleTime = DateTime.Now;
                SaveSettings();
                CalculateRoshanContext(m_settings.DeathCount, m_settings.TotalSeconds);
            }
        }

        public override void KeyPressed(KeyPayload payload)
        {
            CalculateRoshanContext(m_settings.DeathCount, m_settings.TotalSeconds);
            
            if (m_numberOfPresses > 1)
            {
                // If user last keystroke was less than than 0.4f second ago...
                if ((DateTime.Now - m_releasedKeyTime).TotalSeconds < 0.4f)
                {
                    // User has double clicked!
                    m_numberOfPresses = 0;
                    m_hasDoubleClicked = true;
                    
                    // Debug time between double click keys:
                    // Manager.SetImageAsync(args.context, "images/blank.png");
                    // Manager.SetTitleAsync(args.context, (DateTime.Now - releasedKeyTime).TotalSeconds.ToString("F2"));
                }
            }
            
            m_pressedKeyTime = DateTime.Now;
            m_isKeyHeld = true;
        }

        public override void KeyReleased(KeyPayload payload)
        {
            m_numberOfPresses++;
            m_isKeyHeld = false;
            m_releasedKeyTime = DateTime.Now;
            
            if (m_hasDoubleClicked)
            {
                m_hasDoubleClicked = false;
                
                // Resume
                m_roshanTimer.Start();
                
                // Update settings
                m_settings.DeathCount++;
                m_settings.TotalSeconds = 0; // Reset timer
                SaveSettings();
                
                // Update visuals
                CalculateRoshanContext(m_settings.DeathCount, m_settings.TotalSeconds);
                return;
            }

            // Ignore affecting the Roshan timer play/pause when restarting the action via long press.
            if (m_ignoreKeyRelease)
            {
                m_ignoreKeyRelease = false;
                return;
            }
            
            // First run
            if (m_settings.IsRunning == 0)
            {
                m_settings.IsRunning = 1; // Start Roshan timer via flag
                SaveSettings();
                return;
            }
            
            // Play / Pause Roshan Timer
            if (m_settings.IsPaused == 1)
            {
                m_settings.IsPaused = 0;
                SaveSettings();
                
                m_roshanTimer.Start();
                Connection.SetTitleAsync(GetFormattedString(m_settings.TotalSeconds));
            }
            else
            {
                m_settings.IsPaused = 1;
                SaveSettings();
                
                m_roshanTimer.Stop();
                Connection.SetTitleAsync("Paused");
            }
        }
        
        /// <summary>
        /// Sets the action image and text depending on the provided variables (usually pulled from settings).
        /// </summary>
        /// <param name="deathCount"></param>
        /// <param name="totalSeconds"></param>
        private void CalculateRoshanContext(int deathCount = 0, int totalSeconds = 0)
        {
            int totalMinutes = totalSeconds / 60;

            Image defaultContext = Image.FromFile("images\\actions\\dead3.png");
            
            if (totalMinutes < 8)
            {
                Connection.SetImageAsync(deathCount <= 3 
                    ? Image.FromFile("images\\actions\\dead" + deathCount + ".png") : defaultContext);
            }
            else if (totalMinutes < 11)
            {
                Connection.SetImageAsync(deathCount <= 3 
                    ? Image.FromFile("images\\actions\\maybe" + deathCount + ".png") : defaultContext);
            }
            else
            {
                Connection.SetImageAsync(deathCount <= 3 
                    ? Image.FromFile("images\\actions\\alive" + deathCount + ".png") : defaultContext);
            } 
            
            Connection.SetTitleAsync(GetFormattedString(m_settings.TotalSeconds));
        }

        /// <summary>
        /// Returns a formatted time string (such as '0:00') using the provided totalSeconds variable.
        /// </summary>
        /// <param name="totalSeconds"></param>
        /// <returns></returns>
        private string GetFormattedString(int totalSeconds)
        {
            int totalMinutes = totalSeconds / 60;
            if (totalMinutes == 0)
            {
                return totalMinutes + ":" + totalSeconds.ToString("00");
            }
            
            return totalMinutes + ":" + (totalSeconds - totalMinutes * 60).ToString("00");
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Tools.AutoPopulateSettings(m_settings, payload.Settings);
            SaveSettings();
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        public override void OnTick() { }

        public override void Dispose()
        {
            // Unsubscribe
            m_applicationTimer.Elapsed -= ApplicationTimerTick;
            m_roshanTimer.Elapsed -= RoshanTimerTick;
            
            // Dispose
            m_applicationTimer.Dispose();
            m_roshanTimer.Dispose();
        }

        private void SaveSettings()
        {
            Connection.SetSettingsAsync(JObject.FromObject(m_settings));
        }
    }
}