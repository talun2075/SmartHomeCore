using InnerCore.Api.DeConz.Models.Groups;

namespace SmartHome.Classes.SmartHome.Data
{
    public class SmartHomeRoom
    {
        public Group Room { get; set; }
        public bool Hide { get; set; } = false;
        public int SortOrder { get; set; } = 100;
    }
}