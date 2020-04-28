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
            if (!DirectoryHelper.ValidateDirectories() || !ProgramHelper.ValidatePrograms()) return;

            ProgramHelper.ReportFFmpegVersion();
            ProgramHelper.ReportYoutubeDlVersion();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        #region Helpers

        private static class DirectoryHelper
        {
            public static bool ValidateDirectories()
            {
                Console.WriteLine("Folders as configurated:");
                Console.WriteLine($"DOWNLOAD_DIR = {Environment.GetEnvironmentVariable("DOWNLOAD_DIR")}");
                Console.WriteLine($"OUTPUT_DIR = {Environment.GetEnvironmentVariable("OUTPUT_DIR")}");
                Console.WriteLine($"CONFIG_DIR = {Environment.GetEnvironmentVariable("CONFIG_DIR")}");

                var downloadValidated = ValidateDirectory(Environment.GetEnvironmentVariable("DOWNLOAD_DIR"), true);
                var outputValidated = ValidateDirectory(Environment.GetEnvironmentVariable("OUTPUT_DIR"), true);
                var configValidated = ValidateDirectory(Environment.GetEnvironmentVariable("CONFIG_DIR"), true);

                return downloadValidated && outputValidated;
            }

            static bool ValidateDirectory(string directory, bool ensureWriteAccess)
            {
                Console.WriteLine($"Validating directory \"{directory}\", WriteAccessRequired? {ensureWriteAccess}:");
                try
                {
                    const string testContent = "test";
                    const string testFile = "test.txt";

                    if (!Directory.Exists(directory))
                    {
                        Console.WriteLine("-> Directory does not exist.");
                        return false;
                    }

                    var hasWriteAccess = false;
                    if (ensureWriteAccess)
                    {
                        using (var writer = File.CreateText(Path.Combine(directory, testFile)))
                        {
                            writer.Write(testContent);
                        }

                        var content = File.ReadAllText(Path.Combine(directory, testFile));
                        hasWriteAccess = content == testContent;

                        File.Delete(Path.Combine(directory, testFile));
                    }

                    Console.WriteLine($"-> Validated directory {directory}");
                    return !ensureWriteAccess || hasWriteAccess;
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"-> Could not validate directory {directory}");
                    Console.WriteLine(ex);
                    return false;
                }
            }
        }

        private static class ProgramHelper
        {
            public static bool ValidatePrograms()
            {
                var ffmpeg = ValidateProgram("FFMPEG_LOCATION", "ffmpeg");
                Console.WriteLine($"Validated installation of ffmpeg, result: {ffmpeg}");
                var youtubedl = ValidateProgram("YOUTUBE_DL_LOCATION", "youtube-dl");
                Console.WriteLine($"Validated installation of youtubedl, result: {youtubedl}");

                return ffmpeg && youtubedl;
            }

            private static bool ValidateProgram(string environmentLocation, string name)
            {
                return Environment.OSVersion.Platform == PlatformID.Win32NT ? ValidateProgramWindows(environmentLocation, name) : ValidateProgramUnix(environmentLocation, name);
            }

            private static bool ValidateProgramWindows(string environmentLocation, string name)
            {
                var env = Environment.GetEnvironmentVariable(environmentLocation);

                if (env != null) return File.Exists(Path.Combine(env, name + ".exe"));
                var path = Environment.GetEnvironmentVariable("PATH")?.Split(";", StringSplitOptions.RemoveEmptyEntries);
                return path.Any(p => File.Exists(Path.Combine(p, name + ".exe")));
            }

            private static bool ValidateProgramUnix(string environmentLocation, string name)
            {
                var env = Environment.GetEnvironmentVariable(environmentLocation);

                if (env != null) return File.Exists(Path.Combine(env, name));
                var path = Environment.GetEnvironmentVariable("PATH")?.Split(":", StringSplitOptions.RemoveEmptyEntries);
                return path.Any(p => File.Exists(Path.Combine(p, name)));
            }

            public static void ReportFFmpegVersion()
            {
                using var process = new System.Diagnostics.Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        FileName = "ffmpeg",
                        Arguments = "-version"
                    }
                };
                Console.WriteLine("FFMPEG Version:");
                process.OutputDataReceived += (sender, args) => System.Console.WriteLine(args.Data);
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
            }

            public static void ReportYoutubeDlVersion()
            {
                using var process = new System.Diagnostics.Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        FileName = "youtube-dl",
                        Arguments = "--version"
                    }
                };
                Console.WriteLine("Youtube-DL Version:");
                process.OutputDataReceived += (sender, args) => System.Console.WriteLine(args.Data);
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
            }
        }

        #endregion
    }
}
