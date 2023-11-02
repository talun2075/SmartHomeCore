using SmartHome.Classes.Aurora.Core.Enums;
using System;

namespace SmartHome.Classes.Aurora.Core.DataClasses
{
    public class TouchData
    {
        public int Hue { get; set; } = -1;
        public int Saturation { get; set; } = -1;

        public int Brightness { get; set; } = -1;

        public string Value { get; set; }

        public TouchEventActions EventActions { get; set; }

        public EventIDTouchAttributtes EventType { get; set; }
    }
}
