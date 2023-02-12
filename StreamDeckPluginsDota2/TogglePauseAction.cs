using System;
using System.Drawing;
using BarRaider.SdTools;
using Dota2GSI;
using Dota2GSI.Nodes;
using WindowsInput.Native;

namespace StreamDeckPluginsDota2
{
    [PluginActionId("com.adrian-miasik.sdpdota2.pause-match-toggle")]
    public class TogglePauseAction : InputSimBase
    {
        // Game state integration
        private bool m_isSubscribedToGSIEvents;
        private GameState m_gameState;
        
        // Clock logic
        private int m_lastClockTime;
        private int m_currentClockTime;
        
        public TogglePauseAction(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            // Attempt to subscribe to GSI events
            CheckGSI();
        }

        /// <summary>
        /// Subscribes this class to GSI event callback (OnNewGameState) if possible.
        /// </summary>
        private void CheckGSI()
        {
            // Initialize GSI / Subscribe to GSI event
            if (!m_isSubscribedToGSIEvents)
            {
                // If GSI hasn't been started...
                if(!Program.isGSIInitialized())
                {
                    // Attempt to start GSI...
                    if (Program.InitializeGSI())
                    {
                        // On success...
                        
                        // Subscribe to GSI event
                        Program.m_gameStateListener.NewGameState += OnNewGameState;
                        m_isSubscribedToGSIEvents = true;
                        
                        Console.WriteLine("TogglePauseAction has subscribed to GSI.");
                    }
                }
                // Otherwise, GSI is ready...
                else
                {
                    // Subscribe to GSI event
                    Program.m_gameStateListener.NewGameState += OnNewGameState;
                    m_isSubscribedToGSIEvents = true;
                    
                    Console.WriteLine("TogglePauseAction has subscribed to GSI.");
                }
            }
        }

        /// <summary>
        /// Tick / Updates with new game state (Once per second it seems)
        /// </summary>
        /// <param name="gamestate"></param>
        private void OnNewGameState(GameState gamestate)
        {
            // Cache for tick check
            m_gameState = gamestate;
        }

        public override void KeyPressed(KeyPayload payload)
        {
            base.KeyPressed(payload); // Dirty key press flag
            
            // Visualize Dota and GSI states
            if (!Program.IsDotaRunning())
            {
                Connection.SetImageAsync(Program.m_grey);
            }
            else
            {
                Connection.SetImageAsync(m_gameState == null ? Program.m_yellow : Program.m_green);
                
                // If dota is not currently in focus...
                if (!Program.IsDotaFocused())
                {
                    // Focus dota:
                    // Invoking method 'SetForegroundWindow' doesn't seem to always work.
                    // To make the method more responsive, we simply press the alt key which fixes the unresponsiveness.
                    InputSimulator.Keyboard.KeyPress(VirtualKeyCode.MENU); // Workaround for 'SetForegroundWindow'.
                    Program.FocusDota();
                }
            }
        }

        public override void KeyReleased(KeyPayload payload)
        {
            base.KeyReleased(payload); // Clean key press flag
            
            // Press pause toggle hotkey
            InputSimulator.Keyboard.KeyPress(VirtualKeyCode.F16);
        }
        
        public override void ReceivedSettings(ReceivedSettingsPayload payload) { }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }
        
        // Ticks once a second
        public override void OnTick()
        {
            // Prevent tick updates when the action is being interacted with...
            if (isKeyPressed)
            {
                return;
            }
            
            if (!Program.IsDotaRunning())
            {
                // Set to default state
                Connection.SetImageAsync(Image.FromFile("images\\actions\\pause-resume-match@2x.png"));
                Connection.SetTitleAsync(string.Empty);
                
                // Dispose of GSI since it won't be needed when Dota isn't running.
                Dispose();
            }
            else
            {
                // Otherwise, Dota is running...
                
                // Initialize GSI since we'll need it.
                CheckGSI();
                
                // Sanity check GSI
                if (m_gameState != null)
                {
                    // We are not in-game...
                    if (m_gameState.Map.GameState == DOTA_GameState.Undefined)
                    {
                        // Show default state
                        Connection.SetImageAsync(Image.FromFile("images\\actions\\pause-resume-match@2x.png"));
                        Connection.SetTitleAsync(string.Empty);
                    
                        return;
                    }
                
                    m_currentClockTime = m_gameState.Map.ClockTime;
                
                    // Determine pause state: (if clock time is progressing)
                    bool isPaused = m_currentClockTime == m_lastClockTime;
                
                    // Determine day/night cycle: (If it's not night stalker ult time AND is day time. Otherwise, night time.)
                    bool isDayTime = !m_gameState.Map.IsNightstalker_Night && m_gameState.Map.IsDaytime;
                    
                    Connection.SetImageAsync(isPaused
                        ? Image.FromFile("images\\actions\\resume-match@2x.png")
                        : Image.FromFile("images\\actions\\pause-match@2x.png"));
                
                    // Cache for next tick calculation
                    m_lastClockTime = m_currentClockTime;
                }
                else
                {
                    // Otherwise: GSI is not found.
                    
                    // Set to default state
                    Connection.SetImageAsync(Image.FromFile("images\\actions\\pause-resume-match@2x.png"));
                    Connection.SetTitleAsync(string.Empty);
                }
            }
        }
        
        public override void Dispose()
        {
            if (Program.isGSIInitialized() && m_isSubscribedToGSIEvents)
            {
                // Unsub
                Program.m_gameStateListener.NewGameState -= OnNewGameState;
                m_gameState = null;
                m_isSubscribedToGSIEvents = false;

                Console.WriteLine("TogglePauseAction has unsubscribed from GSI.");
            }
        }
    }
}