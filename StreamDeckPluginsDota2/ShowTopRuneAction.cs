using BarRaider.SdTools;

namespace StreamDeckPluginsDota2
{
    [PluginActionId("com.adrian-miasik.sdpdota2.show-top-rune")]
    public class ShowTopRuneAction : RuneBase
    {
        public ShowTopRuneAction(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
        }

        public override void KeyPressed(KeyPayload payload)
        {
            // TODO: Pull string from cfg file
            InputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.F14);
        }

        public override void KeyReleased(KeyPayload payload)
        {
            InputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.F13);
        }
    }
}