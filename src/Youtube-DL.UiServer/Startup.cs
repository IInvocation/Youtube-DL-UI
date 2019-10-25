using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Youtube_DL.UiServer.Localization;

namespace Youtube_DL.UiServer
{
    /// <summary>	A startup. </summary>
    /// <remarks>
    ///     1: Startup - used to initialize configuration
    ///     2: ConfigureServices - loads all required services
    ///     3: Configure - actual pipeline using the services
    /// </remarks>
    public class Startup
    {
        #region Properties

        /// <summary>	Gets the configuration. </summary>
        /// <value>	The configuration. </value>
        public IConfigurationRoot Configuration { get; }

        /// <summary>Gets the environment.</summary>
        /// <value>The environment.</value>
        public IWebHostEnvironment Environment { get; }

        #endregion

        /// <summary>Constructor.</summary>
        /// <param name="environment">  . </param>
        /// <remarks>Adds configuration from json-Files and environment-variables.</remarks>
        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;

            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // MVC
            services
                .AddMvc(mvcOptions => { 
                    mvcOptions.EnableEndpointRouting = false; 
                })
                .AddViewLocalization()
                .AddDataAnnotationsLocalization()
                .AddRazorOptions(razorOptions => {
                    razorOptions.ViewLocationExpanders.Add(new CultureSubFolderViewLocationExpander());
                });

            // Proxy-Support
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders();
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                // ui-controllers
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");

                // api-controllers
                routes.MapRoute(
                    "api",
                    "api/[controller]");
            });
        }
    }
}
