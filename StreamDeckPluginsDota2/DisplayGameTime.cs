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
        private bool isDotaRunning;
        
        private Image init; 
        private Image unPaused;
        private Image paused;

        private GameState m_gameState;

        private InputSimulator InputSimulator;
        
        private TitleParameters titleParameters = new TitleParameters(FontFamily.GenericMonospace, FontStyle.Regular, 8, Color.White, true, TitleVerticalAlignment.Bottom);

        private bool isPaused;

        public DisplayGameTime(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            // Generate status images and cache
            init = GenerateImage(144, 144, Color.Orange);
            unPaused = GenerateImage(144, 144, Color.MediumSeaGreen);
            paused = GenerateImage(144, 144, Color.Crimson);
            
            // Set starting image
            // Connection.SetImageAsync(init);
            // Connection.SetTitleAsync("Standby"); // Default state
            
            // Create input sim
            InputSimulator = new InputSimulator();
            
            // Subscribe method to event
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
        /// Tick / Updates with new game state
        /// </summary>
        /// <param name="gamestate"></param>
        private void GSIOnNewGameState(GameState gamestate)
        {
            // Cache for tick check
            m_gameState = gamestate;
        }

        public override void KeyPressed(KeyPayload payload)
        {
            InputSimulator.Keyboard.KeyPress(VirtualKeyCode.F16);
        }

        public override void KeyReleased(KeyPayload payload) { }
        
        public override void ReceivedSettings(ReceivedSettingsPayload payload) { }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }
        
        // Ticks once a second
        public override async void OnTick()
        {
            // m_dotaProcesses = Process.GetProcessesByName("Dota2");
            // isDotaRunning = m_dotaProcesses.Length > 0;
            
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
            
            if (m_gameState.Previously.Map.ClockTime == -1)
            {
                await Connection.SetTitleAsync("Paused");
                await Connection.SetImageAsync(paused);
                    
                return;
            }

            // Not paused
            await Connection.SetTitleAsync(Program.GetFormattedString(m_gameState.Map.ClockTime));
            await Connection.SetImageAsync(unPaused);
            
            // await Connection.SetImageAsync(Image.FromFile("images\\actions\\display-game-time.png"));

            // await Connection.SetTitleAsync(
            // GraphicsTools.WrapStringToFitImage(m_gameState.Map.GameState.ToString(), titleParameters));
                
            // await Connection.SetTitleAsync(isPaused.ToString());
        }

        public override void Dispose()
        {
            Program._gsi.NewGameState -= GSIOnNewGameState;
        }
    }
}