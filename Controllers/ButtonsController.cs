using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Classes.SmartHome.Interfaces;
using SmartHome.Classes.SmartHome.Util;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.Controllers
{
    public class ButtonsController : Controller
    {
        ISmartHomeWrapper shw;
        public ButtonsController(IWebHostEnvironment env, ISmartHomeWrapper _shw)
        {
            SmartHomeConstants.Env = env;
            shw = _shw;
        }
        public async Task<ActionResult> Index()
        {
            if (!SmartHomeConstants.KnowingButtons.Any()) await shw.ReadButtonXML();
            ViewBag.Buttons = SmartHomeConstants.KnowingButtons;
            ViewBag.Title = "MyStrom Buttons";
            ViewBag.png = "buttons.png";
            ViewBag.svg = "buttons.svg";
            ViewBag.png16 = "buttons16.png";
            ViewBag.png32 = "buttons32.png";
            ViewBag.NavClass = "navThird";
            return View();
        }
    }
}
