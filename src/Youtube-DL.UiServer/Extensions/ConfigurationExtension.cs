using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Youtube_DL.UiServer.Options;

namespace Youtube_DL.UiServer.Extensions
{
    /// <summary>	A configuration extension. </summary>
    public static class ConfigurationExtension
    {
        private static readonly Dictionary<Type, string> ConfigurationNames = new Dictionary<Type, string>();

        /// <summary>	Name by type. </summary>
        /// <param name="entityType">	Type of the entity. </param>
        /// <returns>	A string. </returns>
        private static string NameByType(Type entityType)
        {
            if (ConfigurationNames.ContainsKey(entityType)) return ConfigurationNames[entityType];
            var attribute =
                entityType.GetTypeInfo().GetCustomAttributes(typeof(ConfigurationNameAttribute))
                    .SingleOrDefault() as ConfigurationNameAttribute;
            ConfigurationNames.Add(entityType, attribute != null ? attribute.Name : entityType.Name);

            return ConfigurationNames[entityType];
        }

        /// <summary>An IConfigurationRoot extension method that gets a configuration.</summary>
        /// <typeparam name="TSettings">    Type of the settings. </typeparam>
        /// <param name="configuration">    The configuration to act on. </param>
        /// <param name="configurationKey"> The configuration key. </param>
        /// <param name="loggerFactory">    (Optional) The logger factory. </param>
        /// <returns>The configuration.</returns>
        public static TSettings GetConfiguration<TSettings>(this IConfigurationRoot configuration,
            string configurationKey, ILoggerFactory loggerFactory = null)
            where TSettings : new()
        {
            return new ConfigurationSettingsService<TSettings>(configuration, configurationKey, loggerFactory).Get();
        }

        /// <summary>An IConfigurationRoot extension method that gets a configuration.</summary>
        /// <typeparam name="TSettings">    Type of the settings. </typeparam>
        /// <param name="configuration">    The configuration to act on. </param>
        /// <param name="loggerFactory">    (Optional) The logger factory. </param>
        /// <returns>The configuration.</returns>
        public static TSettings GetConfiguration<TSettings>(this IConfigurationRoot configuration, ILoggerFactory loggerFactory = null)
            where TSettings : new()
        {
            return new ConfigurationSettingsService<TSettings>(configuration, NameByType(typeof(TSettings)), loggerFactory).Get();
        }
    }
}