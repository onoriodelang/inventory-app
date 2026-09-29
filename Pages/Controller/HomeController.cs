using Microsoft.AspNetCore.Mvc;

namespace inventory_app.Pages.Controller
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
