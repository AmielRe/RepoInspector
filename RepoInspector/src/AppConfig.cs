using Microsoft.Extensions.Configuration;
using RepoInspector.src.Exceptions;
using System;
using System.IO;

namespace RepoInspector.src
{
    /// <summary>
    /// Provides access to application configuration settings from the appsettings.json file.
    /// </summary>
    public class AppConfig
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the AppConfig class and loads the configuration from appsettings.json.
        /// </summary>
        public AppConfig()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        /// <summary>
        /// Gets a string setting from the configuration by key.
        /// </summary>
        /// <param name="key">The key for the string setting.</param>
        /// <returns>The string value associated with the key.</returns>
        /// <exception cref="ConfigurationException">Thrown if the key is invalid or missing.</exception>
        public string GetString(string key)
        {
            try
            {
                return _configuration[key];
            }
            catch (Exception)
            {
                throw new ConfigurationException($"Invalid or missing string setting: {key}");
            }
        }

        /// <summary>
        /// Gets an integer setting from the configuration by key.
        /// </summary>
        /// <param name="key">The key for the integer setting.</param>
        /// <returns>The integer value associated with the key.</returns>
        /// <exception cref="ConfigurationException">Thrown if the key is invalid or missing, or if the value cannot be parsed as an integer.</exception>
        public int GetInt(string key)
        {
            if (int.TryParse(_configuration[key], out int result))
            {
                return result;
            }

            throw new ConfigurationException($"Invalid or missing integer setting: {key}");
        }

        /// <summary>
        /// Gets a TimeSpan setting from the configuration by key.
        /// </summary>
        /// <param name="key">The key for the TimeSpan setting.</param>
        /// <returns>The TimeSpan value associated with the key.</returns>
        /// <exception cref="ConfigurationException">Thrown if the key is invalid or missing, or if the value cannot be parsed as a TimeSpan.</exception>
        public TimeSpan GetTimeSpan(string key)
        {
            if (TimeSpan.TryParse(_configuration[key], out TimeSpan result))
            {
                return result;
            }

            throw new ConfigurationException($"Invalid or missing TimeSpan setting: {key}");
        }

        /// <summary>
        /// Gets a configuration section by its name.
        /// </summary>
        /// <param name="sectionName">The name of the configuration section.</param>
        /// <returns>The configuration section with the specified name.</returns>
        /// <exception cref="ConfigurationException">Thrown if the section name is invalid or missing.</exception>
        public IConfigurationSection GetSection(string sectionName)
        {
            try
            {
                return _configuration.GetSection(sectionName);
            }
            catch (Exception)
            {
                throw new ConfigurationException($"Invalid or missing section setting: {sectionName}");
            }
        }
    }
}
