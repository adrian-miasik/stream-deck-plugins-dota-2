using System;
using System.Diagnostics;
using System.Drawing;
using BarRaider.SdTools;
using BarRaider.SdTools.Wrappers;
using Dota2GSI;
using WindowsInput;
using WindowsInput.Native;

namespace StreamDeckPluginsDota2
{
    [PluginActionId("com.adrian-miasik.sdpdota2.display-game-time")]
    public class DisplayGameTime : PluginBase
    {
        private Process[] m_dotaProcesses;
        private bool m_isDotaRunning;

        private readonly Image paused;
        private readonly Image m_dayImage;
        private readonly Image m_nightImage;

        private GameState m_gameState;
        private int m_lastClockTime;
        private int m_currentClockTime;

        private readonly InputSimulator m_inputSimulator;
        private TitleParameters m_titleParameters;
        private FontFamily heeboBold;
        
        // Define colors
        private readonly Color m_pauseColor = Color.FromArgb(210, 40, 40);
        private readonly Color m_dayColor = Color.FromArgb(255, 193, 50);
        private readonly Color m_nightColor = Color.FromArgb(37, 64, 112);

        public DisplayGameTime(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            // TODO: Test font / ship in folder?
            // Fetch Heebo font
            heeboBold = new FontFamily("Heebo");
            m_titleParameters = new TitleParameters(heeboBold, FontStyle.Bold, 20, Color.White, true, TitleVerticalAlignment.Middle);
            
            // Generate assets and cache images
            paused = GenerateImage(144, 144, m_pauseColor);
            m_dayImage = GenerateImage(144, 144, m_dayColor);
            m_nightImage = GenerateImage(144, 144, m_nightColor);

            // Create input sim for pausing
            m_inputSimulator = new InputSimulator();
            
            // Subscribe GSI method
            Program._gsi.NewGameState += GSIOnNewGameState;
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
        private void GSIOnNewGameState(GameState gamestate)
        {
            // Cache for tick check
            m_gameState = gamestate;
        }

        public override void KeyPressed(KeyPayload payload)
        {
            m_inputSimulator.Keyboard.KeyPress(VirtualKeyCode.F16);
        }

        public override void KeyReleased(KeyPayload payload) { }
        
        public override void ReceivedSettings(ReceivedSettingsPayload payload) { }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }
        
        // Ticks once a second
        public override void OnTick()
        {
            // if (!Program.IsDotaRunning())
            // {
            //     // Directly set image + title to starting states
            //     Connection.SetImageAsync(Image.FromFile("images\\actions\\display-game-time.png"));
            //     Connection.SetTitleAsync(string.Empty);
            //     
            //     // Early exit
            //     return;
            // }

            // Text title and Image we are going to render later
            string renderString;
            Image renderImage;
            
            m_currentClockTime = m_gameState.Map.ClockTime;
            
            // Determine pause state: (if clock time is progressing)
            bool isPaused = m_currentClockTime == m_lastClockTime;

            // Determine day/night cycle: (If it's not night stalker ult time AND is day time. Otherwise, night time.)
            bool isDayTime = !m_gameState.Map.IsNightstalker_Night && m_gameState.Map.IsDaytime;
            
            int resultTitleFontSize;
            
            // If time isn't progressing...
            if (isPaused)
            {
                renderString = "Unpause";
                renderImage = paused;
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
            
            // Define render
            Bitmap renderResult = Tools.GenerateGenericKeyImage(out Graphics g);

            // Define tools
            Point point = new Point(0, 0);
            // Pen pen = new Pen(Color.Yellow);
            Brush brush = new SolidBrush(Color.Black);
            
            // Render image
            g.DrawImage(renderImage, point);
            
            // Define rectangle
            // Rectangle square = new Rectangle(0, 0, 144, 144); // (Top left expanding down right?)
            
            // Draw said filled Rectangle
            // g.DrawRectangle(pen, square); // Outline
            g.FillRectangle(brush, new Rectangle(0, 40, 144, 55));
            
            // Draw/Render Text to graphic
            m_titleParameters = new TitleParameters(heeboBold, FontStyle.Bold, resultTitleFontSize, Color.White, true, TitleVerticalAlignment.Middle);
            Tools.AddTextPathToGraphics(g, m_titleParameters, 120, 144, renderString, 10);

            // Render / Set image
            Connection.SetImageAsync(renderResult);

            // Dispose
            g.Dispose();

            // Cache for next tick
            m_lastClockTime = m_currentClockTime;
            
            // // If clock is returning nothing...
            // if (gamestate.Map.ClockTime == -1)
            // {
            //     // Set starting image
            //     Connection.SetImageAsync(Image.FromFile("images\\actions\\display-game-time.png"));
            //     Connection.SetTitleAsync("Standby"); // Default state
            //     
            //     Console.WriteLine("Clock isn't ready.");
            //     return;
            // }
            
            // if (m_gameState.Previously.Map.ClockTime == -1)
            // {
            //     await Connection.SetTitleAsync("Standby");
            //     await Connection.SetImageAsync(init);
            //     
            //     return;
            // }

            // await Connection.SetImageAsync(Image.FromFile("images\\actions\\display-game-time.png"));

            // await Connection.SetTitleAsync(
            // GraphicsTools.WrapStringToFitImage(m_gameState.Map.GameState.ToString(), titleParameters));
                
            // await Connection.SetTitleAsync(isPaused.ToString());
        }

        public override void Dispose()
        {
            Program._gsi.NewGameState -= GSIOnNewGameState;
            
            Logger.Instance.LogMessage(TracingLevel.INFO, "Disposed of the DisplayGameTime action.");
        }
    }
}