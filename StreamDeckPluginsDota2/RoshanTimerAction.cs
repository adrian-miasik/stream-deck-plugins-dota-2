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
                    IsPaused = 0
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
            
            CreateApplicationTimer();
            CreateRoshanTimer(); // This timer doesn't get started until user input
            
            if (m_settings.IsPaused == 0)
            {
                m_roshanTimer.Start();
            }
        }

        private void CreateApplicationTimer()
        {
            m_applicationTimer = new Timer();
            m_applicationTimer.Elapsed += ApplicationTimerTick;
            m_applicationTimer.AutoReset = true;
            m_applicationTimer.Interval = 100; // 10 ticks per second
            m_applicationTimer.Start();
        }

        /// <summary>
        /// A timer that gets invoked 10 times a second (by default).
        /// Used for polling user input such as button holds, and double clicks.
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
                // Restart App
                m_ignoreKeyRelease = true;
                
                m_settings.DeathCount = 0;
                m_settings.TotalSeconds = 0;
                m_settings.IsRunning = 0;
                m_settings.IsPaused = 0;
                SaveSettings();
                
                m_numberOfPresses = 0;
                m_hasDoubleClicked = false;
                
                // Reset action image to Roshan
                Connection.SetImageAsync(Image.FromFile("images\\actions\\roshan-timer@2x.png"));
                Connection.SetTitleAsync(String.Empty);
                
                Dispose();
                
                CreateApplicationTimer();
                CreateRoshanTimer();
                
                if (m_settings.IsPaused == 0)
                {
                    m_roshanTimer.Start();
                }
            }
        }

        private void CreateRoshanTimer()
        {
            m_roshanTimer = new Timer();
            m_roshanTimer.Elapsed += RoshanTimerTick;
            m_roshanTimer.AutoReset = true;
            m_roshanTimer.Interval = 1000; // Tick one per second
        }

        /// <summary>
        /// A timer that gets invoked 1 times a second (by default).
        /// Used for updating the timer, and changing the Roshan art depending on the timer.
        /// </summary>
        private void RoshanTimerTick(object sender, ElapsedEventArgs e)
        {
            if (m_settings.IsRunning == 1)
            {
                m_settings.TotalSeconds++;
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
            m_releasedKeyTime = DateTime.Now;

            m_isKeyHeld = false;

            if (m_hasDoubleClicked)
            {
                m_hasDoubleClicked = false;
                m_settings.DeathCount++;
                m_settings.TotalSeconds = 0; // Reset timer
                
                // Resume
                m_roshanTimer.Start();
                Connection.SetTitleAsync(GetFormattedString(m_settings.TotalSeconds));
                
                CalculateRoshanContext(m_settings.DeathCount, m_settings.TotalSeconds);
                SaveSettings();
                return;
            }

            // Ignore re-init when the user was holding to reset the roshan timer/app.
            if (m_ignoreKeyRelease)
            {
                m_ignoreKeyRelease = false;
                return;
            }

            if (m_settings.IsRunning == 0)
            {
                m_settings.IsRunning = 1;
                SaveSettings();
                return;
            }
            
            // Play / Pause
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