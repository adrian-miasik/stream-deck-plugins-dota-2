using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Timers;

namespace StreamDeckPluginsDota2
{
    [PluginActionId("com.adrian-miasik.sdpdota2.roshan-timer")]
    public class PluginAction : PluginBase
    {
        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                PluginSettings instance = new PluginSettings
                {
                    TotalSeconds = 0
                };
                return instance;
            }

            [JsonProperty(PropertyName = "totalSeconds")]
            public int TotalSeconds { get; set; }
        }
        
        private PluginSettings settings;
        
        private Timer applicationTimer; // Used for processing input. Cannot be paused.
        
        // Hold
        private bool isKeyHeld;
        private DateTime pressedKeyTime;
        private bool ignoreKeyRelease;

        // Double Click
        private int numberOfPresses;
        private DateTime releasedKeyTime;
        private bool hasDoubleClicked;
        
        // Roshan
        private Timer roshanTimer; // Used for keeping track of roshan's respawn time. Can be paused.
        private bool isRoshanTimerPaused;
        // TODO: Expose deathCount in inspector property field
        private int deathCount;
        
        public PluginAction(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                settings = PluginSettings.CreateDefaultSettings();
                SaveSettings();
            }
            else
            {
                settings = payload.Settings.ToObject<PluginSettings>();
            }
        }

        public override void KeyPressed(KeyPayload payload)
        {
            if (numberOfPresses > 1)
            {
                // If user last keystroke was less than than 0.4f second ago...
                if ((DateTime.Now - releasedKeyTime).TotalSeconds < 0.4f)
                {
                    // User has double clicked!
                    numberOfPresses = 0;
                    hasDoubleClicked = true;
                    
                    // Debug time between double click keys:
                    // Manager.SetImageAsync(args.context, "images/blank.png");
                    // Manager.SetTitleAsync(args.context, (DateTime.Now - releasedKeyTime).TotalSeconds.ToString("F2"));
                }
            }
            
            pressedKeyTime = DateTime.Now;
            isKeyHeld = true;
        }

        public override void KeyReleased(KeyPayload payload)
        {
            numberOfPresses++;
            releasedKeyTime = DateTime.Now;

            isKeyHeld = false;

            if (hasDoubleClicked)
            {
                hasDoubleClicked = false;
                deathCount++;
                settings.TotalSeconds = 0; // Reset timer
                ResumeRoshanTimer();
                CalculateRoshanContext(settings.TotalSeconds);
                return;
            }

            // Ignore re-init when the user was holding to reset the roshan timer/app.
            if (ignoreKeyRelease)
            {
                ignoreKeyRelease = false;
                return;
            }

            // First press - Create application timer to poll for inputs such as long presses
            if (applicationTimer == null)
            {
                applicationTimer = new Timer();
                applicationTimer.Elapsed += (sender, eventArgs) =>
                {
                    ApplicationTimerTick();
                };
                applicationTimer.AutoReset = true;
                applicationTimer.Interval = 100; // 10 ticks per second
                applicationTimer.Start();
            }
            
            // First press - Create roshan timer
            if (roshanTimer == null)
            { 
                CreateRoshanTimer();
                return;
            }
            
            // Play / Pause
            if (isRoshanTimerPaused)
            {
                ResumeRoshanTimer();
            }
            else
            {
                PauseRoshanTimer();
            }
        }

        /// <summary>
        /// A timer that gets invoked 10 times a second (by default).
        /// Used for polling user input such as button holds, and double clicks.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void ApplicationTimerTick()
        {
            if (!isKeyHeld)
            {
                return;
            }
            
            // If user held key for longer than 1 second...
            if ((DateTime.Now - pressedKeyTime).TotalSeconds > 1)
            {
                // Restart App
                ignoreKeyRelease = true;
                settings.TotalSeconds = 0;
                deathCount = 0;
                numberOfPresses = 0;
                hasDoubleClicked = false;
                
                // Dispose roshan timer
                roshanTimer.Stop();
                roshanTimer.Dispose();
                roshanTimer = null;

                // Reset action image to Roshan
                Connection.SetImageAsync(Image.FromFile("images/pluginAction@2x.png"));
                Connection.SetTitleAsync(String.Empty);
            }
        }

        private void CreateRoshanTimer()
        {
            roshanTimer = new Timer();
            roshanTimer.Elapsed += (sender, eventArgs) =>
            {
                RoshanTimerTick();
            };
            
            roshanTimer.AutoReset = true;
            roshanTimer.Interval = 1000; // Tick one per second
            roshanTimer.Start();
            
            isRoshanTimerPaused = false;
            Connection.SetTitleAsync(GetFormattedString(settings.TotalSeconds));
            Connection.SetImageAsync(Image.FromFile("images/states/dead0.png"));
        }

        private void ResumeRoshanTimer()
        {
            // Start timer again
            roshanTimer.Start();
            
            // Update state
            isRoshanTimerPaused = false;
            Connection.SetTitleAsync(GetFormattedString(settings.TotalSeconds));
        }

        private void PauseRoshanTimer()
        {
            // Stop timer
            roshanTimer.Stop();
            
            // Update state
            isRoshanTimerPaused = true;
            Connection.SetTitleAsync("Paused");
        }

        /// <summary>
        /// A timer that gets invoked 1 times a second (by default).
        /// Used for updating the timer, and changing the Roshan art depending on the timer.
        /// </summary>
        private void RoshanTimerTick()
        {
            if (roshanTimer == null)
            {
                // Early exit
                return;
            }

            settings.TotalSeconds++;
            CalculateRoshanContext(settings.TotalSeconds);
        }

        private void CalculateRoshanContext(int totalSeconds)
        {
            int totalMinutes = totalSeconds / 60;

            Image defaultContext = Image.FromFile("images/states/dead3.png");
            
            if (totalMinutes < 8)
            {
                Connection.SetImageAsync(deathCount <= 3 
                    ? Image.FromFile("images/states/dead" + deathCount + ".png") : defaultContext);
            }
            else if (totalMinutes < 11)
            {
                Connection.SetImageAsync(deathCount <= 3 
                    ? Image.FromFile("images/states/maybe" + deathCount + ".png") : defaultContext);
            }
            else
            {
                Connection.SetImageAsync(deathCount <= 3 
                    ? Image.FromFile("images/states/alive" + deathCount + ".png") : defaultContext);
            } 
            
            Connection.SetTitleAsync(GetFormattedString(settings.TotalSeconds));
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
            Tools.AutoPopulateSettings(settings, payload.Settings);
            SaveSettings();
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        public override void OnTick() { }

        public override void Dispose() { }

        private void SaveSettings()
        {
            Connection.SetSettingsAsync(JObject.FromObject(settings));
        }
    }
}