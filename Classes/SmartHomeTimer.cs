using System;

namespace SmartHome.Classes
{
    /// <summary>
    /// Interne Timer Klasse
    /// </summary>
    public class SmartHomeTimer
    {
        /// <summary>
        /// Name of this specific Timer
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// Is the Time a dedicated Time or a Repeat Time
        /// If true the Timer will start instantly and then repeat after the time
        /// </summary>
        public Boolean Repeat { get; set; } = false;
        /// <summary>
        /// The Time when the Timer starts or after which time it will restart. 
        /// </summary>
        public TimeSpan Time { get; set; }
        /// <summary>
        /// The Class where the Method is stored
        /// With complete Namespace
        /// </summary>
        public String Class { get; set; }
        /// <summary>
        /// The Method to call
        /// </summary>
        public String Method { get; set; }
        /// <summary>
        /// Optional Arguments to invoke the Method
        /// </summary>
        public string[] Arguments { get; set; }
        /// <summary>
        /// Datetime when was the last runtime
        /// </summary>
        public DateTime LastRuntime { get; set; }
        /// <summary>
        /// The Type of Timer
        /// </summary>
        public SmartHomeConstants.TimerType TimerType { get; set; } = SmartHomeConstants.TimerType.URL;
        /// <summary>
        /// The URI if Timertype is URL
        /// </summary>
        public String URI { get; set; }
        /// <summary>
        /// Only for ReflectionCall important. All other will Be Async
        /// </summary>
        public Boolean Async { get; set; } = true;
        /// <summary>
        /// Should we log the run?
        /// </summary>
        public Boolean Logging { get; set; } = false;
        public SmartHomeConstants.RequestEnums RequestTypeUrlCalls { get; set; } = SmartHomeConstants.RequestEnums.GET;
    }
}