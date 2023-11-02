using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartHome.DataClasses
{
    public class OverviewRoom
    {
        public string Room { get; set; }
        public List<OverView> Controllers { get; set; } = new List<OverView>();
    }
    
    public class OverView
    {
        
        //Name="" GetCurrentValue="" On="" OnText="" Off="" OffText=""/>
        public string Name { get; set; }
        public List<string> GetCurrentValue { get; set; } = new();
        public List<string> On { get; set; } = new();
        public List<string> Off { get; set; } = new();
        public string Guid { get; set; } = System.Guid.NewGuid().ToString("N");

        public bool IsNotEmpty { get
            {
                return !string.IsNullOrEmpty(Name) && GetCurrentValue.Any() && On.Any() && Off.Any();
            } 
        }
    }
}
