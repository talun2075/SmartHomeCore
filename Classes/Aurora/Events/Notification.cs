using SmartHome.Classes.Aurora.Core;

namespace SmartHome.Classes.Aurora.Events
{
    public class Notification
    {
        public AuroraLigth Aurora { get; set; }
        public string Message { get; set; }
        public AuroraConstants.AuroraEvents EventType { get; set; }
    }
}