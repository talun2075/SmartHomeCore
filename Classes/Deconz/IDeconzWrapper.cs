using InnerCore.Api.DeConz;
using InnerCore.Api.DeConz.Models;
using InnerCore.Api.DeConz.Models.Bridge;
using InnerCore.Api.DeConz.Models.Groups;
using InnerCore.Api.DeConz.Models.Lights;
using InnerCore.Api.DeConz.Models.Scenes;
using InnerCore.Api.DeConz.Models.Sensors;
using SmartHome.Classes.SmartHome.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartHome.Classes.Deconz
{
    public interface IDeconzWrapper
    {
        LightCommand LightCommand { get; }
        SceneCommand SceneCommand { get; }
        IEnumerable<Sensor> Sensors { get; set; }
        DeConzClient UseClient { get; }

        Task<DeConzResults> ChangeGroupState(LightCommand command, SmartHomeRoom room);
        Task<DeConzResults> ChangeLightState(LightCommand command, List<string> ListOfLightIds);
        Task<DeConzResults> ChangeLightState(LightCommand command, string LightId);
        DeConzResults GenerateExceptionMessage(Exception ex, string calledMethod);
        Task<BridgeConfig> GetConfig();
        Task<SmartHomeRoom> GetGroup(int id);
        Task<SmartHomeRoom> GetGroup(string name);
        Task<List<SmartHomeRoom>> GetGroups();
        Task<Light> GetLightById(string id);
        Task<Light> GetLightByName(string name);
        Task<IEnumerable<Light>> GetLights();
        Task<IReadOnlyCollection<Scene>> GetScenesbyGroup(Group _group);
        Task<IReadOnlyCollection<Scene>> GetScenesbyGroup(string groupid);
        void SetLightColor(string id, string color);
    }
}