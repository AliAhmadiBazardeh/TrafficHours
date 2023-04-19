using EmployeesTrafficHours.Models;
using Microsoft.AspNetCore.Mvc;

namespace TrafficHours.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Calculate(IFormFile file)
        {
            try
            {

                return View("~/Views/Home/Calculate.cshtml", new FileModel());

            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;

                return View("~/Views/Home/Index.cshtml", new FileInputModel());
            }

        }
    }
}
