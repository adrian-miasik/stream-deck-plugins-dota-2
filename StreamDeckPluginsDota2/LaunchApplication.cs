using System.Diagnostics;
using BarRaider.SdTools;

namespace StreamDeckPluginsDota2
{
    [PluginActionId("com.adrian-miasik.sdpdota2.launch-application")]
    public class LaunchApplication : PluginBase
    {
        public LaunchApplication(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
        }

        public override void KeyPressed(KeyPayload payload)
        {
            Process.Start("steam://rungameid/570");
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