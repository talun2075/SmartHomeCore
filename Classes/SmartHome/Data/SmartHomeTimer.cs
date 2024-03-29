﻿using System;
using SmartHome.Classes.SmartHome.Util;

namespace SmartHome.Classes.SmartHome.Data
{
    /// <summary>
    /// Interne Timer Klasse
    /// </summary>
    public class SmartHomeTimer
    {
        /// <summary>
        /// Name of this specific Timer
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Is the Time a dedicated Time or a Repeat Time
        /// If true the Timer will start instantly and then repeat after the time
        /// </summary>
        public bool Repeat { get; set; } = false;
        /// <summary>
        /// The Time when the Timer starts or after which time it will restart. 
        /// </summary>
        public TimeSpan Time { get; set; }
        /// <summary>
        /// The Method to call
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// Optional Arguments to invoke the Method
        /// </summary>
        public string[] Arguments { get; set; }
        /// <summary>
        /// Datetime when was the last runtime
        /// </summary>
        public DateTime LastRuntime { get; set; }
        /// <summary>
        /// The URI if Timertype is URL / Class of Method to call if Type INTERNAL
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Only for ReflectionCall important. All other will Be Async
        /// </summary>
        public bool Async { get; set; } = true;
        /// <summary>
        /// Should we log the run?
        /// </summary>
        public bool Logging { get; set; } = false;
        /// <summary>
        /// is the Timer Active
        /// </summary>
        public bool Active { get; set; } = true;
        public SmartHomeConstants.RequestEnums RequestTypeUrlCalls { get; set; } = SmartHomeConstants.RequestEnums.GET;
    }
}