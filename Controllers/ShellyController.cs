using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ShellyController : Controller
    {
        private static readonly Dictionary<string, DateTime> ignoreOnTogglePower = new();
        public ShellyController(IWebHostEnvironment env)
        {
            SmartHomeConstants.Env = env;
        }
        public async Task<ActionResult> Index()
        {
            await UpdateShellys();
            ViewBag.Shellys = SmartHomeConstants.Shelly1;
            ViewBag.Title = "Shelly";
            ViewBag.png = "power.png";
            ViewBag.svg = "power.svg";
            ViewBag.png16 = "power.png";
            ViewBag.png32 = "power.png";
            return View();
        }
        [HttpGet("GetShellys")]
        public async Task<List<Shelly1>> GetShellys()
        {
            return await ShellyWorker.Read();
        }
        
        [HttpGet("TogglePower/{sheyllmac}/{power}/{ignoreListActive=false}")]
        public async Task<String> TogglePower(String sheyllmac, Boolean power, Boolean ignoreListActive)
        {
            await UpdateShellys();
            UpdateIgnoreList();
            Shelly1 shelly = SmartHomeConstants.Shelly1.FirstOrDefault(x => x.Device.MacAdress.ToLower() == sheyllmac?.ToLower());
            var onignorelist = ignoreOnTogglePower.ContainsKey(sheyllmac);
            if (shelly == null) return "Shelly nicht gefunden";
            if (!ignoreListActive && onignorelist) return "Shelly wird manuell geschaltet";
            shelly.Relays.First().IsOn = power;
            string url = "http://" + shelly.Name + "/relay/0?turn=";
            if (power)
            {
                url += "on";
            }
            else
            {
                url += "off";
            }
            return await SmartHomeConstants.ConnectToWeb(SmartHomeConstants.RequestEnums.GET, url);
        }
        
        [HttpGet("UpdateShelly/{mac}/{power}")]
        public async Task<ActionResult> UpdateShelly(string mac, Boolean power)
        {
            await UpdateShellys();
            Shelly1 sh = SmartHomeConstants.Shelly1.FirstOrDefault(x => x.Device.MacAdress == mac);
            if (sh == null) return BadRequest();
            sh.Relays.First().IsOn = power;
            return Ok();
        }
        /// <summary>
        /// Wird vom Shelly aufgerufen, wenn der Schalter betätigt wird
        /// Wenn der Shelly noch aus ist, wird der Bewegungssensor damit deaktiviert.
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="power"></param>
        /// <returns></returns>
        [HttpGet("UpdateShellyButton/{mac}/{power}")]
        public async Task<ActionResult> UpdateShellyButton(string mac, Boolean power)
        {
            await UpdateShellys();
            UpdateIgnoreList();
            Shelly1 sh = SmartHomeConstants.Shelly1.FirstOrDefault(x => x.Device.MacAdress == mac);
            if (sh == null) return BadRequest();
            //SmartHomeConstants.log.TraceLog("UpdateShellyButton", "mac:" + mac + " value:" + power);
            if (ignoreOnTogglePower.ContainsKey(mac))
            {
                //SmartHomeConstants.log.TraceLog("trace", "UpdateShellys Remove:" + mac);
                ignoreOnTogglePower.Remove(mac);
            }
            else
            {
                if (!sh.Relays.First().IsOn)
                {
                    //SmartHomeConstants.log.TraceLog("trace", "UpdateShellys add:" + mac);
                    ignoreOnTogglePower.Add(mac, DateTime.Now);
                }
            }
            //wenn shelly gemeldet wird, und aus ist, dann toggle power off für 10 minuten ignorieren.
            return Ok();
        }
        [HttpGet("GetUpdates")]
        public List<Dictionary<string, string>> GetUpdates()
        {
            List<Dictionary<string, string>> retval = new();
            foreach (Shelly1 shelly in SmartHomeConstants.Shelly1)
            {
                Dictionary<string, string> sv = new();
                sv.Add("Name", shelly.Name);
                sv.Add("Mac", shelly.Device.MacAdress);
                sv.Add("IsOn", shelly.Relays.First().IsOn.ToString().ToLower());
                retval.Add(sv);
            }

            return retval;

        }
        private static void UpdateIgnoreList()
        {
            List<string> toremove = new();
            foreach (var item in ignoreOnTogglePower)
            {
                if((DateTime.Now - item.Value).TotalMinutes > 15)
                {
                    toremove.Add(item.Key);
                }
            }
            if (!toremove.Any()) return;

            foreach (var item in toremove)
            {
                //SmartHomeConstants.log.TraceLog("trace", "UpdateIgnoreList Remove:" + item);
                ignoreOnTogglePower.Remove(item);
            }

        }
        private static async Task<Boolean> UpdateShellys()
        {
            try
            {
                if (!SmartHomeConstants.Shelly1.Any() || (DateTime.Now - SmartHomeConstants.ShellyLastChange).Minutes > 5)
                {
                    SmartHomeConstants.Shelly1 = await ShellyWorker.Read();
                    SmartHomeConstants.ShellyLastChange = DateTime.Now;
                }
                return true;
            }
            catch (Exception ex)
            {
                SmartHomeConstants.log.ServerErrorsAdd("UpdateShelly", ex, "ShellyController");
                return false;
            }
        }
    }
}
