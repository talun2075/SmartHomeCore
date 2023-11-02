using SmartHome.Classes.Aurora.Core.DataClasses;
using System;
using System.Collections.Generic;

namespace SmartHome.Classes.Aurora.Core
{
    /// <summary>
    /// Class for your Knowing Devices.
    /// </summary>
    public class AuroraKnowingDevices
    {
        public AuroraKnowingDevices() { }

        public AuroraKnowingDevices(string _MacAdress, string _AuthToken, string _DeviceName, bool _useTouch = false, bool _useSubscription = false)
        {
            MacAdress = _MacAdress;
            AuthToken = _AuthToken;
            DeviceName = _DeviceName;
            UseSubscription = _useSubscription;
            UseTouch = _useTouch;
        }
        public AuroraKnowingDevices(string _MacAdress, string _AuthToken, string _DeviceName, string IP, bool _useTouch = false, bool _useSubscription = false)
        {
            MacAdress = _MacAdress;
            AuthToken = _AuthToken;
            DeviceName = _DeviceName;
            KnowingIP = IP;
            UseSubscription = _useSubscription;
            UseTouch = _useTouch;
        }
        public AuroraKnowingDevices(string _MacAdress, string _AuthToken, string _DeviceName, string IP, string serial, bool _useTouch = false, bool _useSubscription = false)
        {
            MacAdress = _MacAdress;
            AuthToken = _AuthToken;
            DeviceName = _DeviceName;
            KnowingIP = IP;
            Serial = serial;
            UseSubscription = _useSubscription;
            UseTouch = _useTouch;
        }
        public AuroraKnowingDevices(string _MacAdress, string _AuthToken, string _DeviceName, string IP, string serial, List<TouchData> _touchdata, bool _useTouch = false, bool _useSubscription = false)
        {
            MacAdress = _MacAdress;
            AuthToken = _AuthToken;
            DeviceName = _DeviceName;
            KnowingIP = IP;
            Serial = serial;
            UseSubscription = _useSubscription;
            UseTouch = _useTouch;
            TouchDatas = _touchdata;
        }
        /// <summary>
        /// Knowing Mac Adress
        /// </summary>
        public string MacAdress { get; set; }
        /// <summary>
        /// Used AuthToken
        /// </summary>
        public string AuthToken { get; set; }
        /// <summary>
        /// Internal Name we Use
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// Internal IP we Use
        /// </summary>
        public string KnowingIP { get; set; }
        /// <summary>
        /// Knowing Serial of the Device
        /// </summary>
        public string Serial { get; set; }
        /// <summary>
        /// Room to group
        /// </summary>
        public string Room { get; set; }
        /// <summary>
        /// Should we use SSE (Server Sent Event from Aurora to Server that we can react on it.)
        /// </summary>
        public bool UseSubscription { get; private set; } = false;
        /// <summary>
        /// Should we use TouchEvents
        /// </summary>
        public bool UseTouch { get; set; } = false;

        public List<TouchData> TouchDatas { get; set; } = new();
    }
}
