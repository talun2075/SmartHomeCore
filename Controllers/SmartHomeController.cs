using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Classes;
using SmartHome.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SmartHomeController : ControllerBase
    {
        public SmartHomeController(IWebHostEnvironment env)
        {
            SmartHomeConstants.Env = env;
        }

        [HttpGet("Touch/{id}")]
        public async Task<Boolean> Touch(string id)
        {
            return await SmartHomeWrapper.Touch(id, GetContext());
        }
        [HttpGet("Single/{id}")]
        public async Task<Boolean> Single(string id)
        {
            return await SmartHomeWrapper.Single(id);
        }
        [HttpGet("Double/{id}")]
        public async Task<Boolean> Double(string id)
        {
            return await SmartHomeWrapper.Double(id);
        }
        [HttpGet("Long/{id}")]
        public async Task<Boolean> Long(string id)
        {
            return await SmartHomeWrapper.Long(id);
        }
        [HttpGet("Generic/{id}")]
        public async Task<Boolean> Generic(string id)
        {
            return await SmartHomeWrapper.Generic(GetContext());
        }
        [HttpGet("CheckTimer")]
        public async Task<Boolean> CheckTimer()
        {
            return await SmartHomeTimerWorker.CheckTimer();
        }
        [HttpGet("AllButtons")]
        public async Task<List<Button>> AllButtons()
        {
            if (!SmartHomeConstants.KnowingButtons.Any())
                await SmartHomeWrapper.ReadButtonXML();
            return SmartHomeConstants.KnowingButtons;
        }
        [HttpGet("Test")]
        public void Test()
        {
           // var K = await SmartHomeHelper.DeconzGardenOn();
        }
        [HttpGet("CupeLiving/{id}")]
        public async Task<Boolean> CupeLiving(int id)
        {
            return await SmartHomeWrapper.CupeLiving(id);
        }
        private ButtonRequest GetContext()
        {
            ButtonRequest br = new ();
            if (HttpContext.Request.Query.ContainsKey("mac"))
            {
                br.Mac = HttpContext.Request.Query["mac"].ToString();
            }
            if (HttpContext.Request.Query.ContainsKey("battery"))
            {
                br.Battery = HttpContext.Request.Query["battery"].ToString();
            }
            if (HttpContext.Request.Query.ContainsKey("wheel"))
            {
                br.Wheel = HttpContext.Request.Query["wheel"].ToString();
            }
            if (HttpContext.Request.Query.ContainsKey("action"))
            {
                br.Action = HttpContext.Request.Query["action"].ToString();
            }
            br.IP = HttpContext.Connection.RemoteIpAddress.ToString();
            return br;
        }
    }
}
