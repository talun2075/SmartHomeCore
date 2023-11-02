using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartHome.Classes.ModbusWrapper;

namespace PVUi.Controllers
{
    public class PVController : Controller
    {
        private IDeyeModbus _deye;
        private readonly IOptionsMonitor<LanguageOptions> _LanguageOptions;
        public PVController(IDeyeModbus deyeModbus, IOptionsMonitor<LanguageOptions> languageoptions)
        {
            _deye = deyeModbus;
            _LanguageOptions = languageoptions;
        }

        public IActionResult Index()
        {
            ViewBag.PV = _deye.ReadData();
            ViewBag.Language = _LanguageOptions.CurrentValue;
            ViewBag.Title = "PhotoVoiltaik";
            ViewBag.png = "bat.png";
            ViewBag.svg = "pv.svg";
            ViewBag.png16 = "bat.png";
            ViewBag.png32 = "bat.png";
            ViewBag.NavClass = "navFifths";
            return View();
        }
    }
}