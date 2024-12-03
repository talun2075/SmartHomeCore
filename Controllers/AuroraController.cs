using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Classes;
using SmartHome.Classes.Aurora;
using SmartHome.Classes.Aurora.Core;
using SmartHome.Classes.Aurora.Events;
using SmartHome.Classes.Aurora.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SmartHome.Controllers
{
    /// <summary>
    /// Schnittstelle/API für die Nanoleaf Aurora
    /// </summary>
    [Route("/[controller]")]
    public class AuroraController : Controller
    {
        /// <summary>
        /// Get Data
        /// </summary>
        /// <returns>Nanoleaf Object</returns>
        private static bool EventingInited = false;
        readonly IWebHostEnvironment _env = null;
        public AuroraController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Aurora";
            ViewBag.png = "nanoleaflogo.png";
            ViewBag.png16 = "favicon-16x16.png";
            ViewBag.png32 = "favicon-32x32.png";
            ViewBag.svg = "nanoleaf.svg";
            ViewBag.NavClass = "navSecond";
            return View();
        }

        [HttpGet("Get")]
        public async Task<List<AuroraLigth>> Get()
        {
            await AuroraWrapper.CheckAuroraLiving();
            if (!EventingInited)
            {
                AuroraWrapper.Auroras_Changed += AuroraWrapper_Auroras_Changed;
                EventingInited = true;
            }
            //images creation if in the settings only && is Windows OS
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            Util util = new(_env);
            if (!isWindows || util.CreateOnlyIfNotExist && util.CheckExist())
            {
                return AuroraWrapper.AurorasList;
            }
            foreach (AuroraLigth aurora in AuroraWrapper.AurorasList)
            {
                util.Start(aurora.NLJ.PanelLayout.Layout, aurora.Name.ToLower());
            }
            return AuroraWrapper.AurorasList;
        }

        private void AuroraWrapper_Auroras_Changed(object sender, AuroraLigth e)
        {
            AuroraEventController.EventBroadCast(new Notification() { EventType = (AuroraConstants.AuroraEvents)sender, Aurora = e });
        }

        [HttpGet("RefreshAuroraProperties")]
        public async Task<bool> RefreshAuroraProperties()
        {
            return await AuroraWrapper.RefreshAruroaProperties();
        }
        /// <summary>
        /// Set Scenario
        /// </summary>
        /// <param name="id">Name of Scenario</param>
        /// <param name="v"></param>
        /// <returns></returns>
        [HttpGet("SetSelectedScenario/{id}/{v}")]
        public async Task<string> SetSelectedScenario(string id, string v)
        {
            try
            {
                if (string.IsNullOrEmpty(v) || string.IsNullOrEmpty(id)) return null;
                AuroraLigth a = await AuroraWrapper.GetAurorabySerial(id);
                if (a == null) return null;
                if (a.Scenarios.Contains(v) && a.SelectedScenario != v)
                {
                    await a.SetSelectedScenario(v);
                }
                return a.SelectedScenario;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Set Powerstate
        /// </summary>
        /// <param name="id">Serial</param>
        /// <param name="v">Value of Powerstate</param>
        /// <returns></returns>
        [HttpGet("SetPowerState/{id}/{v}")]
        public async Task<bool> SetPowerState(string id, bool v)
        {
            if (string.IsNullOrEmpty(id)) return false;
            AuroraLigth a = await AuroraWrapper.GetAurorabySerial(id);
            if (a == null) return false;
            if (a.NLJ.State.Powerstate.Value != v)
            {
                await a.SetPowerOn(v);
            }
            return a.NLJ.State.Powerstate.Value;
        }
        /// <summary>
        /// Get Powerstate
        /// </summary>
        /// <param name="id">Serial</param>
        /// <returns></returns>
        [HttpGet("GetPowerOn/{id}")]
        public async Task<bool> GetPowerOn(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            AuroraLigth a = await AuroraWrapper.GetAurorabySerial(id);
            if (a == null) return false;
            return a.NLJ.State.Powerstate.Value;
        }
        /// <summary>
        /// Get Powerstate
        /// </summary>
        /// <param name="room">Room Name</param>
        /// <returns></returns>
        [HttpGet("GetPowerOnForRoom/{room}")]
        public async Task<bool> GetPowerOnForRomm(string room)
        {
            if (string.IsNullOrEmpty(room)) return false;
            var a = await AuroraWrapper.GetAurorasbyRoom(room);
            if (a == null || !a.Any()) return false;
            var poweronlist = a.Where(x => x.NLJ.State.Powerstate.Value == true).ToList();
            return a.Count == poweronlist.Count;
        }
        /// <summary>
        /// Set Powerstate
        /// </summary>
        /// <param name="id">Serial</param>
        /// <param name="v">Value of Powerstate</param>
        /// <returns></returns>
        [HttpGet("SetPowerStateIgnoreOldValue/{id}/{v}")]
        public async Task<bool> SetPowerStateIgnoreOldValue(string id, bool v)
        {
            if (string.IsNullOrEmpty(id)) return false;
            AuroraLigth a = await AuroraWrapper.GetAurorabySerial(id);
            if (a == null) return false;
            await a.SetPowerOn(v, true);
            return a.NLJ.State.Powerstate.Value;
        }
        /// <summary>
        /// Set Powerstate ignoriert den aktuellen Status
        /// </summary>
        /// <param name="id">NAme</param>
        /// <param name="v">Value of Powerstate</param>
        /// <returns></returns>
        [HttpGet("SetPowerStateByName/{name}/{v}")]
        public async Task<bool> SetPowerStateByName(string name, string v)
        {
            if (string.IsNullOrEmpty(v) || string.IsNullOrEmpty(name)) return false;
            if (bool.TryParse(v, out bool po))
            {
                AuroraLigth a = await AuroraWrapper.GetAurorabyName(name);
                if (a == null) return false;
                await a.SetPowerOn(po);
                return a.NLJ.State.Powerstate.Value;
            }
            return false;
        }
        [HttpGet("SetGroupPowerState/{room}/{id}")]
        public async Task<bool> SetGroupPowerState(string room, bool id)
        {
            return await AuroraWrapper.GroupPowerOn(room, id);
        }
        [HttpGet("SetGroupPowerStateAll/{id}")]
        public async Task<bool> SetGroupPowerStateAll(bool id)
        {
            return await AuroraWrapper.GroupPowerOnAll(id, true);
        }
        [HttpGet("SetGroupPowerStateIgnoreOldValue/{room}/{id}")]
        public async Task<bool> SetGroupPowerStateIgnoreOldValue(string room, bool id)
        {
            return await AuroraWrapper.GroupPowerOn(room, id, true);
        }
        /// <summary>
        /// Brightness /Helligkeit
        /// </summary>
        /// <param name="id">Number between min and max</param>
        /// <param name="v">Value of Brightness</param>
        /// <returns>Brightness</returns>
        [HttpGet("SetBrightness/{id}/{v}")]
        public async Task<int> SetBrightness(string id, int v)
        {
            if (string.IsNullOrEmpty(id)) return -999;
            AuroraLigth a = await AuroraWrapper.GetAurorabySerial(id);
            if (a == null) return 0;
            if (v > a.NLJ.State.Brightness.Max || v < a.NLJ.State.Brightness.Min) return 0;
            if (a.NLJ.State.Brightness.Value != v)
            {
                await a.SetBrightness(v);
                await a.RefreshProperties();
            }
            return a.NLJ.State.Brightness.Value;
        }
        /// <summary>
        /// Brightness /Helligkeit
        /// </summary>
        /// <param name="id">Number between min and max</param>
        /// <param name="v">Value of Brightness</param>
        /// <returns>Brightness</returns>
        [HttpGet("SetBrightnessRoom/{room}/{value}/{up}")]
        public async Task<Boolean> SetBrightnessRoom(string room, string value,Boolean up = true)
        {
            if (string.IsNullOrEmpty(room) || string.IsNullOrEmpty(value)) return false;
            if (!int.TryParse(value, out var b)) return false;

            return await AuroraWrapper.SetBrightnessByRoom(room, b, up);
            
        }
        [HttpPost("SetHSVColor/{id}")]
        public async Task<int> SetHSVColor(string id, [FromBody] hsvColor hsvcolor)
        {
            if (string.IsNullOrEmpty(id)) return -999;
            AuroraLigth a = await AuroraWrapper.GetAurorabySerial(id);
            if (a == null) return 0;
            if (hsvcolor.v > a.NLJ.State.Brightness.Max || hsvcolor.v < a.NLJ.State.Brightness.Min) return 0;
            if (hsvcolor.s > a.NLJ.State.Saturation.Max || hsvcolor.s < a.NLJ.State.Saturation.Min) return 0;
            if (hsvcolor.h > a.NLJ.State.Hue.Max || hsvcolor.h < a.NLJ.State.Hue.Min) return 0;
            if (a.NLJ.State.Brightness.Value != hsvcolor.v)
            {
                await a.SetBrightness(hsvcolor.v);
            }
            if (a.NLJ.State.Hue.Value != hsvcolor.h)
            {
                await a.SetHue(hsvcolor.h);
            }
            if (a.NLJ.State.Saturation.Value != hsvcolor.s)
            {
                await a.SetSaturation(hsvcolor.s);
            }
            await a.RefreshProperties();
            return a.NLJ.State.Brightness.Value;
        }
        [HttpGet("SetSaturation/{id}/{v}")]
        public async Task<int> SetSaturation(string id, int v)
        {
            if (string.IsNullOrEmpty(id)) return -999;
            AuroraLigth a = await AuroraWrapper.GetAurorabySerial(id);
            if (a == null) return 0;
            if (v > a.NLJ.State.Saturation.Max || v < a.NLJ.State.Saturation.Min) return 0;
            if (a.NLJ.State.Saturation.Value != v)
            {
                await a.SetSaturation(v);
                await a.RefreshProperties();
            }
            return a.NLJ.State.Saturation.Value;
        }
        [HttpGet("SetColorTemperature/{id}/{v}")]
        public async Task<int> SetColorTemperature(string id, int v)
        {
            if (string.IsNullOrEmpty(id)) return -999;
            AuroraLigth a = await AuroraWrapper.GetAurorabySerial(id);
            if (a == null) return 0;
            if (v > a.NLJ.State.ColorTemperature.Max || v < a.NLJ.State.ColorTemperature.Min) return 0;
            if (a.NLJ.State.ColorTemperature.Value != v)
            {
                await a.SetColorTemperature(v);
                await a.RefreshProperties();
            }
            return a.NLJ.State.ColorTemperature.Value;
        }
        /// <summary>
        /// Setzen eins zufälligen Scenarios
        /// </summary>
        /// <param name="serial">Serial of the Aurora</param>
        /// <returns></returns>
        [HttpGet("SetRandomScenario/{id}/{v}")]
        public async Task<string> SetRandomScenario(string id, bool v = false)
        {
            if (string.IsNullOrEmpty(id)) return null;
            AuroraLigth a = await AuroraWrapper.GetAurorabySerial(id);
            if (a == null) return "false";
            return await a.SetRandomScenario(v);
        }
        /// <summary>
        /// Ermitteln der Gruppenscenarien
        /// </summary>
        /// <param name="id">Dummy</param>
        /// <returns></returns>
        [HttpGet("GetGroupScenario")]
        public async Task<List<string>> GetGroupScenario()
        {
            return await AuroraWrapper.GetGroupScenarios();
        }
        /// <summary>
        /// Ermitteln der Gruppenscenarien
        /// </summary>
        /// <param name="id">Dummy</param>
        /// <returns></returns>
        [HttpGet("GetGroupScenariosForRooms")]
        public async Task<Dictionary<string, List<string>>> GetGroupScenariosForRooms()
        {
            return await AuroraWrapper.GetGroupScenariosforRooms();
        }
        /// <summary>
        /// Setzen der Gruppen Scenarien
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("SetGroupScenario/{room}/{id}")]
        public async Task<string> SetGroupScenario(string room, string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            if (string.IsNullOrEmpty(room)) return null;
            return await AuroraWrapper.SetGroupScenarios(room, id);
        }
        /// <summary>
        /// Setzen der Gruppen Scenarien Random
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("SetGroupRandomScenario/{room}")]
        public async Task<string> SetGroupRandomScenario(string room)
        {
            if (string.IsNullOrEmpty(room)) return null;
            return await AuroraWrapper.SetGroupRandomScenarios(room);
        }
        [HttpGet("SetHue/{id}/{v}")]
        public async Task<bool> SetHue(string id, int v)
        {
            if (string.IsNullOrEmpty(id)) return false;
            AuroraLigth a = await AuroraWrapper.GetAurorabySerial(id);
            if (a == null) return false;
            if (v < a.NLJ.State.Hue.Min || v > a.NLJ.State.Hue.Max) return false;
            await a.SetHue(v);
            await a.RefreshProperties();
            return true;
        }
        /// <summary>
        /// Registriert einen neuen User bei allen gefundenen Aurroas.
        /// Funktioniert nur, wenn auch bei der Aurora 5-7 Sekunden geklickt wurde. 
        /// </summary>
        /// <param name="id">IP</param>
        /// <returns>Token</returns>
        [HttpGet("RegisterNewUser/{id}")]
        public async Task<string> RegisterNewUser(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;
            AuroraLigth a = new("New", id, "NewAurora");
            var retval = await a.NewUser();
            if (string.IsNullOrEmpty(retval))
            {
                retval = "Ein Fehler ist aufgetreten";
            }
            return retval;
        }
        /// <summary>
        /// Umbenennen von Scenarien
        /// </summary>
        /// <param name="id">Serial der Auroras</param>
        /// <param name="v">Altes Scenario @ Neues Scenario Beispiel old@new</param>
        /// <returns>True wenn es geklappt hat.</returns>
        [HttpGet("RenameScenario/{id}/{v}")]
        public async Task<bool> RenameScenario(string id, string v)
        {
            if (string.IsNullOrEmpty(v) || string.IsNullOrEmpty(id)) return false;
            AuroraLigth a = await AuroraWrapper.GetAurorabySerial(id);
            if (a == null || !v.Contains("@")) return false;
            var sp = v.Split('@');
            if (string.IsNullOrEmpty(sp[0]) || string.IsNullOrEmpty(sp[1])) return false;
            return await a.RenameScenario(sp[0], sp[1]);
        }
        /// <summary>
        /// Löschen Scenario
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("DeleteScenario/{id}/{name}")]
        public async Task<bool> DeleteScenario(string id, string name)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(id)) return false;
            AuroraLigth a = await AuroraWrapper.GetAurorabySerial(id);
            if (a == null) return false;
            return await a.DeleteScenario(name);
        }
    }
}
