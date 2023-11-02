
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SmartHome.DataClasses
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
        public List<ShellyRelay> Relays = new();
        [JsonPropertyName("device")]
        public ShellyDevice Device = new();
        [JsonPropertyName("wifi_ap")]
        public ShellyWifiAp WifiAccessPoint = new();
        [JsonPropertyName("wifi_sta")]
        public ShellyWifiStatus WifiState = new();
        [JsonPropertyName("wifi_sta1")]
        public ShellyWifiStatus WifiState1 = new();
        [JsonPropertyName("login")]
        public ShellyLogin Login = new();
        [JsonPropertyName("cloud")]
        public ShellyCloud Cloud = new();
        [JsonPropertyName("hwinfo")]
        public ShellyHardwareInfo HardwareInfo = new();
        [JsonPropertyName("build_info")]
        public ShellyBuildInfo BuildInfo = new();
        [JsonPropertyName("sntp")]
        public ShellySntp SNTP = new();

        [JsonPropertyName("pin_code")]
        public string PinCode { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("fw")]
        public string Firmware { get; set; }
        [JsonPropertyName("timezone")]
        public string TimeZone { get; set; }
        [JsonPropertyName("lat")]
        public double Lat { get; set; }
        [JsonPropertyName("lng")]
        public double Lng { get; set; }
        [JsonPropertyName("tzautodetect")]
        public bool TimeZoneAutoDetect { get; set; }
        [JsonPropertyName("tz_utc_offset")]
        public double TimeZoneUTCOffset { get; set; }
        [JsonPropertyName("time")]
        public string Time { get; set; }
        [JsonPropertyName("unixtime")]
        public double UnixTime { get; set; }
        [JsonPropertyName("ext_switch_enable")]
        public bool Ext_switch_enable { get; set; }
        [JsonPropertyName("ext_switch_reverse")]
        public bool Ext_switch_reverse { get; set; }
        [JsonPropertyName("mode")]
        public string Mode { get; set; }
        [JsonPropertyName("wifirecovery_reboot_enabled")]
        public bool WifirecoveryRebootEnabled { get; set; }
        [JsonPropertyName("longpush_time")]
        public double LongpushTime { get; set; }
        [JsonPropertyName("tz_dst")]
        public bool TimeZoneDestination { get; set; }
        [JsonPropertyName("tz_dst_auto")]
        public bool TimeZoneDestinationAuto { get; set; }
        [JsonPropertyName("factory_reset_from_switch")]
        public bool FactoryResetFromSwitch { get; set; }
        [JsonPropertyName("discoverable")]
        public bool Discoverable { get; set; }

        //Own
        public string Description {  get; set; }

    }
    #region Shelly Data Classes
    public class ShellyHardwareInfo
    {
        [JsonPropertyName("hw_revision")]
        public string HardwareVersion { get; set; }
        [JsonPropertyName("batch_id")]
        public double BatchId { get; set; }
    }
    public class ShellyBuildInfo
    {
        [JsonPropertyName("build_id")]
        public string ID { get; set; }
        [JsonPropertyName("build_timestamp")]
        public DateTime Timestamp { get; set; }
        [JsonPropertyName("build_version")]
        public string Version { get; set; }
    }
    public class ShellyLogin
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
        [JsonPropertyName("unprotected")]
        public bool Unprotected { get; set; }
        [JsonPropertyName("username")]
        public string UserName { get; set; }
    }
    public class ShellySntp
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
        [JsonPropertyName("server")]
        public string Server { get; set; }
    }
    public class ShellyCloud
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
        [JsonPropertyName("connected")]
        public bool Connected { get; set; }
    }
    public class ShellyWifiAp
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
        [JsonPropertyName("ssid")]
        public string SSID { get; set; }
        [JsonPropertyName("key")]
        public string Key { get; set; }
    }
    public class ShellyWifiStatus
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }
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
        public double NumberOfOutputs { get; set; }

    }

    public class ShellyRelay
    {
        [JsonPropertyName("ison")]
        public bool IsOn { get; set; }
        [JsonPropertyName("has_timer")]
        public bool HasTimer { get; set; }
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
        public double ButtonReverse { get; set; }
        [JsonPropertyName("auto_on")]
        public double AutoOn { get; set; }
        [JsonPropertyName("auto_off")]
        public double AutoOff { get; set; }
        [JsonPropertyName("power")]
        public double Power { get; set; }
        [JsonPropertyName("schedule")]
        public bool Schedule { get; set; }
        [JsonPropertyName("schedule_rules")]
        public string[] ScheduleRules { get; set; }
    }
    #endregion Shelly Data Classes
}