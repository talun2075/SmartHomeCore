using System.Collections.Generic;
using System.Linq;
using ExternalDevices;
using InnerCore.Api.DeConz.Models;
using System.Threading.Tasks;
using System;
using System.Net;
using System.Net.Http;
using InnerCore.Api.DeConz.Models.Lights;
using InnerCore.Api.DeConz.ColorConverters.HSB.Extensions;
using InnerCore.Api.DeConz.ColorConverters;
using SmartHome.Classes.Aurora.Core;
using SmartHome.Classes.SmartHome.Interfaces;
using SmartHome.Classes.SmartHome.Data;
using SmartHome.Classes.SmartHome.Util;
using SmartHome.Classes.Shelly;
using SmartHome.Classes.Deconz;

namespace SmartHome.Classes.SmartHome
{

    public class SmartHomeHelper : ISmartHomeHelper
    {
        IShellyWorker shellyworker;
        IDeconzWrapper deconz;
        public SmartHomeHelper(IShellyWorker _sw, IDeconzWrapper _deconz)
        {
            shellyworker = _sw;
            deconz = _deconz;
        }

        #region Auroras
        /// <summary>
        /// Auroras einschalten
        /// </summary>
        public async Task<bool> PowerOnAuroras(string room)
        {
            try
            {
                return await AuroraWrapper.GroupPowerOn(room, true);
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "PowerOnAurora:Block2");
                return false;
            }
        }
        /// <summary>
        /// Auroras ausschalten
        /// </summary>
        public async Task<bool> PowerOffAuroras(string room)
        {
            try
            {
                return await AuroraWrapper.GroupPowerOn(room, false);
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "PowerOffAuroras:Block2");
                return false;
            }
        }
        /// <summary>
        /// Auroras ausschalten
        /// </summary>
        public async Task<bool> PowerOffAuroras()
        {
            try
            {
                return await AuroraWrapper.GroupPowerOnAll(false);
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "PowerOffAuroras:Block2");
                return false;
            }
        }
        /// <summary>
        /// Dedizierte Aurora ausschalten
        /// </summary>
        public async Task<bool> PowerOffAurora(string selectedaurora)
        {
            try
            {

                if (string.IsNullOrEmpty(selectedaurora)) return false;
                AuroraLigth a = await AuroraWrapper.GetAurorabyName(selectedaurora);
                if (a == null) return false;
                await a.SetPowerOn(false);
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "PowerOffAurora2:" + selectedaurora);
                return false;
            }
        }
        /// <summary>
        /// Dedizierte Aurora Anschalten
        /// </summary>
        /// <param name="selectedaurora"></param>
        /// <returns></returns>
        public async Task<bool> PowerOnAurora(string selectedaurora)
        {
            try
            {
                if (string.IsNullOrEmpty(selectedaurora)) return false;
                if (string.IsNullOrEmpty(selectedaurora)) return false;
                AuroraLigth a = await AuroraWrapper.GetAurorabyName(selectedaurora);
                if (a == null) return false;
                await a.SetPowerOn(true);
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "PowerOnAurora:" + selectedaurora);
                return false;
            }
        }
        #endregion Auroras
        #region Denon
        public async Task<bool> PowerOffDenon(bool ignoreinput = false)
        {
            try
            {
                //Daten vom Denon ermitteln
                await Denon.Initialisieren(SmartHomeConstants.Denon.BaseURL);
                //Ist auf Sonos?
                if ((Denon.SelectedInput == DenonInputs.Sonos || ignoreinput) && Denon.PowerOn)
                {
                    //Denon ausschalten.
                    Denon.PowerOn = false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Schaltet den Denon ein und setzt ihn auf Sonos falls nicht schon eingestellt.
        /// </summary>
        public async Task<bool> PowerOnDenon()
        {
            try
            {
                //Denon Verarbeiten.
                await Denon.Initialisieren(SmartHomeConstants.Denon.BaseURL);
                if (Denon.SelectedInput != DenonInputs.Sonos)
                {
                    Denon.SelectedInput = DenonInputs.Sonos;
                }
                if (!Denon.PowerOn)
                {
                    Denon.PowerOn = true;
                }
                if (Denon.Volume != "-40.0")
                {
                    Denon.Volume = "40";
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion Denon
        #region Marantz
        /// <summary>
        /// Schaltet den Marantz aus, wenn auf Sonos geschaltet
        /// </summary>
        //public async Task<Boolean> PowerOffMarantz(bool ignoreinput = false)
        //{
        //    try
        //    {
        //        //Daten vom Marantz ermitteln
        //        await Marantz.Initialisieren(SmartHomeConstants.Marantz.BaseURL);
        //        //Ist auf Sonos?
        //        if ((Marantz.SelectedInput == MarantzInputs.Sonos || ignoreinput) && Marantz.PowerOn)
        //        {
        //            //Marantz ausschalten.
        //            Marantz.PowerOn = false;
        //        }
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        /// <summary>
        /// Schaltet den Marantz ein und setzt ihn auf Sonos falls nicht schon eingestellt.
        /// </summary>
        //public async Task<Boolean> PowerOnMarantz()
        //{
        //    try
        //    {
        //        //Marantz Verarbeiten.
        //        await Marantz.Initialisieren(SmartHomeConstants.Marantz.BaseURL);
        //        if (Marantz.SelectedInput != MarantzInputs.Sonos)
        //        {
        //            Marantz.SelectedInput = MarantzInputs.Sonos;
        //        }
        //        if (!Marantz.PowerOn)
        //        {
        //            Marantz.PowerOn = true;
        //        }
        //        if (Marantz.Volume != "-40.0")
        //        {
        //            Marantz.Volume = "-40.0";
        //        }
        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}
        #endregion Marantz
        #region Deconz
        /// <summary>
        /// Schaltet alle Lampen im Erdgeschoss an.
        /// </summary>
        /// <returns></returns>
        public async Task<DeConzResults> DeconzGroundFloorOn()
        {
            try
            {
                return await ChangeGroupState(deconz.LightCommand.TurnOn(), await deconz.GetGroup("Wohnzimmer"));
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "DeconzGroundFloorOn");
                return deconz.GenerateExceptionMessage(ex, "DeconzGroundFloorOn");
            }
        }

        public async Task<DeConzResults> DeconzLightsPower(string id, bool PowerOn = true)
        {
            return await DeconzLightsPower(new List<string> { id }, PowerOn);
        }
        public async Task<DeConzResults> DeconzLightsPower(List<string> ids, bool PowerOn = true)
        {
            LightCommand lw = new();
            if (PowerOn)
            {
                lw.TurnOn();
            }
            else
            {
                lw.TurnOff();
            }
            return await ChangeGroupState(lw, ids);
        }
        public async Task<bool> CupeLiving(int id)
        {
            /*
             * Licht 31 = Lowboard nur an/aus
             * Licht 22 = Lightstrip Random
             * Licht 30 = Iris Random
             */
            switch (id)
            {
                case 3:
                    //90°
                    Random ran = new();
                    SmartHomeRoom wz = await deconz.GetGroup("Wohnzimmer");
                    await ChangeGroupState(deconz.LightCommand.TurnOn(), wz);
                    foreach (string item in wz.Room.Lights)
                    {
                        if (item == "30")
                        {
                            LightCommand l = new();
                            l.SetColor(RGBColor.Random());
                            l.TransitionTime = new TimeSpan(0, 0, 9);
                            await ChangeGroupState(l, item);
                        }
                        else
                        {

                            LightCommand l = new();
                            l.SetColor(ran.NextDouble(), ran.NextDouble());
                            await ChangeGroupState(l, item);
                        }

                        LightCommand lb = new()
                        {
                            Brightness = (byte)ran.Next(100, 255)
                        };
                        await ChangeGroupState(lb, item);

                    }
                    break;
                case 4:
                    //180°
                    await ChangeGroupState(deconz.LightCommand.TurnOff(), await deconz.GetGroup("Wohnzimmer"));
                    break;
            }

            return true;
        }

        /// <summary>
        /// Schaltet den Garten an
        /// </summary>
        /// <returns></returns>
        public async Task<string> DeconzGardenOn()
        {
            try
            {
                //id 9 & 22
                _ = await ChangeGroupState(deconz.LightCommand.TurnOn().SetColor(SmartHomeConstants.Deconz.RandomRGBColor), "9");
                LightCommand lw = new();
                lw.TurnOn();
                Random rand = new();
                lw.SetColor(SmartHomeConstants.Deconz.RandomRGBColor);
                lw.Brightness = (byte)rand.Next(31, 150);
                _ = await ChangeGroupState(lw, "22");
                return "ok";
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "DeconzGardenon");
                return "error";
                //return deconz.GenerateExceptionMessage(ex, "DeconzGardenOn");
            }
        }
        /// <summary>
        /// Schaltet den Garten aus
        /// </summary>
        /// <returns></returns>
        public async Task<string> DeconzGardenOff()
        {
            try
            {
                _ = await ChangeGroupState(deconz.LightCommand.TurnOff(), await deconz.GetGroup("Garten"));
                return "ok";
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "DeconzGardenOff");
                //return deconz.GenerateExceptionMessage(ex, "DeconzGardenOff");
                return "error";
            }
        }
        /// <summary>
        /// Schaltet, wenn eingeschaltet das Erdgeschoss aus.
        /// </summary>
        /// <returns></returns>
        public async Task<DeConzResults> DeconzGroundFloorOff()
        {
            try
            {
                List<SmartHomeRoom> listofgroups = await deconz.GetGroups();
                //List<Group> lofg = listofgroups.ToList();
                var usethisgroups = listofgroups.Where(x => x.Room.Name.StartsWith("Wohnzimmer") || x.Room.Name.StartsWith("Essz") || x.Room.Name.StartsWith("Küche"));
                List<string> allIds = new();
                foreach (SmartHomeRoom g in usethisgroups)
                {
                    allIds.AddRange(g.Room.Lights);
                }
                List<string> offstate = await DeconzCheckOnState(allIds);
                if (offstate.Any())
                {
                    foreach (var item in offstate)
                    {
                        allIds.Remove(item);
                    }
                }
                if (allIds.Any())
                    return await ChangeGroupState(deconz.LightCommand.TurnOff(), allIds);

                DefaultDeConzResult d = new()
                {
                    Success = new SuccessResult()
                };
                d.Success.Id = "999";
                var hrs = new DeConzResults
                {
                    d
                };
                return hrs;
            }
            catch (Exception ex)
            {
                return deconz.GenerateExceptionMessage(ex, "DeconzGroundFloorOff");
            }
        }
        /// <summary>
        /// Schaltet alle Lampen im ersten Stock aus (Nicht Schlafzimmer)
        /// </summary>
        /// <returns></returns>
        public async Task<DeConzResults> DeconzFirstFloorOff()
        {
            try
            {
                //Ian, Finn ermitteln
                SmartHomeRoom ian = await deconz.GetGroup("Ian");
                SmartHomeRoom finn = await deconz.GetGroup("Finn");
                List<string> allIds = ian.Room.Lights.Union(finn.Room.Lights).ToList();
                List<string> offstate = await DeconzCheckOnState(allIds);
                if (offstate.Any())
                {
                    foreach (var item in offstate)
                    {
                        allIds.Remove(item);
                    }
                }
                if (allIds.Any())
                    return await ChangeGroupState(deconz.LightCommand.TurnOff(), allIds);
                DefaultDeConzResult d = new()
                {
                    Success = new SuccessResult()
                };
                d.Success.Id = "999";
                var hrs = new DeConzResults
                {
                    d
                };
                return hrs;
            }
            catch (Exception ex)
            {
                return deconz.GenerateExceptionMessage(ex, "DeconzFirstFloorOff");
            }
        }
        /// <summary>
        /// Schaltet alle Lampen im Haus aus. 
        /// </summary>
        /// <returns></returns>
        public async Task<DeConzResults> DeconzAllLightsOff()
        {
            try
            {
                List<SmartHomeRoom> listofgroups = await deconz.GetGroups();
                var usethisgroups = listofgroups.Where(x => !x.Room.Name.StartsWith("Fritz") && !x.Room.Name.StartsWith("Garten") && !x.Room.Name.StartsWith("TaLun"));
                List<string> allIds = new();
                foreach (SmartHomeRoom g in usethisgroups)
                {
                    allIds.AddRange(g.Room.Lights);
                }
                List<string> offstate = await DeconzCheckOnState(allIds);
                if (offstate.Any())
                {
                    foreach (var item in offstate)
                    {
                        allIds.Remove(item);
                    }
                }
                if (allIds.Any())
                    return await ChangeGroupState(deconz.LightCommand.TurnOff(), allIds);

                DefaultDeConzResult d = new()
                {
                    Success = new SuccessResult()
                };
                d.Success.Id = "999";
                var hrs = new DeConzResults
                {
                    d
                };
                return hrs;
            }
            catch (Exception ex)
            {
                return deconz.GenerateExceptionMessage(ex, "DeconzAllLightsOff");
            }
        }
        /// <summary>
        /// Schaltet das Esszimmer an oder aus
        /// </summary>
        /// <param name="hs"></param>
        /// <returns></returns>
        public async Task<DeConzResults> DeconzEating(DeconzSwitch hs)
        {
            try
            {
                if (hs == DeconzSwitch.On)
                {
                    var eating = await deconz.GetGroup("Esszimmer");
                    var scene = eating.Room.Scenes.FirstOrDefault(x => x.Name == "Essen");
                    await deconz.UseClient.RecallSceneAsync(scene.Id, eating.Room.Id);
                    var kitchen = await deconz.GetGroup("Küche");
                    Random rdn = new();
                    return await ChangeGroupState(deconz.LightCommand.TurnOn().SetColor(rdn.NextDouble(), rdn.NextDouble()), kitchen.Room.Lights);

                    //var maxscene = kitchen.Room.Scenes.Count > 0 ? kitchen.Room.Scenes.Count - 1 : 0;
                    //var selectedscene = rdn.Next(0, maxscene);
                    //return await deconz.UseClient.RecallSceneAsync(selectedscene.ToString(), kitchen.Room.Id);
                }
                else
                {
                    var e = await deconz.GetGroup("Essz");
                    var k = await deconz.GetGroup("Küche");
                    return await ChangeGroupState(deconz.LightCommand.TurnOff(), e.Room.Lights.Union(k.Room.Lights).ToList());
                }
            }
            catch (Exception ex)
            {
                return deconz.GenerateExceptionMessage(ex, "DeconzEating");
            }
        }
        /// <summary>
        /// Sendet für die Übergebene Gruppe die das command ab.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private async Task<DeConzResults> ChangeGroupState(LightCommand command, SmartHomeRoom group)
        {
            return await ChangeGroupState(command, group.Room.Lights);
        }
        /// <summary>
        /// Sendet für die Übergebene Licht ID die das command ab.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private async Task<DeConzResults> ChangeGroupState(LightCommand command, string id)
        {
            return await ChangeGroupState(command, new List<string>() { id });
        }
        /// <summary>
        /// Sendet für die Übergebenen IDs das Command
        /// </summary>
        /// <param name="ListOfLightIds"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private async Task<DeConzResults> ChangeGroupState(LightCommand command, List<string> ListOfLightIds)
        {
            return await deconz.UseClient.SendCommandAsync(command, ListOfLightIds);
        }
        /// <summary>
        /// Prüft für die Übergebenen Lampen, ob eine eingeschaltet ist und gibt bei der ersten gefundenen ein true zurück.
        /// </summary>
        /// <param name="ListOfLightIds"></param>
        /// <returns>A List with all Lights IDs that are OFF</returns>
        private async Task<List<string>> DeconzCheckOnState(List<string> ListOfLightIds)
        {
            List<string> removefromlist = new();
            foreach (var item in ListOfLightIds)
            {
                var light = await deconz.UseClient.GetLightAsync(item);
                if (light.State.On == true)
                {
                    continue;
                }
                else
                {
                    removefromlist.Add(item);
                }
            }
            return removefromlist;

        }
        #endregion Deconz
        #region Sonos
        public async Task<bool> SonosGuestRoomOff()
        {
            try
            {
                string retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.GuestRoomOff);
                if (bool.TryParse(retval, out bool retvalchecked))
                {
                    return retvalchecked;
                }
                else
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", new Exception(retval), "SonosGuestRoomOff");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "SonosGuestRoomOff:Block2");
                return false;
            }
        }
        public async Task<bool> SonosIanRoomOff()
        {
            try
            {
                string retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.IanRoomOff);
                if (bool.TryParse(retval, out bool retvalchecked))
                {
                    return retvalchecked;
                }
                else
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", new Exception(retval), "SonosIanRoomOff");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "SonosGuestRoomOff:Block2");
                return false;
            }
        }
        public async Task<bool> SonosLivingRoomVolume(bool vol)
        {
            try
            {
                string retval;
                retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.LivingRoomVolume + vol);
                SmartHomeConstants.log.InfoLog("WheelTest", "Vol:" + vol + " Result:" + retval + " ConnString:" + SmartHomeConstants.Sonos.LivingRoomVolume + vol);
                if (bool.TryParse(retval, out bool retvalchecked))
                {
                    return retvalchecked;
                }
                else
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", new Exception(retval), "SonosLivingRoomVolume");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "SonosLivingRoomVolume:Block2");
                return false;
            }
        }
        public async Task<bool> SonosGuestRoomVolume(bool vol)
        {
            try
            {
                string retval;
                retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.GuestRoomVolume + vol);
                if (bool.TryParse(retval, out bool retvalchecked))
                {
                    return retvalchecked;
                }
                else
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", new Exception(retval), "SonosGuestRoomVolume");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "SonosGuestRoomVolume:Block2");
                return false;
            }
        }
        public async Task<bool> SonosGuestRoom(string playlist, int volume = 0)
        {
            try
            {
                string retval;
                if (volume > 0)
                {
                    retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.GuestRoom + "/" + playlist + "/" + volume);
                }
                else
                {
                    retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.GuestRoom + "/" + playlist);
                }

                if (bool.TryParse(retval, out bool retvalchecked))
                {
                    return retvalchecked;
                }
                else
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", new Exception(retval), "SonosGuestRoom");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "SonosGuestRoom:Block2");
                return false;
            }
        }
        public async Task<bool> SonosWorkRoom(string playlist, int volume = 0)
        {
            try
            {
                string retval;
                if (volume > 0)
                {
                    retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.GenericRoomOn + "/" + playlist + "/Arbeit/" + volume);
                }
                else
                {
                    return false;
                }

                if (bool.TryParse(retval, out bool retvalchecked))
                {
                    return retvalchecked;
                }
                else
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", new Exception(retval), "SonosWorkRoom");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "SonosWorkRoom:Block2");
                return false;
            }
        }
        public async Task<bool> SonosWorkRoomOff()
        {
            try
            {
                string retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.GenericRoomOff + "/Arbeit");
                

                if (bool.TryParse(retval, out bool retvalchecked))
                {
                    return retvalchecked;
                }
                else
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", new Exception(retval), "SonosWorkRoom");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "SonosWorkRoom:Block2");
                return false;
            }
        }
        public async Task<bool> SonosIanRoomRandom3()
        {
            try
            {
                string retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.IanRoomRandom3);
                if (bool.TryParse(retval, out bool retvalchecked))
                {
                    return retvalchecked;
                }
                else
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", new Exception(retval), "SonosIanRoomRandomPlay3");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "SonosIanRoomRandomPlay3:Block2");
                return false;
            }
        }
        public async Task<bool> SonosIanRoomRandom2()
        {
            try
            {
                string retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.IanRoomRandom2);
                if (bool.TryParse(retval, out bool retvalchecked))
                {
                    return retvalchecked;
                }
                else
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", new Exception(retval), "SonosIanRoomRandomPlay2");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "SonosIanRoomRandomPlay2:Block2");
                return false;
            }
        }
        public async Task<bool> SonosFinnRoomOff()
        {
            try
            {
                string retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.GenericRoomOff + "/Finn");


                if (bool.TryParse(retval, out bool retvalchecked))
                {
                    return retvalchecked;
                }
                else
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", new Exception(retval), "SonosWorkRoom");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "SonosWorkRoom:Block2");
                return false;
            }
        }
        public async Task<bool> SonosFinn(string playlist)
        {
            try
            {
                string retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.POST, SmartHomeConstants.Sonos.FinnRoomReplace,playlist);


                if (bool.TryParse(retval, out bool retvalchecked))
                {
                    return retvalchecked;
                }
                else
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", new Exception(retval), "SonosWorkRoom");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "SonosWorkRoom:Block2");
                return false;
            }
        }
        public async Task<bool> SonosFinnRoomRandom1()
        {
            try
            {
                string retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.FinnRoomRandom1);
                if (bool.TryParse(retval, out bool retvalchecked))
                {
                    return retvalchecked;
                }
                else
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", new Exception(retval), "SonosFinnRoomRandom1");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "SonosFinnRoomRandom1:Block2");
                return false;
            }
        }
        public async Task<bool> SonosGuestRoomAudioInOn()
        {
            try
            {
                string retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.GuestRoomAudioInOn);
                if (bool.TryParse(retval, out bool retvalchecked))
                {
                    return retvalchecked;
                }
                else
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", new Exception(retval), "SonosGuestRoomAudioInOn");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "SonosGuestRoomAudioInOn:Block2");
                return false;
            }
        }
        public async Task<bool> SonosGuestRoomAudioInOff()
        {
            try
            {
                string retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.GuestRoomAudioInOff);
                if (bool.TryParse(retval, out bool retvalchecked))
                {
                    return retvalchecked;
                }
                else
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", new Exception(retval), "SonosGuestRoomAudioInOff");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "SonosGuestRoomAudioInOn:Block2");
                return false;
            }
        }
        public async Task<bool> SonosGroundFloorOn(string playlist)
        {
            try
            {
                string retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.GroundFloorOn + "/" + playlist);
                if (bool.TryParse(retval, out bool retvalchecked))
                {
                    return retvalchecked;
                }
                else
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", new Exception(retval), "SonosGroundFloorOn");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "SonosGroundFloorOn:Block2");
                return false;
            }
        }
        public async Task<bool> SonosLivingRoomSpezial()
        {
            try
            {
                string retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.LivingRoomSpezial);
                if (bool.TryParse(retval, out bool retvalchecked))
                {
                    return retvalchecked;
                }
                else
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", new Exception(retval), "SonosLivingRoomSpezial");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "SonosLivingRoomSpezial:Block2");
                return false;
            }
        }
        public async Task<bool> SonosStoppAllPlayer()
        {
            try
            {
                string retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.StoppAllPlayers);
                if (bool.TryParse(retval, out bool retvalchecked))
                {
                    return retvalchecked;
                }
                else
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", new Exception(retval), "SonosStoppAllPlayer");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "SonosStoppAllPlayer:Block2");
                return false;
            }
        }
        public async Task<bool> SonosGroundFloorOff()
        {
            try
            {
                string retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.GroundFloorOff);
                if (bool.TryParse(retval, out bool retvalchecked))
                {
                    return retvalchecked;
                }
                else
                {
                    SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", new Exception(retval), "SonosGroundFloorOff");
                    return false;
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "SonosGroundFloorOff:Block2");
                return false;
            }
        }
        #endregion Sonos
        #region Shellys
        /// <summary>
        /// Shellys Gästezimmer schalten
        /// </summary>
        public async Task<bool> PowerShellysGuestRoom(bool powerOn)
        {
            return await shellyworker.PowerGuestRoom(powerOn);
        }
        public async Task<bool> PowerShellysGuestRoomRight(bool powerOn)
        {
            return await shellyworker.PowerGuestRoomRight(powerOn);
        }
        #endregion Shellys
        #region ErrorHandling
        public Exception ReturnWebError(string message, HttpStatusCode hsc = HttpStatusCode.BadRequest)
        {
            var response = new HttpResponseMessage(hsc)
            {
                Content = new StringContent(message, System.Text.Encoding.UTF8, "text/plain")
            };
            return new Exception(message);//todo: Exception Handling korrigieren.
        }
        #endregion ErrorHandling
        public string Test()
        {
            try
            {
                return "No Test implemented";
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "PowerOnAurora:Block2");
                return ex.Message;
            }
        }
    }
}