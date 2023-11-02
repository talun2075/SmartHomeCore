using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Classes.ModbusWrapper.Model
{
    public class Battery
    {
        public string FilledInPercent { get; set; } = string.Empty;
        public string Temperatur { get; set; } = string.Empty;
        public int CurrentPower { get; set; } = 0;
        public string CurrentVoltage { get; set; } = string.Empty;
    }
}
