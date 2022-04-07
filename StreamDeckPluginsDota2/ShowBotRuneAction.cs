using BarRaider.SdTools;
using WindowsInput;

namespace StreamDeckPluginsDota2
{
    [PluginActionId("com.adrian-miasik.sdpdota2.show-bot-rune")]
    public class ShowBotRuneAction : PluginBase
    {
        private InputSimulator m_inputSimulator;
        
        public ShowBotRuneAction(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            m_inputSimulator = new InputSimulator();
        }

        public override void KeyPressed(KeyPayload payload)
        {
            // TODO: Pull string from cfg file
            m_inputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.F14);
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