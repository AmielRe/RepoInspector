using Microsoft.Extensions.Configuration;
using RepoInspector.src.Exceptions;
using System;
using System.IO;

namespace RepoInspector.src
{
    public class AppConfig
    {
        private readonly IConfiguration _configuration;

        public AppConfig()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

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

        public int GetInt(string key)
        {
            if (int.TryParse(_configuration[key], out int result))
            {
                return result;
            }

            throw new ConfigurationException($"Invalid or missing integer setting: {key}");
        }

        public TimeSpan GetTimeSpan(string key)
        {
            if (TimeSpan.TryParse(_configuration[key], out TimeSpan result))
            {
                return result;
            }

            throw new ConfigurationException($"Invalid or missing TimeSpan setting: {key}");
        }

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
