using System;

namespace SmartHome.Classes.SmartHome.Data
{
    public class Button
    {
        public Button()
        {

        }
        public Button(string _mac)
        {
            Mac = _mac;
        }
        public Button(string _mac, string _name)
        {
            Mac = _mac;
            Name = _name;
        }
        public Button(string _mac, int _battery, ButtonAction _action)
        {
            Mac = _mac;
            Batterie = _battery;
            LastAction = _action;
        }
        /// <summary>
        /// Mac des Button
        /// </summary>
        public string Mac { get; set; }
        /// <summary>
        /// Name des Button
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// IP des Button
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// Batteriestatus in Prozent
        /// </summary>
        public int Batterie { get; set; } = -1;
        /// <summary>
        /// letzte Bekannte Action (geliefert über Generic)
        /// </summary>
        public ButtonAction LastAction { get; set; } = ButtonAction.NotSet;
        /// <summary>
        /// Ist dieser Button aktiv im Einsatz
        /// </summary>
        public bool Aktiv { get; set; } = false;
        /// <summary>
        /// Wann wurde der Button das letzte mal gedrückt
        /// </summary>
        public DateTime LastClick { get; set; }
        /// <summary>
        /// soll der Button an der Oberfläche gerendert werden.
        /// </summary>
        public bool Visible { get; set; }
    }
    /// <summary>
    /// Enum, der die Action bei den Generic aufruf mitteilt. 
    /// </summary>
    public enum ButtonAction
    {
        SINGLE = 1,
        DOUBLE = 2,
        LONG = 3,
        TOUCH = 4,
        WHEEL = 5,
        WHEEL_FINAL = 11,
        BATTERY = 6,
        NotSet = 7
    }
}