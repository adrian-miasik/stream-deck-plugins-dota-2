using BarRaider.SdTools;
using WindowsInput;

namespace StreamDeckPluginsDota2
{
    // TODO: Create a base class w/ GSI integration
    public class InputSimBase : PluginBase
    {
        protected InputSimulator InputSimulator;

        protected InputSimBase(ISDConnection connection, InitialPayload payload) : base(connection, payload)
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