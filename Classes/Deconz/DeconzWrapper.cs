using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using InnerCore.Api.DeConz;
using InnerCore.Api.DeConz.Models;
using InnerCore.Api.DeConz.Models.Groups;
using InnerCore.Api.DeConz.Models.Scenes;
using InnerCore.Api.DeConz.Models.Bridge;
using InnerCore.Api.DeConz.Models.Lights;
using InnerCore.Api.DeConz.Models.Sensors;
using InnerCore.Api.DeConz.ColorConverters;
using InnerCore.Api.DeConz.ColorConverters.HSB.Extensions;
using SmartHome.Classes.SmartHome.Data;
using SmartHome.Classes.SmartHome.Util;
using System.Drawing;

namespace SmartHome.Classes.Deconz
{
    /// <summary>
    /// Wrapper für die Deconz Lib
    /// </summary>
    public class DeconzWrapper : IDeconzWrapper
    {
        private DeConzClient _client;
        private BridgeConfig _bridgeConfig;
        private readonly List<DeconzDataConfig> DeconzDataConfigList = [];

        #region Properties
        /// <summary>
        /// Der Primäre DeconzBridgeClient, wird für sämtliche Kommunikation genutzt.
        /// </summary>
        public DeConzClient UseClient
        {
            get
            {
                if (_client == null)
                {
                    try
                    {
                        _client = new DeConzClient(SmartHomeConstants.Deconz.BaseUrl, SmartHomeConstants.Deconz.HttpPort, SmartHomeConstants.Deconz.ApiKey);
                    }
                    catch (Exception e)
                    {
                        SmartHomeConstants.log.ServerErrorsAdd("Property:UseClient", e, "DeconzWrapper");
                    }
                }
                return _client;

            }
        }

        public IEnumerable<Sensor> Sensors { get; set; }
        #endregion Properties
        #region public Methods
        /// <summary>
        /// Sendet für die Übergebene Gruppe die das command ab.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<DeConzResults> ChangeGroupState(LightCommand command, SmartHomeRoom room)
        {
            return await ChangeLightState(command, room.Room.Lights);
        }
        /// <summary>
        /// Läd alle Gruppen aus der Bridge
        /// </summary>
        /// <returns></returns>
        public async Task<List<SmartHomeRoom>> GetGroups()
        {
            var g = await UseClient.GetGroupsAsync();
            if (DeconzDataConfigList.Count == 0)
            {
                ReadDeconzDataConfig();
            }
            List<SmartHomeRoom> shrList = [];
            foreach (Group item in g)
            {
                var shr = new SmartHomeRoom
                {
                    Room = item
                };
                DeconzDataConfig hdc = DeconzDataConfigList.FirstOrDefault(x => x.RoomID.ToString() == item.Id);
                if (hdc != null)
                {
                    shr.Hide = hdc.Hide;
                    shr.SortOrder = hdc.SortOrder;
                    if (hdc.OverWriteName && !string.IsNullOrEmpty(hdc.OverWrittenName))
                        shr.Room.Name = hdc.OverWrittenName;
                }
                if (!shr.Hide)
                    shrList.Add(shr);
            }
            return shrList.OrderBy(x => x.SortOrder).ToList();
        }
        /// <summary>
        /// Läd alle Lichter aus der Bridge
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Light>> GetLights()
        {
            return await UseClient.GetLightsAsync();
        }
        /// <summary>
        /// Läd definierte Lampe
        /// </summary>
        /// <returns></returns>
        public async Task<Light> GetLightById(string id)
        {
            var _lights = await GetLights();
            return _lights.FirstOrDefault(x => x.Id == id);
        }
        /// <summary>
        /// Läd definierte Lampe
        /// </summary>
        /// <returns></returns>
        public async Task<Light> GetLightByName(string name)
        {
            var _lights = await GetLights();
            return _lights.FirstOrDefault(x => x.Name.StartsWith(name));
        }
        /// <summary>
        /// Ermittelt die Gruppe über den Namen
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<SmartHomeRoom> GetGroup(string name)
        {
            var g = await GetGroups();
            return g.FirstOrDefault(x => x.Room.Name.StartsWith(name, StringComparison.CurrentCultureIgnoreCase));
        }
        /// <summary>
        /// Ermittelt die Gruppe über die ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SmartHomeRoom> GetGroup(int id)
        {
            var g = await GetGroups();
            return g.FirstOrDefault(x => x.Room.Id == id.ToString());
        }
        /// <summary>
        /// Ermittelt die Konfiguration der Bridge
        /// </summary>
        /// <returns></returns>
        public async Task<BridgeConfig> GetConfig()
        {
            _bridgeConfig ??= await UseClient.GetConfigAsync();
            return _bridgeConfig;
        }
        /// <summary>
        /// Liefert die Scenen der Gruppe (Raumes) zurück
        /// </summary>
        /// <param name="groupid">ID des Raumes</param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<Scene>> GetScenesbyGroup(string groupid)
        {
            var s = await GetGroup(groupid);
            return s.Room.Scenes;
        }
        /// <summary>
        /// Liefert die Scenen der Gruppe (Raumes) zurück
        /// </summary>
        /// <param name="groupid">ID des Raumes</param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<Scene>> GetScenesbyGroup(Group _group)
        {
            return await GetScenesbyGroup(_group.Id);
        }
        /// <summary>
        /// Gibt immer ein neues Kommando um eine oder mehrere Lampen zu steuern
        /// </summary>
        public LightCommand LightCommand
        {
            get
            {
                return new LightCommand();
            }
        }
        /// <summary>
        /// Gibt immer ein neues Kommando um eine Scene zu steuern.
        /// </summary>
        public SceneCommand SceneCommand
        {
            get
            {
                return new SceneCommand();
            }
        }
        /// <summary>
        /// Generiert eine DeconzResults Exception Message und loggt den eigentlichen Fehler.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="calledMethod"></param>
        /// <returns></returns>
        public DeConzResults GenerateExceptionMessage(Exception ex, string calledMethod)
        {
            SmartHomeConstants.log.ServerErrorsAdd(calledMethod, ex, "DeconzWrapper");
            return new DeConzResults {
                new DefaultDeConzResult
                {

                Success = new SuccessResult(),
                Error = new ErrorResult
                {
                    Type = ex.HResult,
                    Description = ex.Message,
                    Address = calledMethod

                }
            }
        };
        }
        /// <summary>
        /// Sendet für die Übergebenen IDs das Command
        /// </summary>
        /// <param name="ListOfLightIds">Liste von zu schaltenden Lampe(n)</param>
        /// <param name="command">Light Command für Power ON/Off etc.</param>
        /// <returns></returns>
        public async Task<DeConzResults> ChangeLightState(LightCommand command, List<string> ListOfLightIds)
        {
            return await UseClient.SendCommandAsync(command, ListOfLightIds);
        }
        /// <summary>
        /// Sendet für die Übergebenen ID das Command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="LightId"></param>
        /// <returns></returns>
        public async Task<DeConzResults> ChangeLightState(LightCommand command, string LightId)
        {
            return await ChangeLightState(command, [LightId]);
        }

