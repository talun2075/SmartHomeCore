using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using HomeLogging;
using InnerCore.Api.DeConz.Models;

namespace SmartHome.Classes
{
    public class SmartHomeWrapper
    {
        #region ClassVariables
        #endregion ClassVariables
        #region ClickEvents
        public async static Task<Boolean> Touch(string mac, ButtonRequest br = null)
        {
            if (!await CheckButton(mac))
            {
                SmartHomeConstants.log.ServerErrorsAdd("Touch", new Exception("Unbekannte MAC:" + mac), "SmartHomeWrapper");
                throw SmartHomeHelper.ReturnWebError("Unbekannte MAC:" + mac);
            }
            switch (mac)
            {
                case "5CCF7FF0D13F":
                        return await GuestRoom("zzz Regen Neu");
                case "2C3AE801C092":
                    return await GroundFloorOn();
                case "5CCF7FF0D1CA":
                    DeConzResults retv = await SmartHomeHelper.DeconzEating(DeconzSwitch.On);
                    if (retv[0].Error == null) return true;
                    throw SmartHomeHelper.ReturnWebError("Fehler aufgetreten beim Licht einschalten");
                case "2C3AE8018316":
                    try
                    {
                        await GroundFloorOff();
                        await SmartHomeHelper.DeconzGroundFloorOff();
                        await SmartHomeHelper.PowerOffAurora("Wohnzimmer");
                        await SmartHomeHelper.PowerOffAurora("Esszimmer");
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                default:
                    SmartHomeConstants.log.TraceLog("SmartHomeWrapper:Touch:Default", "Für diesen Case ist nichts definiert. MAC:" + mac);
                    throw SmartHomeHelper.ReturnWebError("Für diesen Case ist nichts definiert. MAC:" + mac);

            }
        }
        public async static Task<Boolean> Single(string mac)
        {
            if (!await CheckButton(mac))
            {
                SmartHomeConstants.log.ServerErrorsAdd("Single", new Exception("Unbekannte MAC:" + mac), "SmartHomeWrapper");
                throw SmartHomeHelper.ReturnWebError("Unbekannte MAC:" + mac);
            }
            switch (mac)
            {
                case "5CCF7FF0D13F":
                    return await GuestRoom("zzz tempsleep", 6);
                case "2C3AE8018316":
                    return await FirstOff();
                case "BCFF4D4B1034":
                    return await SmartHomeHelper.SonosIanRoomRandom2();
                case "68C63AD16455":
                    return await SmartHomeHelper.SonosIanRoomRandom3();
                case "60019427DE18":
                    return await PowerShellysGuestRoom(true);
                case "2C3AE801C092":
                    return await GroundFloorOff();
                case "483FDA6EBD7D":
                    return await GroundFloorOn("Foo Fighters");
                case "60019427EFA7":
                    DeConzResults retv = await SmartHomeHelper.DeconzEating(DeconzSwitch.On);
                    if (retv[0].Error == null) return true;
                    throw SmartHomeHelper.ReturnWebError("Fehler aufgetreten beim Licht einschalten");
                case "5CCF7FF0D1CA":
                    return await AllOff();
                default:
                    SmartHomeConstants.log.TraceLog("SmartHomeWrapper:Single:Default", "Für diesen Case ist nichts definiert. MAC:" + mac);
                    throw SmartHomeHelper.ReturnWebError("Für diesen Case ist nichts definiert. MAC:" + mac);
            }
        }
        public async static Task<Boolean> Double(string mac)
        {
            if (!await CheckButton(mac))
            {
                SmartHomeConstants.log.ServerErrorsAdd("Double", new Exception("Unbekannte MAC:" + mac), "SmartHomeWrapper");
                throw SmartHomeHelper.ReturnWebError("Unbekannte MAC:" + mac);
            }
            switch (mac)
            {
                case "60019427DE18":
                    return await WohnzimmerSpezial();
                case "2C3AE801C092":
                case "5CCF7FF0D1CA":
                    return await GroundFloorOff();
                case "483FDA6EBD7D":
                    return await GroundFloorOn("Gelegenheit Party");
                case "5CCF7F0CC51A":
                    return await SmartHomeHelper.SonosIanRoomOff();
                default:
                    SmartHomeConstants.log.TraceLog("SmartHomeWrapper:Double:Default", "Für diesen Case ist nichts definiert. MAC:" + mac);
                    throw SmartHomeHelper.ReturnWebError("Für diesen Case ist nichts definiert. MAC:" + mac);

            }
        }
        public async static Task<Boolean> Long(string mac)
        {
            if (!await CheckButton(mac))
            {
                SmartHomeConstants.log.ServerErrorsAdd("Long", new Exception("Unbekannte MAC:" + mac), "SmartHomeWrapper");
                throw SmartHomeHelper.ReturnWebError("Unbekannte MAC:" + mac);
            }
            switch (mac)
            {
                case "5CCF7F0CC51A":
                    return await SmartHomeHelper.SonosIanRoomOff();
                case "60019427DE18":
                    return await PowerShellysGuestRoom(false);
                case "2C3AE801C092":
                    return await GroundFloorOn("Harte Gruppen");
                case "5CCF7FF0D13F":
                    return await GuestRoomOff();
                case "68C63AD16455":
                    return await SmartHomeHelper.SonosIanRoomRandom3();
                case "483FDA6EBD7D":
                    return await GroundFloorOn("Harte Gruppen Genre");
                case "5CCF7FF0D1CA":
                    return await GroundFloorOn();
                case "2C3AE8018316":
                    return await AllOff();
                case "60019427EFA7":
                    DeConzResults retv = await SmartHomeHelper.DeconzEating(DeconzSwitch.Off);
                    if (retv[0].Error == null) return true;
                    throw SmartHomeHelper.ReturnWebError("Fehler aufgetreten beim Lichtausschalten");
                default:
                    SmartHomeConstants.log.TraceLog("SmartHomeWrapper:Long:Default", "Für diesen Case ist nichts definiert. MAC:" + mac);
                    throw SmartHomeHelper.ReturnWebError("Für diesen Case ist nichts definiert. MAC:" + mac);
            }
        }

        public async static Task<Boolean> CupeLiving (int id)
        {
            return await SmartHomeHelper.CupeLiving(id);
        }

        public static Logging generic_log = new (new LoggerWrapperConfig() { ConfigName = "GenericLog", InfoFileName = "battery.txt" });
        public static async Task<Boolean> Generic(ButtonRequest br)
        {
            //var mac = HttpContext.Current.Request["mac"];
            //var battery = HttpContext.Current.Request["battery"];
            //var wheel = HttpContext.Current.Request["wheel"];
            //var action = HttpContext.Current.Request["action"];
            /*
             Name 	Type 	Range 	    Given 	Comments
               mac 	MAC 		        yes 	
            action 	ENUM 	SINGLE = 1  yes 	
                            DOUBLE=2 
                            LONG=3
                            TOUCH=4
                            WHEEL=5
                            WHEEL_FINAL=11
                            BATTERY=6 	
            wheel 	INT 	-127...127 	no 	    Only in action WHEEL case
            battery INT 	0..100 	yes 	    In percent
            

            https://api.mystrom.ch/?version=latest

             */
            try
            {
                if (!SmartHomeConstants.KnowingButtons.Any()) await ReadButtonXML();
                Button b = SmartHomeConstants.KnowingButtons.FirstOrDefault(x => x.Mac == br.Mac);
                if (b == null) return false;
                b.Batterie = Convert.ToInt32(br.Battery);
                if (Enum.TryParse(br.Action, out ButtonAction ba))
                {
                    b.LastAction = ba;
                }
                b.LastClick = DateTime.Now;
                try
                {
                    if (b.LastAction != ButtonAction.BATTERY)
                    {
                        await DatabaseWrapper.UpdateButton(b.Mac, b.Name, b.Batterie, b.LastAction.ToString());
                    }
                    else
                    {
                        await DatabaseWrapper.UpdateBattery(b.Mac, b.Batterie);
                    }
                }
                catch (Exception ex)
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeWrapper:Generic:SQLitecall", ex, br.Mac);
                    throw SmartHomeHelper.ReturnWebError("SmartHomeWrapper:Generic:SQLitecall:" + ex.Message);
                }
                finally
                {
                    DatabaseWrapper.Close();
                }
                //if (b.Batterie < 25 && b.Aktiv == true)
                //    generic_log.InfoLog(b.Name + " (" + b.Mac + ") ", battery);
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeWrapper:Generic", ex, br.Mac);
                throw SmartHomeHelper.ReturnWebError("SmartHomeWrapper:Generic:SQLitecall:" + ex.Message);
            }
        }
        public async static Task<String> Test()
        {
            return await SmartHomeHelper.Test();
        }
        #endregion ClickEvents
        #region private Methoden
        private static async Task<Boolean> CheckButton(string tocheck)
        {
            if (!SmartHomeConstants.KnowingButtons.Any())
            {
                await ReadButtonXML();
            }
            Button b = SmartHomeConstants.KnowingButtons.FirstOrDefault(x => x.Mac.ToLower() == tocheck.ToLower());
            if (b != null && b.Aktiv) return true;
            return false;
        }
        public static async Task<Boolean> ReadButtonXML()
        {
            try
            {
                //SmartHomeConstants.log.TraceLog("ReadButtonXML", "Start");
                string path = SmartHomeConstants.Env.ContentRootPath + "\\Configuration\\Buttons.xml";
                XmlDocument myXmlDocument = new ();
                myXmlDocument.Load(path);
                //myXmlDocument.Load(mUrl + mXMLPath); //Load NOT LoadXml
                XmlNodeList buttonsconfig = myXmlDocument.SelectNodes("/Buttons/Button");
                foreach (XmlNode item in buttonsconfig)
                {
                    Button st = new ()
                    {
                        Name = item.Attributes["Name"].Value,
                        Mac = item.Attributes["Mac"].Value,
                        Aktiv = item.Attributes["Aktiv"]?.Value == null || item.Attributes["Aktiv"]?.Value == "true",
                        Visible = item.Attributes["Visible"]?.Value == null || item.Attributes["Visible"]?.Value == "true",
                        IP = item.Attributes["IP"]?.Value,
                    };
                    //if (!st.Aktiv) continue;
                    if (!SmartHomeConstants.KnowingButtons.Any())
                    {
                        //SmartHomeConstants.log.TraceLog("ReadButtonXML", "Timer add:" + st.Name);
                        SmartHomeConstants.KnowingButtons.Add(st);
                    }
                    else
                    {
                        Button curbutton = SmartHomeConstants.KnowingButtons.FirstOrDefault(x => x.Name == st.Name);
                        if (curbutton == null)
                        {
                            //SmartHomeConstants.log.TraceLog("ReadButtonXML", "Timer add:" + st.Name);
                            SmartHomeConstants.KnowingButtons.Add(st);
                        }
                        else
                        {
                            curbutton.Name = st.Name;
                            curbutton.Mac = st.Mac;
                            curbutton.IP = st.IP;
                            curbutton.Aktiv = st.Aktiv;
                        }
                    }
                }
                await DatabaseWrapper.ReadButtons();
                //SmartHomeConstants.log.TraceLog("ReadButtonXML", "Ende");
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeWrapper:ReadButtonXML:", ex);
                throw SmartHomeHelper.ReturnWebError("SmartHomeWrapper:ReadButtonXML:" + ex.Message);
            }
        }
        
