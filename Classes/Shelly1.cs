
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SmartHome.Classes
{
    public class Shelly1
    {
        //todo: /settings abbilden

        /*
        mqtt
        coiot
         ext_switch
        actions
        ext_sensors
        ext_temperature
        ext_humidity
         */
        [JsonPropertyName("relays")]
        public List<ShellyRelay> Relays = new ();
        [JsonPropertyName("device")]
        public ShellyDevice Device = new ();
        [JsonPropertyName("wifi_ap")]
        public ShellyWifiAp WifiAccessPoint = new ();
        [JsonPropertyName("wifi_sta")]
        public ShellyWifiStatus WifiState = new ();
        [JsonPropertyName("wifi_sta1")]
        public ShellyWifiStatus WifiState1 = new ();
        [JsonPropertyName("login")]
        public ShellyLogin Login = new ();
        [JsonPropertyName("cloud")]
        public ShellyCloud Cloud = new ();
        [JsonPropertyName("hwinfo")]
        public ShellyHardwareInfo HardwareInfo = new ();
        [JsonPropertyName("build_info")]
        public ShellyBuildInfo BuildInfo = new ();
        [JsonPropertyName("sntp")]
        public ShellySntp SNTP = new ();

        [JsonPropertyName("pin_code")]
        public string PinCode { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("fw")]
        public string Firmware { get; set; }
        [JsonPropertyName("timezone")]
        public string TimeZone { get; set; }
        [JsonPropertyName("lat")]
        public Double Lat { get; set; }
        [JsonPropertyName("lng")]
        public Double Lng { get; set; }
        [JsonPropertyName("tzautodetect")]
        public Boolean TimeZoneAutoDetect { get; set; }
        [JsonPropertyName("tz_utc_offset")]
        public Double TimeZoneUTCOffset { get; set; }
        [JsonPropertyName("time")]
        public string Time { get; set; }
        [JsonPropertyName("unixtime")]
        public Double UnixTime { get; set; }
        [JsonPropertyName("ext_switch_enable")]
        public Boolean Ext_switch_enable { get; set; }
        [JsonPropertyName("ext_switch_reverse")]
        public Boolean Ext_switch_reverse { get; set; }
        [JsonPropertyName("mode")]
        public string Mode { get; set; }
        [JsonPropertyName("wifirecovery_reboot_enabled")]
        public Boolean WifirecoveryRebootEnabled { get; set; }
        [JsonPropertyName("longpush_time")]
        public Double LongpushTime { get; set; }
        [JsonPropertyName("tz_dst")]
        public Boolean TimeZoneDestination { get; set; }
        [JsonPropertyName("tz_dst_auto")]
        public Boolean TimeZoneDestinationAuto { get; set; }
        [JsonPropertyName("factory_reset_from_switch")]
        public Boolean FactoryResetFromSwitch { get; set; }
        [JsonPropertyName("discoverable")]
        public Boolean Discoverable { get; set; }

    }
    #region Shelly Data Classes
    public class ShellyHardwareInfo
    {
        [JsonPropertyName("hw_revision")]
        public String HardwareVersion { get; set; }
        [JsonPropertyName("batch_id")]
        public Double BatchId { get; set; }
    }
    public class ShellyBuildInfo
    {
        [JsonPropertyName("build_id")]
        public String ID { get; set; }
        [JsonPropertyName("build_timestamp")]
        public DateTime Timestamp { get; set; }
        [JsonPropertyName("build_version")]
        public String Version { get; set; }
    }
    public class ShellyLogin
    {
        [JsonPropertyName("enabled")]
        public Boolean Enabled { get; set; }
        [JsonPropertyName("unprotected")]
        public Boolean Unprotected { get; set; }
        [JsonPropertyName("username")]
        public string UserName { get; set; }
    }
    public class ShellySntp{
        [JsonPropertyName("enabled")]
        public Boolean Enabled { get; set; }
        [JsonPropertyName("server")]
        public string Server { get; set; }
    }
    public class ShellyCloud
    {
        [JsonPropertyName("enabled")]
        public Boolean Enabled { get; set; }
        [JsonPropertyName("connected")]
        public Boolean Connected { get; set; }
    }
    public class ShellyWifiAp
    {
        [JsonPropertyName("enabled")]
        public Boolean Enabled { get; set; }
        [JsonPropertyName("ssid")]
        public string SSID { get; set; }
        [JsonPropertyName("key")]
        public string Key { get; set; }
    }
    public class ShellyWifiStatus
    {
        [JsonPropertyName("enabled")]
        public Boolean Enabled { get; set; }
        [JsonPropertyName("ssid")]
        public string SSID { get; set; }
        [JsonPropertyName("ipv4_method")]
        public string Ipv4Method { get; set; }
        [JsonPropertyName("ip")]
        public string IP { get; set; }
        [JsonPropertyName("gw")]
        public string Gateway { get; set; }
        [JsonPropertyName("mask")]
        public string Mask { get; set; }
        [JsonPropertyName("dns")]
        public string DNS { get; set; }

    }
    public class ShellyDevice
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("mac")]
        public string MacAdress { get; set; }
        [JsonPropertyName("hostname")]
        public string HostName { get; set; }
        [JsonPropertyName("num_outputs")]
        public Double NumberOfOutputs { get; set; }

    }

    public class ShellyRelay
    {
        [JsonPropertyName("ison")]
        public Boolean IsOn { get; set; }
        [JsonPropertyName("has_timer")]
        public Boolean HasTimer { get; set; }
        [JsonPropertyName("timer_started")]
        public string TimerStarted { get; set; }

        [JsonPropertyName("timer_duration")]
        public string TimerDuration { get; set; }
        [JsonPropertyName("timer_remaining")]
        public string TimerRemaining { get; set; }
        [JsonPropertyName("source")]
        public string Source { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("appliance_type")]
        public string ApplianceType { get; set; }
        [JsonPropertyName("default_state")]
        public string DefaultState { get; set; }
        [JsonPropertyName("btn_type")]
        public string ButtonType { get; set; }
        [JsonPropertyName("btn_reverse")]
        public Double ButtonReverse { get; set; }
        [JsonPropertyName("auto_on")]
        public Double AutoOn { get; set; }
        [JsonPropertyName("auto_off")]
        public Double AutoOff { get; set; }
        [JsonPropertyName("power")]
        public Double Power { get; set; }
        [JsonPropertyName("schedule")]
        public Boolean Schedule { get; set; }
        [JsonPropertyName("schedule_rules")]
        public string[] ScheduleRules { get; set; }
    }
    #endregion Shelly Data Classes
}