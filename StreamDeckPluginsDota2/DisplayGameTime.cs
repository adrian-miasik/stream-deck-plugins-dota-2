using System.Drawing;
using BarRaider.SdTools;
using BarRaider.SdTools.Wrappers;
using Dota2GSI;
using Dota2GSI.Nodes;
using WindowsInput;
using WindowsInput.Native;

namespace StreamDeckPluginsDota2
{
    // TODO: Use RuneBase but rename
    [PluginActionId("com.adrian-miasik.sdpdota2.display-game-time")]
    public class DisplayGameTime : PluginBase
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

        // Keyboard Interaction
        private readonly InputSimulator m_inputSimulator;
        
        // Graphics
        private TitleParameters m_titleParameters;
        private FontFamily heeboBold;

        public DisplayGameTime(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            // TODO: Test font / ship in folder?
            // Fetch Heebo font
            heeboBold = new FontFamily("Heebo");
            m_titleParameters = new TitleParameters(heeboBold, FontStyle.Bold, 20, Color.White, true, TitleVerticalAlignment.Middle);
            
            // Generate assets and cache images
            m_paused = GenerateImage(144, 144, m_pauseColor);
            m_running = GenerateImage(144, 144, m_runningColor);
            m_dayImage = GenerateImage(144, 144, m_dayColor);
            m_nightImage = GenerateImage(144, 144, m_nightColor);
            
            // Create input sim for pausing
            m_inputSimulator = new InputSimulator();
            
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
            m_inputSimulator.Keyboard.KeyPress(VirtualKeyCode.F16);
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
            
                // Text title and Image we are going to render later
                string renderString;
                Image renderImage;
                
                // Determine pause state: (if clock time is progressing)
                bool isPaused = m_currentClockTime == m_lastClockTime;
                
                // Determine day/night cycle: (If it's not night stalker ult time AND is day time. Otherwise, night time.)
                bool isDayTime = !m_gameState.Map.IsNightstalker_Night && m_gameState.Map.IsDaytime;
                
                int resultTitleFontSize;
                
                // If time isn't progressing...
                if (isPaused)
                {
                    renderString = "Unpause";
                    renderImage = m_paused;
                    resultTitleFontSize = 14;
                }
                // Not paused
                else
                {
                    // Show current game time
                    renderString = Program.GetFormattedString(m_currentClockTime);
                    renderImage = isDayTime ? m_dayImage : m_nightImage;
                    resultTitleFontSize = 20;
                }

                Connection.SetImageAsync(renderImage);
                Connection.SetTitleAsync(renderString);
                
                // Define render
                // Bitmap renderResult = Tools.GenerateGenericKeyImage(out Graphics graphics);
                
                // Draw/Render Text to graphic
                // m_titleParameters = new TitleParameters(heeboBold, FontStyle.Bold, resultTitleFontSize, Color.White, true, TitleVerticalAlignment.Middle);
                // Tools.AddTextPathToGraphics(graphics, m_titleParameters, 120, 144, renderString, 10);
                
                // Dispose
                // graphics.Dispose();
                
                //
                // // Define tools
                // Point point = new Point(0, 0);
                // // Pen pen = new Pen(Color.Yellow);
                // Brush brush = new SolidBrush(Color.Black);
                //
                // // Render image
                // graphics.DrawImage(renderImage, point);
                //
                // // Define rectangle
                // // Rectangle square = new Rectangle(0, 0, 144, 144); // (Top left expanding down right?)
                //
                // // Draw said filled Rectangle
                // // g.DrawRectangle(pen, square); // Outline
                // graphics.FillRectangle(brush, new Rectangle(0, 40, 144, 55));
                //
                // // Draw/Render Text to graphic
                // m_titleParameters = new TitleParameters(heeboBold, FontStyle.Bold, resultTitleFontSize, Color.White, true, TitleVerticalAlignment.Middle);
                // Tools.AddTextPathToGraphics(graphics, m_titleParameters, 120, 144, renderString, 10);
                //
                // // Render / Set image
                // Connection.SetImageAsync();
                //
                // // Dispose
                // graphics.Dispose();
                
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