        public async void SetLightColor(string id, string color)
        {
            var light = await GetLightById(id);
            RGBColor rGBColor = new(color);
            await ChangeLightState(LightCommand.TurnOn().SetColor(rGBColor, light.State.Hue == null && light.State.Saturation == null), id);
        }

        #endregion public Methods
        #region private Methoden
        private void ReadDeconzDataConfig()
        {
            try
            {
                string path = SmartHomeConstants.Env.ContentRootPath + "\\Configuration\\DeconzConfiguration.xml";
                // string path = HttpRuntime.AppDomainAppPath + "Configuration\\DeconzConfiguration.xml";
                XmlDocument myXmlDocument = new();
                myXmlDocument.Load(path);
                XmlNodeList Deconzconfig = myXmlDocument.SelectNodes("/Deconz/RoomSortOrder");
                foreach (XmlNode item in Deconzconfig)
                {
                    DeconzDataConfig st = new()
                    {
                        Hide = (item.Attributes["Hide"]?.Value) != null && (item.Attributes["Hide"]?.Value) != "false",
                        OverWriteName = (item.Attributes["OverWriteName"]?.Value) != null && (item.Attributes["OverWriteName"]?.Value) != "false",
                        OverWrittenName = item.Attributes["OverWrittenName"]?.Value ?? string.Empty,
                        RoomID = Convert.ToInt32(item.Attributes["RoomID"].Value),
                        SortOrder = item.Attributes["SortOrder"]?.Value == null ? 100 : Convert.ToInt32(item.Attributes["SortOrder"].Value)
                    };
                    if (DeconzDataConfigList.Count == 0)
                    {
                        DeconzDataConfigList.Add(st);
                    }
                    else
                    {
                        DeconzDataConfig curbutton = DeconzDataConfigList.FirstOrDefault(x => x.RoomID == st.RoomID);
                        if (curbutton == null)
                        {
                            DeconzDataConfigList.Add(st);
                        }
                        else
                        {
                            curbutton.Hide = st.Hide;
                            curbutton.RoomID = st.RoomID;
                            curbutton.SortOrder = st.SortOrder;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("ReadDeconzDataConfig:ReadXML:", ex);
            }
        }
        #endregion private Methoden
    }

    public enum DeconzSwitch
    {
        On,
        Off
    }
}