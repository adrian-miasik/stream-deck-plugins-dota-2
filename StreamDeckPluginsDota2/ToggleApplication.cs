using System.Diagnostics;
using System.Drawing;
using BarRaider.SdTools;

namespace StreamDeckPluginsDota2
{
    [PluginActionId("com.adrian-miasik.sdpdota2.toggle-application")]
    public class ToggleApplication : PluginBase
    {
        private Process[] m_dotaProcesses;
        private bool isDotaRunning;
        
        public ToggleApplication(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            
        }

        public override void KeyPressed(KeyPayload payload)
        {
            // If user pressed acton and game is running...
            if (isDotaRunning)
            {
                // Close Dota 2
                foreach (Process process in m_dotaProcesses)
                {
                    process.Kill();
                }

                isDotaRunning = false;
            }
            // Otherwise...
            else
            {
                // Open Dota 2
                Process.Start("steam://rungameid/570");
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
            m_dotaProcesses = Process.GetProcessesByName("Dota2");
            isDotaRunning = m_dotaProcesses.Length > 0;

            Connection.SetImageAsync(isDotaRunning
                ? Image.FromFile("images\\actions\\quit-application@2x.png")
                : Image.FromFile("images\\actions\\open-application@2x.png"));
        }

        public override void Dispose()
        {

        }
    }
}