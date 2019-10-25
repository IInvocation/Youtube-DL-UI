using Microsoft.AspNetCore.Mvc;

namespace Youtube_DL.UiServer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var model = new IndexModel()
            {
                ClientIp = HttpContext.Connection.RemoteIpAddress.ToString()
            };

            return View(model);
        }
    }

    public class IndexModel
    {
        public string ClientIp { get; set; }
    }
}