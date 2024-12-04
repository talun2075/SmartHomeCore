using HomeLogging;
using Microsoft.AspNetCore.Hosting;


namespace SmartHome.Classes.SmartHome.Util
{
    /// <summary>
    /// Constanten für das SmartHome
    /// </summary>
    public static class SmartHomeConstants
    {
        /// <summary>
        /// SmartHome Globaler Logger
        /// </summary>
        public static Logging log = new(new LoggerWrapperConfig() { ConfigName = "SmartHome" });
        /// <summary>
        /// Alle bekannten Buttons
        /// </summary>
        public static DeconZConstants Deconz { get; private set; } = new();
        public static IWebHostEnvironment Env { get; set; }
        
    }
    public class DeconZConstants
    {
        public string BaseUrl
        {
            get
            {
                return "deconz.tami";
            }
        }
        public int HttpPort
        {
            get
            {
                return 888;
            }
        }
        public string ApiKey
        {
            get
            {
                return "463608DCF3";
            }
        }
    }
}