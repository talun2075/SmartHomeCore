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
using System.Xml.Linq;

namespace SmartHome.Classes
{

    public static class SmartHomeHelper
    {
        #region Auroras
        /// <summary>
        /// Auroras einschalten
        /// </summary>
        public static async Task<bool> PowerOnAuroras(string room)
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
        public static async Task<Boolean> PowerOffAuroras()
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
        public static async Task<Boolean> PowerOffAurora(String selectedaurora)
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
        public static async Task<Boolean> PowerOnAurora(String selectedaurora)
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
        public static async Task<Boolean> PowerOffDenon(bool ignoreinput = false)
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
        public static async Task<Boolean> PowerOnDenon()
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
        //public static async Task<Boolean> PowerOffMarantz(bool ignoreinput = false)
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
        //public static async Task<Boolean> PowerOnMarantz()
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
        public static async Task<DeConzResults> DeconzGroundFloorOn()
        {
            try
            {
                return await ChangeGroupState(DeconzWrapper.LightCommand.TurnOn(), await DeconzWrapper.GetGroup("Wohnzimmer"));
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "DeconzGroundFloorOn");
                return DeconzWrapper.GenerateExceptionMessage(ex, "DeconzGroundFloorOn");
            }
        }

        public static async Task<DeConzResults> DeconzLightsPower(string id, Boolean PowerOn = true)
        {
            return await DeconzLightsPower(new List<string> { id }, PowerOn);
        }
        public static async Task<DeConzResults> DeconzLightsPower(List<string> ids, Boolean PowerOn = true)
        {
            LightCommand lw = new();
            if (PowerOn) {
                lw.TurnOn();
            }
            else
            {
                lw.TurnOff();
            }
            return await ChangeGroupState(lw, ids);
        }
        internal async static Task<Boolean> CupeLiving(int id)
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
                    SmartHomeRoom wz = await DeconzWrapper.GetGroup("Wohnzimmer");
                    await ChangeGroupState(DeconzWrapper.LightCommand.TurnOn(), wz);
                    foreach (string item in wz.Room.Lights)
                    {
                        if(item == "30")
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
                    await ChangeGroupState(DeconzWrapper.LightCommand.TurnOff(), await DeconzWrapper.GetGroup("Wohnzimmer"));
                    break;
            }