        public static async Task<Boolean> PowerShellysGuestRoom(Boolean powerOn = false)
        {
            return await SmartHomeHelper.PowerShellysGuestRoom(powerOn);
        }
        
        /// <summary>
        /// Schaltet alles aus im Haus aus.
        /// </summary>
        /// <returns></returns>
        private static async Task<Boolean> FirstOff()
        {
            try
            {
                await SmartHomeHelper.DeconzFirstFloorOff();
                return true;
            }
            catch (Exception ex)
            {
                throw SmartHomeHelper.ReturnWebError("FirstOff" + ex.Message);
            }
        }
        /// <summary>
        /// Schaltet Ergeschosse und erster Stock aus im Haus aus.
        /// </summary>
        /// <returns></returns>
        private static async Task<Boolean> AllOff()
        {
            try
            {
                await SmartHomeHelper.SonosStoppAllPlayer();
                _ = SmartHomeHelper.PowerOffAuroras();
                _ = SmartHomeHelper.PowerOffMarantz(true);
                await SmartHomeHelper.DeconzAllLightsOff();
                return true;
            }
            catch (Exception ex)
            {
                throw SmartHomeHelper.ReturnWebError("AllOff" + ex.Message);
            }
        }
        /// <summary>
        /// Wohnzimmer alleine machen und mit einer random Playlist versehen. 
        /// Auf Sonos schalten und alle Lampen ausschalten. 
        /// </summary>
        /// <returns></returns>
        private static async Task<Boolean> WohnzimmerSpezial()
        {
            Boolean retval = true;
            var showedex = new Exception();
            try
            {
                await SmartHomeHelper.DeconzGroundFloorOff();
                await SmartHomeHelper.PowerOffAurora("Esszimmer");
                await SmartHomeHelper.PowerOnAurora("Wohnzimmer");
                await SmartHomeHelper.PowerOnMarantz();
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("WohnzimmerSpezial:MultimediaKram", ex, "SmartHomeWrapper");
                retval = false;
                showedex = ex;
            }

            try
            {
                await SmartHomeHelper.SonosLivingRoomSpezial();

            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("WohnzimmerSpezial:Global", ex, "SmartHomeWrapper");
                retval = false;
                showedex = ex;
            }
            if (!retval)
            {
                throw SmartHomeHelper.ReturnWebError("AllOff" + showedex.Message);
            }
            return retval;
        }

