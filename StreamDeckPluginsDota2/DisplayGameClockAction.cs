using System;
using System.Drawing;
using BarRaider.SdTools;
using BarRaider.SdTools.Wrappers;
using Dota2GSI;
using Dota2GSI.Nodes;
using WindowsInput.Native;

namespace StreamDeckPluginsDota2
{
    [PluginActionId("com.adrian-miasik.sdpdota2.display-game-clock")]
    public class DisplayGameClockAction : InputSimBase
    {
        // Define colors
        private readonly Color m_dayColor = Color.FromArgb(255, 193, 50);
        private readonly Color m_nightColor = Color.FromArgb(61, 148, 238);

        // Game state (Clock)
        private bool m_isSubscribedToGSIEvents;
        private GameState m_gameState;
        
        // Clock logic
        private int m_lastClockTime;
        private int m_currentClockTime;
        
        // Graphics
        private TitleParameters m_titleParameters;

        public DisplayGameClockAction(ISDConnection connection, InitialPayload payload) : base(connection, payload)
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

        public override void KeyPressed(KeyPayload payload) { }

        public override void KeyReleased(KeyPayload payload) { }
        
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
                Connection.SetImageAsync(Image.FromFile("images\\actions\\display-game-clock@2x.png"));
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
                        Connection.SetImageAsync(Image.FromFile("images\\actions\\display-game-clock@2x.png"));
                        Connection.SetTitleAsync(string.Empty);
                    
                        return;
                    }
                
                    m_currentClockTime = m_gameState.Map.ClockTime;
                    
                    Render();
                
                    // Cache for next tick calculation
                    m_lastClockTime = m_currentClockTime;
                }
                else
                {
                    // Otherwise: GSI is not found.
                    
                    // Set to default state
                    Connection.SetImageAsync(Image.FromFile("images\\actions\\display-game-clock@2x.png"));
                    Connection.SetTitleAsync(string.Empty);
                }
            }
        }
        
        private void Render()
        {
            // Determine day/night cycle: (If it's not night stalker ult time AND is day time. Otherwise, night time.)
            bool isDayTime = !m_gameState.Map.IsNightstalker_Night && m_gameState.Map.IsDaytime;
            
            // Create graphics
            Bitmap renderResult = Tools.GenerateGenericKeyImage(out Graphics graphics);
            
            // Render image -> background icon
            RectangleF actionBounds = new RectangleF(0, 0, 144, 144);
            Image renderImage = Image.FromFile("images\\actions\\game-clock-base@2x.png");
            graphics.DrawImage(renderImage, actionBounds);

            // Render image -> Center image
            Image centerImage;
            // If night stalker ult...
            if (m_gameState.Map.IsNightstalker_Night)
            {
                // Night time - Nightstalker ult
                centerImage = Image.FromFile("images\\actions\\night-stalker-ultimate.png");
            }
            // Otherwise...regular clock
            else
            {
                if (m_gameState.Map.IsDaytime)
                {
                    // Day time
                    centerImage = Program.GenerateSolidColorImage(144, 144, m_dayColor);
                }
                else
                {
                    // Night time
                    centerImage = Program.GenerateSolidColorImage(144, 144, m_nightColor);
                }
            }
            RectangleF imageCenterBounds = new RectangleF(4, 4, 136, 92);
            graphics.DrawImage(centerImage, imageCenterBounds);

            // Render text -> Clock time
            Brush brush = new SolidBrush(Color.FromArgb(175, 0, 0, 0));
            graphics.FillRectangle(brush, new Rectangle(0, 54 + 6, 144, 36));
            m_titleParameters = new TitleParameters(FontFamily.GenericSansSerif, FontStyle.Bold, 18, 
                isDayTime ? m_dayColor : m_nightColor, false, TitleVerticalAlignment.Middle);
            graphics.AddTextPath(m_titleParameters, 150, 144, 
                Program.GetFormattedString(m_currentClockTime));
                
            // Set/Render graphics
            Connection.SetImageAsync(renderResult);
                
            // Dispose
            graphics.Dispose();
        }
        
        public override void Dispose()
        {
            if (Program.isGSIInitialized() && m_isSubscribedToGSIEvents)
            {
                // Unsub
                Program.m_gameStateListener.NewGameState -= OnNewGameState;
                m_gameState = null;
                m_isSubscribedToGSIEvents = false;
                
                Console.WriteLine("DisplayGameClockAction has unsubscribed from GSI.");
            }
        }
    }
}