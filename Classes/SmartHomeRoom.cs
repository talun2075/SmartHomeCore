using InnerCore.Api.DeConz.Models.Groups;
using System;

namespace SmartHome.Classes
{
    public class SmartHomeRoom
    {
        public Group Room { get; set; }
        public Boolean Hide { get; set; } = false;
        public int SortOrder { get; set; } = 100;
    }
}