        /// <summary>
        /// Erdgeschoss ausschalten
        /// </summary>
        /// <returns></returns>
        private static async Task<Boolean> GroundFloorOff()
        {
            try
            {
                await SmartHomeHelper.SonosGroundFloorOff();
            }
            catch (Exception ex)
            {
                throw SmartHomeHelper.ReturnWebError("GroundFloorOff" + ex.Message);
            }
            try
            {
                await SmartHomeHelper.PowerOffMarantz();
                await SmartHomeHelper.PowerOffAuroras();
            }
            catch (Exception ex)
            {
                throw SmartHomeHelper.ReturnWebError("GroundFloorOff:Part2" + ex.Message);
            }
            return true;
        }
        /// <summary>
        /// Schaltet alle Geräte im Erdgeschosse an.
        /// </summary>
        /// <param name="playlistToPlay">Playlist die abgespielt werden soll</param>
        /// <returns></returns>
        private static async Task<Boolean> GroundFloorOn(string playlistToPlay = "3 Sterne Beide")
        {
            try
            {
                try
                {
                    _ = SmartHomeHelper.PowerOnMarantz();
                }
                catch (Exception ex)
                {
                    SmartHomeConstants.log.ServerErrorsAdd("GroundFloorOn:marantz", ex, "SmartHomeWrapper");
                }
                try
                {
                    await SmartHomeHelper.SonosGroundFloorOn(playlistToPlay);
                }
                catch (Exception ex)
                {
                    SmartHomeConstants.log.ServerErrorsAdd("GroundFloorOn:sonos", ex, "SmartHomeWrapper");
                }
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("GroundFloorOn:global", ex, "SmartHomeWrapper");
                throw SmartHomeHelper.ReturnWebError("GroundFloorOn" + ex.Message);
            }
        }
        private static async Task<Boolean> GuestRoom(string playlistToPlay, int volume = 0)
        {
            return await SmartHomeHelper.SonosGuestRoom(playlistToPlay, volume);
        }
        private static async Task<Boolean> GuestRoomOff()
        {
            return await SmartHomeHelper.SonosGuestRoomOff();
        }

        #endregion private Methoden
    }
}