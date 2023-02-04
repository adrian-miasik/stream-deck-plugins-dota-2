using BarRaider.SdTools;
using Dota2GSI;

namespace StreamDeckPluginsDota2
{
    [PluginActionId("com.adrian-miasik.sdpdota2.display-game-time")]
    public class DisplayGameTime : PluginBase
    {
        public DisplayGameTime(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
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

        private void GSIOnNewGameState(GameState gamestate)
        {
            if (gamestate.Map.ClockTime == -1)
            {
                Connection.SetTitleAsync("Idle");
                return;
            }
            
            Connection.SetTitleAsync(Program.GetFormattedString(gamestate.Map.ClockTime));
        }

        public override void KeyPressed(KeyPayload payload) { }

        public override void KeyReleased(KeyPayload payload) { }
        
        public override void ReceivedSettings(ReceivedSettingsPayload payload) { }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        public override void OnTick() { }

        public override void Dispose()
        {
            Program._gsi.NewGameState -= GSIOnNewGameState;
        }
    }
}