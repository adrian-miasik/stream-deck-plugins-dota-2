using BarRaider.SdTools;
using WindowsInput.Native;

namespace StreamDeckPluginsDota2
{
    [PluginActionId("com.adrian-miasik.sdpdota2.show-bot-rune")]
    public class ShowBotRuneAction : RuneBase
    {
        public ShowBotRuneAction(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
        }

        public override void KeyPressed(KeyPayload payload)
        {
            // TODO: Pull string from cfg file
            InputSimulator.Keyboard.KeyPress(VirtualKeyCode.F15);
        }

        public override void KeyReleased(KeyPayload payload)
        {
            InputSimulator.Keyboard.KeyPress(VirtualKeyCode.F13);
        }
    }
}