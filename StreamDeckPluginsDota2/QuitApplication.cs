using System.Diagnostics;
using BarRaider.SdTools;

namespace StreamDeckPluginsDota2
{
    [PluginActionId("com.adrian-miasik.sdpdota2.quit-application")]
    public class QuitApplication : PluginBase
    {
        public QuitApplication(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            
        }

        public override void KeyPressed(KeyPayload payload)
        {
            Process[] dotaProcesses = Process.GetProcessesByName("Dota2");

            foreach (Process process in dotaProcesses)
            {
                process.Kill();
            }
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