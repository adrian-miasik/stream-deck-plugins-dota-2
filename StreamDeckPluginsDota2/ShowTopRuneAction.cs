using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Timers;

namespace StreamDeckPluginsDota2
{
    [PluginActionId("com.adrian-miasik.sdpdota2.show-top-rune")]
    public class ShowTopRuneAction : PluginBase
    {
        
        public ShowTopRuneAction(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            // Nothing
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

        public override void OnTick() { }

        public override void Dispose() { }
    }
}