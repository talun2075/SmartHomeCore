using System.Text.Json.Serialization;


namespace SmartHome.Classes.Aurora.Core.DataClasses
{
    internal class GlobalTouch
    {
        [JsonPropertyName("touchKillSwitchOn")]
        public bool TouchKillSwitchOn { get; set; } = false;
    }

}
