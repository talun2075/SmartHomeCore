using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Classes;

namespace SmartHome.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(IWebHostEnvironment env)
        {
            SmartHomeConstants.Env = env;
        }

        public IActionResult Index()
        {
            ViewBag.Title = "Smart Home Deconz";
            ViewBag.png = "led-lamp.png";
            ViewBag.svg = "led-lamp.svg";
            ViewBag.png16 = "led-lamp16.png";
            ViewBag.png32 = "led-lamp32.png";
            return View();
        }
        
    }
}
