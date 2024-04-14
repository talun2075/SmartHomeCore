using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Classes.SmartHome.Util;

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
          return  RedirectFromDomain();
        }
        
        private IActionResult RedirectFromDomain()
        {
            var host = HttpContext.Request.Host.Host;
            switch (host)
            {
                case "aurora.tami":
                case "a.tami":
                    return RedirectToAction("Index", "Aurora");

                case "button.tami":
                case "buttons.tami":
                case "b.tami":
                    return RedirectToAction("Index", "Buttons");

                case "shelly.tami":
                case "s.tami":
                    return RedirectToAction("Index", "Shelly");
                case "pv.tami":
                    return RedirectToAction("Index", "PV");
                case "l.tami":
                case "links.tami":
                case "link.tami":
                    return RedirectToAction("Index", "Links");
                case "r.tami":
                case "rezept.tami":
                case "rezepte.tami":
                    return RedirectToAction("Index", "receipt");
            }
            return RedirectToAction("Index", "Deconz"); ;
        }
    }
}
