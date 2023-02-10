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
        private readonly Color m_nightColor = Color.FromArgb(37, 64, 112);
        
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
            // TODO: Test font / ship in folder?
            // Fetch Heebo font
            heeboFont = new FontFamily("Heebo");
            
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
                Connection.SetImageAsync(Image.FromFile("images\\actions\\display-game-time.png"));
                Connection.SetTitleAsync(string.Empty);
            }
        }

        private void Render(bool isPaused, bool isDayTime)
        {
            // Declare
            string renderString;
            Image renderImage;
            int resultTitleFontSize;
                
            // If time isn't progressing...
            if (isPaused)
            {
                Connection.SetImageAsync(Image.FromFile("images\\actions\\game-paused.png"));
                Connection.SetTitleAsync(string.Empty);
            }
            // Otherwise: Running...
            else
            {
                // Show current game time
                renderString = Program.GetFormattedString(m_currentClockTime);
                renderImage = isDayTime ? m_dayImage : m_nightImage;
                resultTitleFontSize = 20;
            }

            // Define render
            Bitmap renderResult = Tools.GenerateGenericKeyImage(out Graphics graphics);
                
            // Define tools
            Point point = new Point(0, 0); 
            Brush brush = new SolidBrush(Color.Black);
                
            // Render image
            graphics.DrawImage(renderImage, point);
                
            // Draw said filled Rectangle
            // g.DrawRectangle(pen, square); // Outline
            graphics.FillRectangle(brush, new Rectangle(0, 36, 144, 72));
                
            // Draw/Render Text to graphic
            m_titleParameters = new TitleParameters(heeboFont, FontStyle.Regular, resultTitleFontSize, Color.White, true, TitleVerticalAlignment.Middle);
            graphics.AddTextPath(m_titleParameters, (int)132.5, 144, renderString, (int)22.5);
                
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