            return true;
        }

        /// <summary>
        /// Schaltet den Garten an
        /// </summary>
        /// <returns></returns>
        public static async Task<DeConzResults> DeconzGardenOn()
        {
            try
            {
                //id 9 & 22
                _ = await ChangeGroupState(DeconzWrapper.LightCommand.TurnOn().SetColor(SmartHomeConstants.Deconz.RandomRGBColor), "9");
                LightCommand lw = new();
                lw.TurnOn();
                Random rand = new();
                lw.SetColor(SmartHomeConstants.Deconz.RandomRGBColor);
                lw.Brightness = (byte)rand.Next(31, 150);
                var retval = await ChangeGroupState(lw, "22");
                return retval;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "DeconzGardenon");
                return DeconzWrapper.GenerateExceptionMessage(ex, "DeconzGardenOn");
            }
        }
        /// <summary>
        /// Schaltet den Garten aus
        /// </summary>
        /// <returns></returns>
        public static async Task<DeConzResults> DeconzGardenOff()
        {
            try
            {
                var retval = await ChangeGroupState(DeconzWrapper.LightCommand.TurnOff(), await DeconzWrapper.GetGroup("Garten"));
                return retval;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("SmartHomeHelper", ex, "DeconzGardenOff");
                return DeconzWrapper.GenerateExceptionMessage(ex, "DeconzGardenOff");
            }
        }
        /// <summary>
        /// Schaltet, wenn eingeschaltet das Erdgeschoss aus.
        /// </summary>
        /// <returns></returns>
        public static async Task<DeConzResults> DeconzGroundFloorOff()
        {
            try
            {
                List<SmartHomeRoom> listofgroups = await DeconzWrapper.GetGroups();
                //List<Group> lofg = listofgroups.ToList();
                var usethisgroups = listofgroups.Where(x => x.Room.Name.StartsWith("Wohnzimmer") || x.Room.Name.StartsWith("Essz") || x.Room.Name.StartsWith("Küche"));
                List<string> allIds = new ();
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
                    return await ChangeGroupState(DeconzWrapper.LightCommand.TurnOff(), allIds);

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
                return DeconzWrapper.GenerateExceptionMessage(ex, "DeconzGroundFloorOff");
            }
        }
        /// <summary>
        /// Schaltet alle Lampen im ersten Stock aus (Nicht Schlafzimmer)
        /// </summary>
        /// <returns></returns>
        public static async Task<DeConzResults> DeconzFirstFloorOff()
        {
            try
            {
                //Ian, Finn ermitteln
                SmartHomeRoom ian = await DeconzWrapper.GetGroup("Ian");
                SmartHomeRoom finn = await DeconzWrapper.GetGroup("Finn");
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
                    return await ChangeGroupState(DeconzWrapper.LightCommand.TurnOff(), allIds);
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
                return DeconzWrapper.GenerateExceptionMessage(ex, "DeconzFirstFloorOff");
            }
        }
        /// <summary>
        /// Schaltet alle Lampen im Haus aus. 
        /// </summary>
        /// <returns></returns>
        public static async Task<DeConzResults> DeconzAllLightsOff()
        {
            try
            {
                List<SmartHomeRoom> listofgroups = await DeconzWrapper.GetGroups();
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
                    return await ChangeGroupState(DeconzWrapper.LightCommand.TurnOff(), allIds);

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
                return DeconzWrapper.GenerateExceptionMessage(ex, "DeconzAllLightsOff");
            }
        }
        /// <summary>
        /// Schaltet das Esszimmer an oder aus
        /// </summary>
        /// <param name="hs"></param>
        /// <returns></returns>
        public static async Task<DeConzResults> DeconzEating(DeconzSwitch hs)
        {
            try
            {
                if (hs == DeconzSwitch.On)
                {
                    var eating = await DeconzWrapper.GetGroup("Esszimmer");
                    var scene = eating.Room.Scenes.FirstOrDefault(x => x.Name == "Essen");
                    await DeconzWrapper.UseClient.RecallSceneAsync(scene.Id, eating.Room.Id);
                    var kitchen = await DeconzWrapper.GetGroup("Küche");
                    Random rdn = new();
                    return await ChangeGroupState(DeconzWrapper.LightCommand.TurnOn().SetColor(rdn.NextDouble(), rdn.NextDouble()), kitchen.Room.Lights);
                    
                    //var maxscene = kitchen.Room.Scenes.Count > 0 ? kitchen.Room.Scenes.Count - 1 : 0;
                    //var selectedscene = rdn.Next(0, maxscene);
                    //return await DeconzWrapper.UseClient.RecallSceneAsync(selectedscene.ToString(), kitchen.Room.Id);
                }
                else
                {
                    var e = await DeconzWrapper.GetGroup("Essz");
                    var k = await DeconzWrapper.GetGroup("Küche");
                    return await ChangeGroupState(DeconzWrapper.LightCommand.TurnOff(), e.Room.Lights.Union(k.Room.Lights).ToList());
                }
            }
            catch (Exception ex)
            {
                return DeconzWrapper.GenerateExceptionMessage(ex, "DeconzEating");
            }
        }
        /// <summary>
        /// Sendet für die Übergebene Gruppe die das command ab.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private static async Task<DeConzResults> ChangeGroupState(LightCommand command, SmartHomeRoom group)
        {
            return await ChangeGroupState(command, group.Room.Lights);
        }
        /// <summary>
        /// Sendet für die Übergebene Licht ID die das command ab.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private static async Task<DeConzResults> ChangeGroupState(LightCommand command, string id)
        {
            return await ChangeGroupState(command, new List<string>() {id });
        }
        /// <summary>
        /// Sendet für die Übergebenen IDs das Command
        /// </summary>
        /// <param name="ListOfLightIds"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private static async Task<DeConzResults> ChangeGroupState(LightCommand command, List<string> ListOfLightIds)
        {
            return await DeconzWrapper.UseClient.SendCommandAsync(command, ListOfLightIds);
        }
        /// <summary>
        /// Prüft für die Übergebenen Lampen, ob eine eingeschaltet ist und gibt bei der ersten gefundenen ein true zurück.
        /// </summary>
        /// <param name="ListOfLightIds"></param>
        /// <returns>A List with all Lights IDs that are OFF</returns>
        private static async Task<List<string>> DeconzCheckOnState(List<string> ListOfLightIds)
        {
            List<string> removefromlist = new();
            foreach (var item in ListOfLightIds)
            {
                var light = await DeconzWrapper.UseClient.GetLightAsync(item);
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
        public static async Task<bool> SonosGuestRoomOff()
        {
            try
            {
                String retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.GuestRoomOff);
                if (Boolean.TryParse(retval, out Boolean retvalchecked))
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
        public static async Task<bool> SonosIanRoomOff()
        {
            try
            {
                String retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.IanRoomOff);
                if (Boolean.TryParse(retval, out Boolean retvalchecked))
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
        public static async Task<bool> SonosLivingRoomVolume(Boolean vol)
        {
            try
            {
                String retval;
                    retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.LivingRoomVolume + vol);
                SmartHomeConstants.log.InfoLog("WheelTest", "Vol:" + vol+ " Result:"+retval+" ConnString:"+ SmartHomeConstants.Sonos.LivingRoomVolume + vol);
                if (Boolean.TryParse(retval, out Boolean retvalchecked))
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
        public static async Task<bool> SonosGuestRoomVolume(Boolean vol)
        {
            try
            {
                String retval;
                retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.GuestRoomVolume + vol);
                if (Boolean.TryParse(retval, out Boolean retvalchecked))
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
        public static async Task<bool> SonosGuestRoom(string playlist, int volume = 0)
        {
            try
            {
                String retval;
                if (volume > 0)
                {
                    retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.GuestRoom + "/" + playlist+"/"+volume);
                }
                else
                {
                    retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.GuestRoom + "/" + playlist);
                }

                if (Boolean.TryParse(retval, out Boolean retvalchecked))
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
        public static async Task<bool> SonosIanRoomRandom3()
        {
            try
            {
                String retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.IanRoomRandom3);
                if (Boolean.TryParse(retval, out Boolean retvalchecked))
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
        public static async Task<bool> SonosIanRoomRandom2()
        {
            try
            {
                String retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.IanRoomRandom2);
                if (Boolean.TryParse(retval, out Boolean retvalchecked))
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
        public static async Task<bool> SonosGuestRoomAudioInOn()
        {
            try
            {
                String retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.GuestRoomAudioInOn);
                if (Boolean.TryParse(retval, out Boolean retvalchecked))
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
        public static async Task<bool> SonosGuestRoomAudioInOff()
        {
            try
            {
                String retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.GuestRoomAudioInOff);
                if (Boolean.TryParse(retval, out Boolean retvalchecked))
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
        public static async Task<bool> SonosGroundFloorOn(string playlist)
        {
            try
            {
                String retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.GroundFloorOn + "/" + playlist);
                if (Boolean.TryParse(retval, out Boolean retvalchecked))
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
        public static async Task<bool> SonosLivingRoomSpezial()
        {
            try
            {
                String retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.LivingRoomSpezial);
                if (Boolean.TryParse(retval, out Boolean retvalchecked))
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
        public static async Task<bool> SonosStoppAllPlayer()
        {
            try
            {
                String retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.StoppAllPlayers);
                if (Boolean.TryParse(retval, out Boolean retvalchecked))
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
        public static async Task<bool> SonosGroundFloorOff()
        {
            try
            {
                String retval = await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, SmartHomeConstants.Sonos.GroundFloorOff);
                if (Boolean.TryParse(retval, out Boolean retvalchecked))
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
        public static async Task<bool> PowerShellysGuestRoom(Boolean powerOn)
        {
            return await ShellyWorker.PowerGuestRoom(powerOn);
        }
        public static async Task<bool> PowerShellysGuestRoomRight(Boolean powerOn)
        {
            return await ShellyWorker.PowerGuestRoomRight(powerOn);
        }
        #endregion Shellys
        #region ErrorHandling
        public static Exception ReturnWebError(string message, HttpStatusCode hsc = HttpStatusCode.BadRequest)
        {
            var response = new HttpResponseMessage(hsc)
            {
                Content = new StringContent(message, System.Text.Encoding.UTF8, "text/plain")
            };
            return new Exception(message);//todo: Exception Handling korrigieren.
        }
        #endregion ErrorHandling
        public static  String Test()
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