using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Youtube_DL.UiServer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();

            StringBuilder sb = new StringBuilder();

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.FileName = "ffmpeg";
            process.StartInfo.Arguments = "-version";
            process.OutputDataReceived += (sender, args) => sb.AppendLine(args.Data);
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();

            return View(new IndexModel { Output = sb.ToString() });
        }
    }

    public class IndexModel
    {
        public string Output { get; set; }
    }
}