using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using HomeLogging;
using InnerCore.Api.DeConz.Models;
using SmartHome.Classes.Database;
using SmartHome.Classes.Deconz;
using SmartHome.Classes.SmartHome.Data;
using SmartHome.Classes.SmartHome.Interfaces;
using SmartHome.Classes.SmartHome.Util;

namespace SmartHome.Classes.SmartHome
{
    public class SmartHomeWrapper : ISmartHomeWrapper
    {
        IDatabaseWrapper db;
        ISmartHomeHelper helper;
        public SmartHomeWrapper(IDatabaseWrapper _db, ISmartHomeHelper _helper)
        {
            db = _db;
            helper = _helper;
        }
        #region ClassVariables
        #endregion ClassVariables
        #region ClickEvents
        public async Task<bool> Touch(string mac, ButtonRequest br = null)
        {
            if (!await CheckButton(mac))
            {
                SmartHomeConstants.log.ServerErrorsAdd("Touch", new Exception("Unbekannte MAC:" + mac + " IP:" + br.IP), "SmartHomeWrapper");
                throw helper.ReturnWebError("Unbekannte MAC:" + mac);
            }
            switch (mac)
            {
                case "5CCF7FF0D13F":
                    return await GuestRoom("zzz Regen Neu");
                case "2C3AE801C092":
                    return await GroundFloorOn();
                case "5CCF7FF0D1CA":
                    DeConzResults retv = await helper.DeconzEating(DeconzSwitch.On);
                    if (retv[0].Error == null) return true;
                    throw helper.ReturnWebError("Fehler aufgetreten beim Licht einschalten");
                case "2C3AE8018316":
                    try
                    {
                        await GroundFloorOff();
                        await helper.DeconzGroundFloorOff();
                        await helper.PowerOffAurora("Wohnzimmer");
                        await helper.PowerOffAurora("Esszimmer");
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                default:
                    SmartHomeConstants.log.TraceLog("SmartHomeWrapper:Touch:Default", "Für diesen Case ist nichts definiert. MAC:" + mac);
                    throw helper.ReturnWebError("Für diesen Case ist nichts definiert. MAC:" + mac);

            }
        }
        public async Task<bool> Single(string mac)
        {
            if (!await CheckButton(mac))
            {
                SmartHomeConstants.log.ServerErrorsAdd("Single", new Exception("Unbekannte MAC:" + mac), "SmartHomeWrapper");
                throw helper.ReturnWebError("Unbekannte MAC:" + mac);
            }
            switch (mac)
            {
                case "5CCF7FF0D13F":
                    return await GuestRoom("zzz tempsleep", 6);
                case "2C3AE8018316":
                    return await FirstOff();
                case "BCFF4D4B1034":
                    return await helper.SonosIanRoomRandom2();
                case "68C63AD16455":
                    return await helper.SonosIanRoomRandom3();
                case "60019427DE18":
                    return await PowerShellysGuestRoom(true);
                case "2C3AE801C092":
                    return await GroundFloorOff();
                case "483FDA6EBD7D":
                    return await GroundFloorOn("Foo Fighters");
                case "60019427EFA7":
                    return await GuestRoom("zzz Regen Neu");
                case "5CCF7FF0D1CA":
                    return await AllOff();
                case "68C63AD1624E":
                    var result = await helper.DeconzLightsPower("28");
                    if (!result.HasErrors())
                    {
                        return true;
                    }
                    return false;
                default:
                    SmartHomeConstants.log.TraceLog("SmartHomeWrapper:Single:Default", "Für diesen Case ist nichts definiert. MAC:" + mac);
                    throw helper.ReturnWebError("Für diesen Case ist nichts definiert. MAC:" + mac);
            }
        }
        public async Task<bool> Double(string mac)
        {
            if (!await CheckButton(mac))
            {
                SmartHomeConstants.log.ServerErrorsAdd("Double", new Exception("Unbekannte MAC:" + mac), "SmartHomeWrapper");
                throw helper.ReturnWebError("Unbekannte MAC:" + mac);
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
                    return await helper.SonosIanRoomOff();
                case "60019427EFA7":
                    return await GuestRoomOff() && await helper.PowerShellysGuestRoomRight(false);
                case "68C63AD1624E":
                    var result = await helper.DeconzLightsPower(new List<string>() { "8", "28" });
                    if (!result.HasErrors())
                    {
                        return true;
                    }
                    return false;
                default:
                    SmartHomeConstants.log.TraceLog("SmartHomeWrapper:Double:Default", "Für diesen Case ist nichts definiert. MAC:" + mac);
                    throw helper.ReturnWebError("Für diesen Case ist nichts definiert. MAC:" + mac);

            }
        }
        public async Task<bool> Long(string mac)
        {
            if (!await CheckButton(mac))
            {
                SmartHomeConstants.log.ServerErrorsAdd("Long", new Exception("Unbekannte MAC:" + mac), "SmartHomeWrapper");
                throw helper.ReturnWebError("Unbekannte MAC:" + mac);
            }
            switch (mac)
            {
                case "5CCF7F0CC51A":
                    return await helper.SonosIanRoomOff();
                case "60019427DE18":
                    return await PowerShellysGuestRoom(false);
                case "2C3AE801C092":
                    return await GroundFloorOn("Harte Gruppen");
                case "5CCF7FF0D13F":
                    return await GuestRoomOff();
                case "68C63AD16455":
                    return await helper.SonosIanRoomRandom3();
                case "483FDA6EBD7D":
                    return await GroundFloorOn("Harte Gruppen Genre");
                case "5CCF7FF0D1CA":
                    return await GroundFloorOn();
                case "2C3AE8018316":
                    return await AllOff();
                case "60019427EFA7":
                    return await GuestRoom("zzz tempsleep", 6);
                case "68C63AD1624E":
                    var result = await helper.DeconzLightsPower(new List<string>() { "8", "28" }, false);
                    if (!result.HasErrors())
                    {
                        return true;
                    }
                    return false;
                default:
                    SmartHomeConstants.log.TraceLog("SmartHomeWrapper:Long:Default", "Für diesen Case ist nichts definiert. MAC:" + mac);
                    throw helper.ReturnWebError("Für diesen Case ist nichts definiert. MAC:" + mac);
            }
        }

        public async Task<bool> CupeLiving(int id)
        {
            return await helper.CupeLiving(id);
        }

        public Logging generic_log = new(new LoggerWrapperConfig() { ConfigName = "GenericLog", InfoFileName = "battery.txt" });
        public async Task<bool> Generic(ButtonRequest br)
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

                if (int.TryParse(br.Battery, out int brbattery))
                {
                    b.Batterie = brbattery;
                }
                if (Enum.TryParse(br.Action, out ButtonAction ba))
                {
                    b.LastAction = ba;
                }
                b.IP = br.IP;
                b.LastClick = DateTime.Now;
                try
                {
                    await db.UpdateButton(b);
                }
                catch (Exception ex)
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeWrapper:Generic:SQLitecall", ex, br.Mac);
                    throw helper.ReturnWebError("SmartHomeWrapper:Generic:SQLitecall:" + ex.Message);
                }
                //if (b.Batterie < 25 && b.Aktiv == true)
                //    generic_log.InfoLog(b.Name + " (" + b.Mac + ") ", battery);
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeWrapper:Generic", ex, br.Mac);
                throw helper.ReturnWebError("SmartHomeWrapper:Generic:SQLitecall:" + ex.Message);
            }
        }
        #endregion ClickEvents
        #region private Methoden
        private async Task<bool> CheckButton(string tocheck)
        {
            if (!SmartHomeConstants.KnowingButtons.Any())
            {
                await ReadButtonXML();
            }
            Button b = SmartHomeConstants.KnowingButtons.FirstOrDefault(x => x.Mac.ToLower() == tocheck.ToLower());
            if (b != null && b.Aktiv) return true;
            return false;
        }
        public async Task<bool> ReadButtonXML()
        {
            try
            {
                //SmartHomeConstants.log.TraceLog("ReadButtonXML", "Start");
                string path = SmartHomeConstants.Env.ContentRootPath + "\\Configuration\\Buttons.xml";
                XmlDocument myXmlDocument = new();
                myXmlDocument.Load(path);
                //myXmlDocument.Load(mUrl + mXMLPath); //Load NOT LoadXml
                XmlNodeList buttonsconfig = myXmlDocument.SelectNodes("/Buttons/Button");
                foreach (XmlNode item in buttonsconfig)
                {
                    Button st = new()
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
                await db.ReadButtons();
                //SmartHomeConstants.log.TraceLog("ReadButtonXML", "Ende");
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeWrapper:ReadButtonXML:", ex);
                throw helper.ReturnWebError("SmartHomeWrapper:ReadButtonXML:" + ex.Message);
            }
        }

        public async Task<bool> PowerShellysGuestRoom(bool powerOn = false)
        {
            return await helper.PowerShellysGuestRoom(powerOn);
        }

        /// <summary>
        /// Schaltet alles aus im Haus aus.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> FirstOff()
        {
            try
            {
                await helper.DeconzFirstFloorOff();
                return true;
            }
            catch (Exception ex)
            {
                throw helper.ReturnWebError("FirstOff" + ex.Message);
            }
        }
        /// <summary>
        /// Schaltet Ergeschosse und erster Stock aus im Haus aus.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> AllOff()
        {
            try
            {
                await helper.SonosStoppAllPlayer();
                _ = helper.PowerOffAuroras();
                _ = helper.PowerOffDenon(true);
                await helper.DeconzAllLightsOff();
                return true;
            }
            catch (Exception ex)
            {
                throw helper.ReturnWebError("AllOff" + ex.Message);
            }
        }
        /// <summary>
        /// Wohnzimmer alleine machen und mit einer random Playlist versehen. 
        /// Auf Sonos schalten und alle Lampen ausschalten. 
        /// </summary>
        /// <returns></returns>
        private async Task<bool> WohnzimmerSpezial()
        {
            bool retval = true;
            var showedex = new Exception();
            try
            {
                await helper.DeconzGroundFloorOff();
                await helper.PowerOffAurora("Esszimmer");
                await helper.PowerOnAuroras("Wohnzimmer");
                await helper.PowerOnDenon();
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("WohnzimmerSpezial:MultimediaKram", ex, "SmartHomeWrapper");
                retval = false;
                showedex = ex;
            }

            try
            {
                await helper.SonosLivingRoomSpezial();

            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("WohnzimmerSpezial:Global", ex, "SmartHomeWrapper");
                retval = false;
                showedex = ex;
            }
            if (!retval)
            {
                throw helper.ReturnWebError("AllOff" + showedex.Message);
            }
            return retval;
        }

        /// <summary>
        /// Erdgeschoss ausschalten
        /// </summary>
        /// <returns></returns>
        private async Task<bool> GroundFloorOff()
        {
            try
            {
                await helper.SonosGroundFloorOff();
            }
            catch (Exception ex)
            {
                throw helper.ReturnWebError("GroundFloorOff" + ex.Message);
            }
            try
            {
                await helper.PowerOffDenon();
            }
            catch (Exception ex)
            {
                throw helper.ReturnWebError("GroundFloorOff:Part2" + ex.Message);
            }
            return true;
        }
        /// <summary>
        /// Schaltet alle Geräte im Erdgeschosse an.
        /// </summary>
        /// <param name="playlistToPlay">Playlist die abgespielt werden soll</param>
        /// <returns></returns>
        private async Task<bool> GroundFloorOn(string playlistToPlay = "3 Sterne Beide")
        {
            try
            {
                try
                {
                    _ = helper.PowerOnDenon();
                }
                catch (Exception ex)
                {
                    SmartHomeConstants.log.ServerErrorsAdd("GroundFloorOn:marantz", ex, "SmartHomeWrapper");
                }
                try
                {
                    await helper.SonosGroundFloorOn(playlistToPlay);
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
                throw helper.ReturnWebError("GroundFloorOn" + ex.Message);
            }
        }
        private async Task<bool> GuestRoom(string playlistToPlay, int volume = 0)
        {
            return await helper.SonosGuestRoom(playlistToPlay, volume);
        }
        private async Task<bool> GuestRoomOff()
        {
            return await helper.SonosGuestRoomOff();
        }

        #endregion private Methoden
    }
}