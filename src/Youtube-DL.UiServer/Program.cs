using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;

namespace Youtube_DL.UiServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello");
            if (ValidateDirectories() && ValidatePrograms())
            {
                ReportFFmpegVersion();
                CreateHostBuilder(args).Build().Run();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static bool ValidateDirectories()
        {
            var downloadValidated = ValidateDirectory(Environment.GetEnvironmentVariable("DOWNLOAD_DIR"), true);
            var outputValidated = ValidateDirectory(Environment.GetEnvironmentVariable("OUTPUT_DIR"), true);
            var configValidated = ValidateDirectory(Environment.GetEnvironmentVariable("CONFIG_DIR"), true);
            return downloadValidated && outputValidated;
        }

        private static bool ValidateDirectory(string directory, bool ensureWriteAccess)
        {
            try
            {
                string testContent = "test";
                string testFile = "test.txt";

                if (!Directory.Exists(directory))
                    return false;
                using (var writer = File.CreateText(Path.Combine(directory, testFile)))
                {
                    writer.Write(testContent);
                }
                
                var content = File.ReadAllText(Path.Combine(directory, testFile));
                var result = content == testContent;

                File.Delete(Path.Combine(directory, testFile));

                Console.WriteLine($"Validated directory {directory}");
                return result;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Could not validate directory {directory}");
                Console.WriteLine(ex);
                return false;
            }            
        }

        private static bool ValidatePrograms()
        {
            var ffmpeg = ValidateProgram("FFMPEG_LOCATION", "ffmpeg");
            Console.WriteLine($"Validated installation of ffmpeg, result: {ffmpeg}");
            var youtubedl = ValidateProgram("YOUTUBE_DL_LOCATION", "youtube-dl");
            Console.WriteLine($"Validated installation of youtubedl, result: {youtubedl}");

            return ffmpeg && youtubedl;
        }

        private static bool ValidateProgram(string environmentLocation, string name)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return ValidateProgramWindows(environmentLocation, name);
            return ValidateProgramUnix(environmentLocation, name);
        }

        private static bool ValidateProgramWindows(string environmentLocation, string name)
        {
            var env = Environment.GetEnvironmentVariable(environmentLocation);

            if (env == null) // examine %PATH%
            {
                var path = Environment.GetEnvironmentVariable("PATH").Split(";", StringSplitOptions.RemoveEmptyEntries);
                return path.Any(p => File.Exists(Path.Combine(p, name + ".exe")));
            }
            else
            {
                return File.Exists(Path.Combine(env, name + ".exe"));
            }
        }

        private static bool ValidateProgramUnix(string environmentLocation, string name)
        {
            var env = Environment.GetEnvironmentVariable(environmentLocation);

            if (env == null) // examine %PATH%
            {
                var path = Environment.GetEnvironmentVariable("PATH").Split(":", StringSplitOptions.RemoveEmptyEntries);
                return path.Any(p => File.Exists(Path.Combine(p, name)));
            }
            else
            {
                return File.Exists(Path.Combine(env, name));
            }
        }

        private static void ReportFFmpegVersion()
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
