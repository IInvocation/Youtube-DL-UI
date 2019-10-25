using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Youtube_DL.UiServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ReportFFmpegVersion();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void ReportFFmpegVersion()
        {
            using (System.Diagnostics.Process process = new System.Diagnostics.Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.FileName = "ffmpeg";
                process.StartInfo.Arguments = "-version";
                process.OutputDataReceived += (sender, args) => System.Console.WriteLine(args.Data);
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
            }
        }
    }
}
