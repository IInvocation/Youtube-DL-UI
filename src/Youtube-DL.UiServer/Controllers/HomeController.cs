using Microsoft.AspNetCore.Mvc;
using System.Text;

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