using InnerCore.Api.DeConz.ColorConverters;
using InnerCore.Api.DeConz.Models;
using InnerCore.Api.DeConz.Models.Lights;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Classes;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace SmartHome.Controllers
{
    [Route("/[controller]")]
    public class DeconzController : ApiController
    {
        public DeconzController(IWebHostEnvironment env)
        {
            SmartHomeConstants.Env = env;
        }

        [HttpGet("GetGroups")]
        public async Task<List<SmartHomeRoom>> GetGroups()
        {
            return await DeconzWrapper.GetGroups();
        }
        [HttpGet("GetLightsbyGroup/{id}")]
        public async Task<SmartHomeRoom> GetLightsbyGroup(int id)
        {
            return await DeconzWrapper.GetGroup(id);
        }
        /// <summary>
        /// Schaltet für eine Gruppe das Licht An/Aus
        /// </summary>
        /// <param name="id"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        [HttpGet("ToggleGroupPowerStateTo/{id}/{v}")]
        public async Task<DeConzResults> ToggleGroupPowerStateTo(int id, bool v)
        {
            SmartHomeRoom g = await DeconzWrapper.GetGroup(id);
            if (g == null) return null;
            var comm = v ? DeconzWrapper.LightCommand.TurnOn() : DeconzWrapper.LightCommand.TurnOff();
            return await DeconzWrapper.ChangeGroupState(comm, g);
        }
        /// <summary>
        /// Schaltet für eine Gruppe das Licht An/Aus
        /// </summary>
        /// <param name="id"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        [HttpGet("ToggleLightPowerStateTo/{id}/{v}")]
        public async Task<DeConzResults> ToggleLightPowerStateTo(int id, bool v)
        {
            var comm = v ? DeconzWrapper.LightCommand.TurnOn() : DeconzWrapper.LightCommand.TurnOff();
            return await DeconzWrapper.ChangeLightState(comm, new List<string> { id.ToString() });
        }

        [HttpGet("GetLights")]
        public async Task<IEnumerable<Light>> GetLights()
        {
            return await DeconzWrapper.GetLights();
        }
        [HttpGet("GetLight/{id}")]
        public async Task<Light> GetLight(string id)
        {
            return await DeconzWrapper.GetLightById(id);
        }
        [HttpPost("SetColor/{id}")]
        public void SetColor(string id,[FromBody]string hexColor)
        {
            DeconzWrapper.SetLightColor(id, hexColor);
        }
    }
}
