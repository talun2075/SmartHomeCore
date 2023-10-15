using System;

namespace SmartHome.DataClasses
{
    public class ButtonRequest
    {

        public string Mac { get; set; }
        public string Battery { get; set; }
        public string Action { get; set; }
        public string Wheel { get; set; }
        public string IP { get; set; }

    }
}