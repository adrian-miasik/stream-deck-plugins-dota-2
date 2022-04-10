using BarRaider.SdTools;
using WindowsInput;

namespace StreamDeckPluginsDota2
{
    public class RuneBase : PluginBase
    {
        public InputSimulator InputSimulator;

        public RuneBase(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            InputSimulator = new InputSimulator();
        }

        public override void KeyPressed(KeyPayload payload)
        {
        }

        public override void KeyReleased(KeyPayload payload)
        {
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload)
        {
        }

        public override void OnTick()
        {
        }

        public override void Dispose()
        {
        }
    }
}