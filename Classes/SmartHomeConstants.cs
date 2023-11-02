using HomeLogging;
using InnerCore.Api.DeConz.ColorConverters;
using Microsoft.AspNetCore.Hosting;
using SmartHome.DataClasses;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHome.Classes
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
        public static List<Button> KnowingButtons = new();
        /// <summary>
        /// Liste aller Shellys1
        /// </summary>
        public static List<Shelly1> Shelly1 = new();
        public static DateTime ShellyLastChange = DateTime.Now;
        //public static AuroraConstants Aurora { get; private set; } = new();
        public static MarantzConstants Marantz { get; private set; } = new();
        public static DenonConstants Denon { get; private set; } = new();
        public static SonosConstants Sonos { get; private set; } = new();
        public static DeconZConstants Deconz { get; private set; } = new();
        public static IWebHostEnvironment Env { get; set; }
        /// <summary>
        /// Request Typen, die Supportet werden
        /// </summary>
        public enum RequestEnums
        {
            GET,
            PUT,
            POST,
        }
        public enum ConnectToWebRetval
        {
            ok
        }
        /// <summary>
        /// TimerTypen
        /// </summary>
        public enum TimerType
        {
            /// <summary>
            /// Use the Class/Methods for internal calling
            /// This is the Default;
            /// </summary>
            INTERNAL,
            /// <summary>
            /// Try to call a Web URI
            /// </summary>
            URL
        }

        #region WebServerCalls
        private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromMinutes(5) };
        /// <summary>
        /// Verbindet sich mit den anderen Webs um befehle abzugeben.
        /// </summary>
        /// <param name="nr"></param>
        /// <param name="call"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task<String> ConnectToWeb(SmartHomeConstants.RequestEnums nr, string call, string value = "")
        {
            try
            {
                _httpClient.DefaultRequestHeaders.ExpectContinue = false;

                if (string.IsNullOrEmpty(value)) value = "";
                Uri urlstate = new(call);
                HttpResponseMessage result;
                string returnValue;
                if (nr == SmartHomeConstants.RequestEnums.GET)
                {

                    result = await _httpClient.GetAsync(urlstate);
                    returnValue = await result.Content.ReadAsStringAsync();
                }
                else
                {
                    using var content = new StringContent(value, System.Text.Encoding.UTF8, "application/json");
                    if (nr == SmartHomeConstants.RequestEnums.POST)
                    {
                        result = await _httpClient.PostAsync(urlstate, content);
                    }
                    else if (nr == SmartHomeConstants.RequestEnums.PUT)
                    {
                        result = await _httpClient.PutAsync(urlstate, content);
                    }
                    else
                    {
                        throw new Exception("Kein unterstützter RequestEnum gesendet.");
                    }
                    if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        returnValue = SmartHomeConstants.ConnectToWebRetval.ok.ToString();
                    }
                    else
                    {
                        returnValue = result.Content.ReadAsStringAsync().Result;
                    }
                }
                return returnValue;
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                // Its a timeout issue
                //SmartHomeConstants.log.ServerErrorsAdd("ConnectToWeb:TimeoutException:Url:" + call, ex);
                return "ok";
            }
            catch (TaskCanceledException ex)
            {
                // Its a some other issue
                SmartHomeConstants.log.ServerErrorsAdd("ConnectToWeb:TaskCanceledException:Url:" + call, ex);
                return String.Empty;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("ConnectToWeb:Url:" + call, ex);
                return String.Empty;
            }
        }
        #endregion WebServerCalls
    }
    public class DeconZConstants
    {
        public RGBColor RandomRGBColor
        {
            get
            {
                Random random = new();
                var color = String.Format("#{0:X6}", random.Next(0x1000000)); // = "#A197B9"
                return new RGBColor(color);
                //Random rnd = new Random();
                //return new RGBColor(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
            }
        }
        public byte RandomBrightness
        {
            get
            {
                Random rnd = new();
                return (byte)rnd.Next(50, 255);
            }
        }
        public byte RandomSaturation
        {
            get
            {
                Random rnd = new();
                return (byte)rnd.Next(0, 255);
            }
        }

        public int RandomHue
        {
            get
            {
                Random rnd = new();
                return rnd.Next(2000, 6500);
            }
        }
        public String BaseUrl
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
        public int WebSocketPort
        {
            get
            {
                return 999;
            }
        }
        public String ApiKey
        {
            get
            {
                return "463608DCF3";
            }
        }
    }
    public class DenonConstants
    {
        public String BaseURL { get; private set; } = "http://denon.tami";
    }
    public class MarantzConstants
    {
        public String BaseURL { get; private set; } = "http://marantz.tami";
    }

    ///// <summary>
    ///// Alle Constanten im Kontext Aurora.
    ///// </summary>
    //public class AuroraConstants
    //{
    //    /// <summary>
    //    /// Base Pfad, IP, da DNS nicht geht.
    //    /// </summary>
    //    public static String BaseURL { get; private set; } = "http://aurora.tami/aurora";
    //    /// <summary>
    //    /// Grund Infos
    //    /// </summary>
    //    public String GroundInfos { get; private set; } = BaseURL + "/Get";
    //    /// <summary>
    //    /// Scenario Setzen
    //    /// </summary>
    //    public String SetSelectedScenario { get; private set; } = BaseURL + "/SetSelectedScenario";
    //    /// <summary>
    //    /// PowerState setzen mit Seriennummner
    //    /// </summary>
    //    public String SetPowerState { get; private set; } = BaseURL + "/SetPowerState";
    //    /// <summary>
    //    /// Powerstate einer einzelnen Aurora über Namen setzen
    //    /// </summary>
    //    public String SetPowerStateByName { get; private set; } = BaseURL + "/SetPowerStateByName";
    //    /// <summary>
    //    /// alle Aurora an bzw. aus schalten.
    //    /// </summary>
    //    public String SetGroupPowerState { get; private set; } = BaseURL + "/SetGroupPowerState";
    //    public String SetGroupPowerStateAll { get; private set; } = BaseURL + "/SetGroupPowerStateAll";
    //    /// <summary>
    //    /// Zufälliges Scenario setzen.
    //    /// </summary>
    //    public String SetRandomScenario { get; private set; } = BaseURL + "/SetRandomScenario";
    //}

    public class SonosConstants
    {
        /// <summary>
        /// Base Pfad, IP, da DNS nicht geht.
        /// </summary>
        public static String BaseURL { get; private set; } = "http://sonos.tami/SmartHome";
        /// <summary>
        /// Alle Player aus
        /// </summary>
        public String StoppAllPlayers { get; private set; } = BaseURL + "/StoppAllPlayers";
        /// <summary>
        /// Ergeschoss an
        /// </summary>
        public String GroundFloorOn { get; private set; } = BaseURL + "/GroundFloorOn";
        public String GroundFloorOff { get; private set; } = BaseURL + "/GroundFloorOff";
        /// <summary>
        /// Wohnzimmer Spezial
        /// </summary>
        public String LivingRoomSpezial { get; private set; } = BaseURL + "/LivingRoomSpezial";
        /// <summary>
        /// Gästezimmer mit übergebener Playlist starten
        /// </summary>
        public String GuestRoom { get; private set; } = BaseURL + "/GuestRoom";
        /// <summary>
        /// Gästezimmer Lautstärke nach oben oder unten korrigieren.
        /// </summary>
        public String GuestRoomVolume { get; private set; } = BaseURL + "/RoomVolumeRelativ/RINCON_000E5850B61601400/";
        public String LivingRoomVolume { get; private set; } = BaseURL + "/RoomVolumeRelativ/RINCON_000E5823E01C01400/";
        /// <summary>
        /// Gästezimmer aus.
        /// </summary>
        public String GuestRoomOff { get; private set; } = BaseURL + "/GuestRoomOff";
        public String IanRoomRandom3 { get; private set; } = "http://sonos.tami/Ian/PlayRandom/zzzIanButton3";
        public String IanRoomRandom2 { get; private set; } = "http://sonos.tami/Ian/PlayRandom/zzzIanButton2";
        /// <summary>
        /// Gästezimmer aus.
        /// </summary>
        public String IanRoomOff { get; private set; } = BaseURL + "/IanRoomOff";
        public String GuestRoomAudioInOn { get; private set; } = BaseURL + "/GuestRoomAudioInOn";

        public String GuestRoomAudioInOff { get; private set; } = BaseURL + "/GuestRoomAudioInOff";
        //SonosGuestRoomAudioInOn
    }
}