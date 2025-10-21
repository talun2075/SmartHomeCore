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
        public static IWebHostEnvironment Env { get; set; }
        
    }
}