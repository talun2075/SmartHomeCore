using Microsoft.AspNetCore.Mvc;
using SmartHome.Classes.Old;

namespace SmartHome.Controllers
{
    public class OverViewController : Controller
    {
        public IActionResult Index()
        {
            OverViewWorker.ReadOverViewConfig();
            ViewBag.OverViews = OverViewWorker.OverViews;
            ViewBag.png = "power.png";
            ViewBag.svg = "power.svg";
            ViewBag.png16 = "power.png";
            ViewBag.png32 = "power.png";
            ViewBag.NavClass = "navFourth";
            return View();
        }

        public IActionResult Reset()
        {
            OverViewWorker.OverViews = new();
            return View();
        }
    }
}
