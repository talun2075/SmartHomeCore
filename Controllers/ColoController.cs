using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Classes;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SmartHome.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ColoController : ControllerBase
    {
        public ColoController(IWebHostEnvironment env)
        {
            SmartHomeConstants.Env = env;
        }

        private static readonly string coloip = "192.168.0.29";
        private static readonly ColoClient cololight = new (IPAddress.Parse(coloip));
        [HttpGet("RingBell")]
        public async Task<Boolean> RingBell()
        {
            try
            {
                await cololight.TurnOn();
                await cololight.SetEffect(ColoClient.Effects.RainbowFlow);
                await Task.Delay(2500);
                await cololight.SetEffect(ColoClient.Effects.TheCircus);
                await Task.Delay(2500);
                await cololight.TurnOff();
                return true;
            }
            catch(Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("RingBell", ex, "ColoController");
                return false;
            }
        }

        [HttpGet("On")]
        public async Task<Boolean> On()
        {
            return await cololight.TurnOn();
        }
        [HttpGet("Off")]
        public async Task<Boolean> Off()
        {
            return await cololight.TurnOff();
        }
        [HttpGet("SetColor/{id}")]
        public async Task<Boolean> SetColor(string id)
        {
            return await cololight.SetColour(id);
        }
        [HttpGet("SetEffect/{id}")]
        public async Task<Boolean> SetEffect(ColoClient.Effects id)
        {
            return await cololight.SetEffect(id);
        }
        [HttpGet("SetBrightness/{id}")]
        public async Task<Boolean> SetBrightness(int id)
        {
            return await cololight.SetBrightness(id);
        }
    }
}
