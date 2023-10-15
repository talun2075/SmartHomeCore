using Microsoft.AspNetCore.Mvc;

namespace SmartHome.Controllers
{
    public class OverView : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
