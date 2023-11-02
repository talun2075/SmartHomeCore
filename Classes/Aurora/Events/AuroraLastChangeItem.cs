using System;
using System.Collections.Generic;

namespace SmartHome.Classes.Aurora.Events
{
    public class AuroraLastChangeItem
    {
        /// <summary>
        /// Serial des betrefenden Lampe
        /// </summary>
        public string Serial { get; set; }
        /// <summary>
        /// TypeEnum als String
        /// </summary>
        public string ChangeType => TypeEnum.ToString();
        /// <summary>
        /// Welches Event wurde ausgelöst
        /// </summary>
        public Core.AuroraConstants.AuroraEvents TypeEnum { get; set; }
        /// <summary>
        /// Welche Daten sollen mit gegeben werden. 
        /// Event ID und der geänderte Wert ist normal.
        /// </summary>
        public Dictionary<string, string> ChangedValues { get; set; } = new Dictionary<string, string>();
    }
}
