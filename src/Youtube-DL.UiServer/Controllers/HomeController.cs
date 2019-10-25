using Microsoft.AspNetCore.Mvc;

namespace Youtube_DL.UiServer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}