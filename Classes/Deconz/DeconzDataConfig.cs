using System;

namespace SmartHome.Classes.Deconz
{
    /// <summary>
    /// Dient dazu um die Oberfläche Anzupassen
    /// </summary>
    public class DeconzDataConfig
    {
        /// <summary>
        /// Ausblenden
        /// </summary>
        public bool Hide { get; set; } = false;
        /// <summary>
        /// Raum um ´den es geht.
        /// </summary>
        public int RoomID { get; set; }
        /// <summary>
        /// Reihenfolge der Ansicht.
        /// </summary>
        public int SortOrder { get; set; }
        /// <summary>
        /// Gibt die Möglichkeit, den namen zu überschreiben. 
        /// </summary>
        public bool OverWriteName { get; set; } = false;
        /// <summary>
        /// Neuer Name
        /// </summary>
        public string OverWrittenName { get; set; } = string.Empty;
    }
}