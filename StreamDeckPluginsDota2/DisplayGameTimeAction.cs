using System.Drawing;
using BarRaider.SdTools;
using BarRaider.SdTools.Wrappers;
using Dota2GSI;
using Dota2GSI.Nodes;
using WindowsInput.Native;

namespace StreamDeckPluginsDota2
{
    [PluginActionId("com.adrian-miasik.sdpdota2.display-game-time")]
    public class DisplayGameTimeAction : InputSimBase
    {
        // Define colors
        private readonly Color m_pauseColor = Color.FromArgb(210, 40, 40);
        private readonly Color m_runningColor = Color.FromArgb(42, 168, 67);
        private readonly Color m_dayColor = Color.FromArgb(255, 193, 50);
        private readonly Color m_nightColor = Color.FromArgb(61, 148, 238);
        
        // Images
        private readonly Image m_paused;
        private readonly Image m_running;
        private readonly Image m_dayImage;
        private readonly Image m_nightImage;

        // Game state (Clock)
        private bool m_isSubscribedToGSIEvents;
        private GameState m_gameState;
        private int m_lastClockTime;
        private int m_currentClockTime;
        
        // Graphics
        private TitleParameters m_titleParameters;
        private FontFamily heeboFont;

        public DisplayGameTimeAction(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            // TODO: Use heebo font if found...
            // Fetch Heebo font
            // heeboFont = new FontFamily("Heebo");

            // Generate assets and cache images
            m_paused = GenerateImage(144, 144, m_pauseColor);
            m_running = GenerateImage(144, 144, m_runningColor);
            m_dayImage = GenerateImage(144, 144, m_dayColor);
            m_nightImage = GenerateImage(144, 144, m_nightColor);
            
            CheckGSI();
        }

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
                    }
                }
                // Otherwise, GSI is ready...
                else
                {
                    // Subscribe to GSI event
                    Program.m_gameStateListener.NewGameState += OnNewGameState;
                    m_isSubscribedToGSIEvents = true;
                }
            }
        }
        
        private Image GenerateImage(int width, int height, Color color)
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
            InputSimulator.Keyboard.KeyPress(VirtualKeyCode.F16);
            Connection.SetImageAsync(m_paused);
        }

        public override void KeyReleased(KeyPayload payload) { }
        
        public override void ReceivedSettings(ReceivedSettingsPayload payload) { }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }
        
        // Ticks once a second
        public override void OnTick()
        {
            if (!Program.IsDotaRunning())
            {
                // Set to default state
                Connection.SetImageAsync(Image.FromFile("images\\actions\\display-game-time.png"));
                Connection.SetTitleAsync(string.Empty);
                
                // Early exit
                return;
            }
            
            // Otherwise, Dota is running...
            CheckGSI();
            
            // Sanity check
            if (m_gameState != null)
            {
                // GSI connected and (gamestate) is safe to use.
                
                // We are not in-game...
                if (m_gameState.Map.GameState == DOTA_GameState.Undefined)
                {
                    Connection.SetImageAsync(Image.FromFile("images\\actions\\display-game-time.png"));
                    Connection.SetTitleAsync(string.Empty);
                    
                    return;
                }
                
                m_currentClockTime = m_gameState.Map.ClockTime;
                
                // Determine pause state: (if clock time is progressing)
                bool isPaused = m_currentClockTime == m_lastClockTime;
                
                // Determine day/night cycle: (If it's not night stalker ult time AND is day time. Otherwise, night time.)
                bool isDayTime = !m_gameState.Map.IsNightstalker_Night && m_gameState.Map.IsDaytime;

                // Render/Determine action image
                Render(isPaused, isDayTime);
                
                // Cache for next tick
                m_lastClockTime = m_currentClockTime;
            }
            else
            {
                // Set to default state
                Connection.SetImageAsync(Image.FromFile("images\\actions\\display-game-time@2x.png"));
                Connection.SetTitleAsync(string.Empty);
            }
        }

        private void Render(bool isPaused, bool isDayTime)
        {
            Image renderImage;
            int resultTitleFontSize = 16;
            string renderString = Program.GetFormattedString(m_currentClockTime);
            Color renderColor = isDayTime ? m_dayColor : m_nightColor;

            // If time isn't progressing...
            if (isPaused)
            {
                renderImage = Image.FromFile("images\\actions\\resume-match@2x.png");
            }
            // Otherwise: Running...
            else
            {
                // Show current game time
                renderImage = Image.FromFile("images\\actions\\pause-match@2x.png");
            }

            // Define render
            Bitmap renderResult = Tools.GenerateGenericKeyImage(out Graphics graphics);
                
            // Define tools
            Brush darkBrush = new SolidBrush(Color.FromArgb(175, 0, 0, 0));
            // Brush lightBrush = new SolidBrush(Color.FromArgb(216, 255, 255, 255)); // 85% opacity
            // Brush dynamicBrush = isPaused ? lightBrush : darkBrush; 

            // Render image
            RectangleF imageRect = new RectangleF(0, 0, 144, 144);
            graphics.DrawImage(renderImage, imageRect);
                
            // Draw said filled Rectangle
            graphics.FillRectangle(darkBrush, new Rectangle(0, 54 + 6, 144, 36));
                
            // Draw/Render Text to graphic
            m_titleParameters = new TitleParameters(FontFamily.GenericSansSerif, FontStyle.Bold, resultTitleFontSize, 
                renderColor, false, TitleVerticalAlignment.Middle);
            graphics.AddTextPath(m_titleParameters, 150, 144, renderString);
                
            // Render graphic/Set image
            Connection.SetImageAsync(renderResult);
                
            // Dispose
            graphics.Dispose();
        }

        public override void Dispose()
        {
            if (Program.isGSIInitialized())
            {
                Program.m_gameStateListener.NewGameState -= OnNewGameState;
                m_isSubscribedToGSIEvents = false;
            }
        }
    }
}