using SmartHome.Classes.ModbusWrapper.Enums;
using System;

namespace SmartHome.Classes.ModbusWrapper.Model
{
    public class Inverter
    {
        public DeyeState State { get; set; } = DeyeState.NotSet;
        public String InverterACTemperature { get; set; } = String.Empty;
        public Boolean PowerOn { get; set; }
    }
}
