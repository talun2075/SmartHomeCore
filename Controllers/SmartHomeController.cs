using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Classes.SmartHome.Data;
using SmartHome.Classes.SmartHome.Interfaces;
using SmartHome.Classes.SmartHome.Util;
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
        ISmartHomeWrapper shw;
        ISmartHomeTimerWorker shtw;
        public SmartHomeController(IWebHostEnvironment env, ISmartHomeWrapper _shw, ISmartHomeTimerWorker _shtw)
        {
            SmartHomeConstants.Env = env;
            shw = _shw;
            shtw = _shtw;
        }

        [HttpGet("Touch/{id}")]
        public async Task<Boolean> Touch(string id)
        {
            return await shw.Touch(id, GetContext());
        }
        [HttpGet("Single/{id}")]
        public async Task<Boolean> Single(string id)
        {
            return await shw.Single(id);
        }
        [HttpGet("Double/{id}")]
        public async Task<Boolean> Double(string id)
        {
            return await shw.Double(id);
        }
        [HttpGet("Long/{id}")]
        public async Task<Boolean> Long(string id)
        {
            return await shw.Long(id);
        }
        [HttpGet("Generic/{id}")]
        public async Task<Boolean> Generic(string id)
        {
            return await shw.Generic(GetContext());
        }
        [HttpGet("CheckTimer")]
        public async Task<Boolean> CheckTimer()
        {
            return await shtw.CheckTimer();
        }
        [HttpGet("AllButtons")]
        public async Task<List<Button>> AllButtons()
        {
            if (!SmartHomeConstants.KnowingButtons.Any())
                await shw.ReadButtonXML();
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
            return await shw.CupeLiving(id);
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
