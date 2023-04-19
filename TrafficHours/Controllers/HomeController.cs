using Microsoft.AspNetCore.Mvc;

namespace TrafficHours.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
