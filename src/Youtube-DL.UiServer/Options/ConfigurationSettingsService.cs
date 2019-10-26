using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Youtube_DL.UiServer.Options
{
    /// <summary>	A configuration settings service. </summary>
    /// <typeparam name="TSettings">	Type of the settings. </typeparam>
    public class ConfigurationSettingsService<TSettings> : ISettingsService<TSettings>
        where TSettings : new()
    {
        /// <summary>	The configuration. </summary>
        protected readonly IConfiguration Configuration;

        /// <summary>	The configuration key. </summary>
        protected readonly string ConfigurationKey;

        /// <summary>The logger.</summary>
        private readonly ILogger _logger;

        /// <summary>	Options for controlling the operation. </summary>
        protected TSettings Settings;

        /// <summary>	Constructor. </summary>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        /// <param name="configuration">	The configuration. </param>
        /// <param name="configKey">		The configuration key. </param>
        public ConfigurationSettingsService(IConfiguration configuration, string configKey)
        {
            if (string.IsNullOrWhiteSpace(configKey)) throw new ArgumentNullException(nameof(configKey));
            ConfigurationKey = configKey;
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>Constructor.</summary>
        /// <param name="configuration">    The configuration. </param>
        /// <param name="configKey">        The configuration key. </param>
        /// <param name="loggerFactory">    The logger factory. </param>
        public ConfigurationSettingsService(IConfiguration configuration, string configKey,
            ILoggerFactory loggerFactory) : this(configuration, configKey)
        {
            if (loggerFactory != null)
            {
                _logger = loggerFactory.CreateLogger(GetType());
            }
        }

        /// <summary>	Gets the get. </summary>
        /// <returns>	The TSettings. </returns>
        public TSettings Get()
        {
            if (Settings != null)
                return Settings;

            _logger?.LogDebug($"Loading Configuration for \"{typeof(TSettings).Name}\" by Key \"{ConfigurationKey}\".");

            // load config-section
            var section = Configuration.GetSection(ConfigurationKey);

            // parse config-section
            Settings = section.Get<TSettings>();

            _logger?.LogDebug($"Parsed Configuration for \"{typeof(TSettings).Name}\", Content: {ObjectDumper.Dump(Settings)}");

            return Settings;
        }
    }
}