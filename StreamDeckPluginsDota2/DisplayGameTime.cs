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