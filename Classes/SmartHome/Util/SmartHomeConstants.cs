using HomeLogging;
using InnerCore.Api.DeConz.ColorConverters;
using Microsoft.AspNetCore.Hosting;
using SmartHome.Classes.Shelly.Data;
using SmartHome.Classes.SmartHome.Data;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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
        #region WebServerCalls
        private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromMinutes(5) };
        /// <summary>
        /// Verbindet sich mit den anderen Webs um befehle abzugeben.
        /// </summary>
        /// <param name="nr"></param>
        /// <param name="call"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task<string> ConnectToWeb(RequestEnums nr, string call, string value = "")
        {
            try
            {
                _httpClient.DefaultRequestHeaders.ExpectContinue = false;

                if (string.IsNullOrEmpty(value)) value = "";
                Uri urlstate = new(call);
                HttpResponseMessage result;
                string returnValue;
                if (nr == RequestEnums.GET)
                {

                    result = await _httpClient.GetAsync(urlstate);
                    returnValue = await result.Content.ReadAsStringAsync();
                }
                else
                {
                    using var content = new StringContent(value, System.Text.Encoding.UTF8, "application/json");
                    if (nr == RequestEnums.POST)
                    {
                        result = await _httpClient.PostAsync(urlstate, content);
                    }
                    else if (nr == RequestEnums.PUT)
                    {
                        result = await _httpClient.PutAsync(urlstate, content);
                    }
                    else
                    {
                        throw new Exception("Kein unterstützter RequestEnum gesendet.");
                    }
                    if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        returnValue = ConnectToWebRetval.ok.ToString();
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
                log.ServerErrorsAdd("ConnectToWeb:TaskCanceledException:Url:" + call, ex);
                return string.Empty;
            }
            catch (Exception ex)
            {
                log.ServerErrorsAdd("ConnectToWeb:Url:" + call, ex);
                return string.Empty;
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
                var color = string.Format("#{0:X6}", random.Next(0x1000000)); // = "#A197B9"
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
        public int WebSocketPort
        {
            get
            {
                return 999;
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
    public class DenonConstants
    {
        public string BaseURL { get; private set; } = "http://denon.tami";
    }
    public class MarantzConstants
    {
        public string BaseURL { get; private set; } = "http://marantz.tami";
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
        public static string BaseURL { get; private set; } = "http://sonos.tami/SmartHome";
        /// <summary>
        /// Alle Player aus
        /// </summary>
        public string StoppAllPlayers { get; private set; } = BaseURL + "/StoppAllPlayers";
        /// <summary>
        /// Ergeschoss an
        /// </summary>
        public string GroundFloorOn { get; private set; } = BaseURL + "/GroundFloorOn";
        public string GroundFloorOff { get; private set; } = BaseURL + "/GroundFloorOff";
        /// <summary>
        /// Wohnzimmer Spezial
        /// </summary>
        public string LivingRoomSpezial { get; private set; } = BaseURL + "/LivingRoomSpezial";
        /// <summary>
        /// Gästezimmer mit übergebener Playlist starten
        /// </summary>
        public string GuestRoom { get; private set; } = BaseURL + "/GuestRoom";
        /// <summary>
        /// Generic Room, wird über den Namen als Parameter gesteuert.
        /// </summary>
        public string GenericRoomOn { get; private set; } = BaseURL + "/GenericRoomOn";
        public string GenericRoomOff { get; private set; } = BaseURL + "/GenericRoomOff";
        /// <summary>
        /// Gästezimmer Lautstärke nach oben oder unten korrigieren.
        /// </summary>
        public string GuestRoomVolume { get; private set; } = BaseURL + "/RoomVolumeRelativ/RINCON_000E5850B61601400/";
        public string LivingRoomVolume { get; private set; } = BaseURL + "/RoomVolumeRelativ/RINCON_000E5823E01C01400/";
        /// <summary>
        /// Gästezimmer aus.
        /// </summary>
        public string GuestRoomOff { get; private set; } = BaseURL + "/GuestRoomOff";
        public string IanRoomRandom3 { get; private set; } = "http://sonos.tami/Ian/PlayRandom/zzzIanButton3";
        public string IanRoomRandom2 { get; private set; } = "http://sonos.tami/Ian/PlayRandom/zzzIanButton2";
        public string FinnRoomRandom1 { get; private set; } = "http://sonos.tami/Finn/PlayRandom/zzzFinnButton1";
        public string FinnRoomReplace { get; private set; } = "http://sonos.tami/Finn/ReplacePlaylist";
        /// <summary>
        /// Gästezimmer aus.
        /// </summary>
        public string IanRoomOff { get; private set; } = BaseURL + "/IanRoomOff";
        public string GuestRoomAudioInOn { get; private set; } = BaseURL + "/GuestRoomAudioInOn";

        public string GuestRoomAudioInOff { get; private set; } = BaseURL + "/GuestRoomAudioInOff";
        //SonosGuestRoomAudioInOn
    }
}