using InnerCore.Api.DeConz.Models;
using InnerCore.Api.DeConz.Models.Lights;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Classes.Deconz;
using SmartHome.Classes.SmartHome.Data;
using SmartHome.Classes.SmartHome.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.Controllers
{
    [Route("/[controller]")]
    public class DeconzController : Controller
    {
        private readonly IDeconzWrapper deconz;
        public DeconzController(IWebHostEnvironment env, IDeconzWrapper _deconz )
        {
            SmartHomeConstants.Env = env;
            deconz = _deconz;
            //todo: Jquery aus oberfläche entfernen
        }
        public IActionResult Index()
        {
            ViewBag.Title = "Smart Home Deconz";
            ViewBag.png = "led-lamp.png";
            ViewBag.svg = "led-lamp.svg";
            ViewBag.png16 = "led-lamp16.png";
            ViewBag.png32 = "led-lamp32.png";
            ViewBag.NavClass = "navFirst";
            return View();
        }
        [HttpGet("GetGroups")]
        public async Task<List<SmartHomeRoom>> GetGroups()
        {
            return await deconz.GetGroups();
        }
        [HttpGet("GetLightsbyGroup/{id}")]
        public async Task<SmartHomeRoom> GetLightsbyGroup(int id)
        {
            return await deconz.GetGroup(id);
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
            SmartHomeRoom g = await deconz.GetGroup(id);
            if (g == null) return null;
            var comm = v ? deconz.LightCommand.TurnOn() : deconz.LightCommand.TurnOff();
            return await deconz.ChangeGroupState(comm, g);
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
            var comm = v ? deconz.LightCommand.TurnOn() : deconz.LightCommand.TurnOff();
            return await deconz.ChangeLightState(comm, [id.ToString()]);
        }

        [HttpGet("GetLights")]
        public async Task<IEnumerable<Light>> GetLights()
        {
            return await deconz.GetLights();
        }
        [HttpGet("GetLight/{id}")]
        public async Task<Light> GetLight(string id)
        {
            return await deconz.GetLightById(id);
        }
        [HttpGet("GetLightPowerOn/{id}")]
        public async Task<bool> GetLightPowerOn(string id)
        {
            return (await deconz.GetLightById(id)).State.On;
        }
        [HttpGet("GetRoomPowerOn/{name}")]
        public async Task<bool> GetRoomPowerOn(string name)
        {
            bool retval = false;
            var room = await deconz.GetGroup(name);
            var lights =  await GetLights();
            foreach (var item in room.Room.Lights)
            {
                var checkup = lights.FirstOrDefault(x => x.Id == item);
                if (checkup != null && checkup.State.IsReachable == true&& !checkup.State.On)
                {
                    return retval;
                }
            }

            return true;
        }

        [HttpGet("SetBrightness/{id}/{value}")]
        public async Task<DeConzResults> SetBrightness(int id, int value)
        {
            if (value < 0 || value > 254)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            } 
            if(value == 0)
            {
              return await ToggleLightPowerStateTo(id, false);
            }
            var lc = new LightCommand();
            lc.TurnOn();
            lc.Brightness = Convert.ToByte(value);
            return await deconz.ChangeLightState(lc, id.ToString());

        }

        [HttpPost("SetColor/{id}")]
        public void SetColor(string id,[FromBody]string hexColor)
        {
            deconz.SetLightColor(id, hexColor);
        }
    }
}
