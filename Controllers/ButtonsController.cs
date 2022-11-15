using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Classes;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHome.Controllers
{
    public class ButtonsController : Controller
    {
        public ButtonsController(IWebHostEnvironment env)
        {
            SmartHomeConstants.Env = env;
        }
        public async Task<ActionResult> Index()
        {
            if (!SmartHomeConstants.KnowingButtons.Any()) await SmartHomeWrapper.ReadButtonXML();
            ViewBag.Buttons = SmartHomeConstants.KnowingButtons;
            ViewBag.Title = "MyStrom Buttons";
            ViewBag.png = "buttons.png";
            ViewBag.svg = "buttons.svg";
            ViewBag.png16 = "buttons16.png";
            ViewBag.png32 = "buttons32.png";
            return View();
        }
    }
}
