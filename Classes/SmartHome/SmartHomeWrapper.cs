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
            return true;
        }
        public async Task<bool> Single(string mac)
        {
            return true;
        }
        public async Task<bool> Double(string mac)
        {
            return true;
        }
        public async Task<bool> Long(string mac)
        {
            return true;
        }

        public async Task<bool> CupeLiving(int id)
        {
            return await helper.CupeLiving(id);
        }

        public Logging generic_log = new(new LoggerWrapperConfig() { ConfigName = "GenericLog", InfoFileName = "battery.txt" });
       
        #endregion ClickEvents
        #region private Methoden

     

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