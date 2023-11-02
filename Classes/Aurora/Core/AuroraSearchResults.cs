using System;
using System.Text.RegularExpressions;

namespace SmartHome.Classes.Aurora.Core
{
    /// <summary>
    /// Class for discovered Devices
    /// </summary>
    public class AuroraSearchResults
    {
        public AuroraSearchResults(string ip, string MacAdress, int port)
        {
            if (string.IsNullOrEmpty(ip) || string.IsNullOrEmpty(MacAdress)) throw new ArgumentNullException(nameof(ip), "ip or MACAdress is Empty");
            if (ip.StartsWith("http://"))
                ip = ip.Replace("http://", "");
            if (!Regex.IsMatch(ip, @"^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$")) throw new ArgumentOutOfRangeException(nameof(ip), ip, "This is not a IP");
            if (port == 0) throw new ArgumentOutOfRangeException(nameof(port), port, "Need Port grater then Zero");

            IP = ip;
            Port = port;
            MACAdress = MacAdress;
        }
        /// <summary>
        /// Port Default is 16021
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Ip Adress of the Aurora
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// Mac Adress of the Aurora
        /// </summary>
        public string MACAdress { get; set; }
    }

}

