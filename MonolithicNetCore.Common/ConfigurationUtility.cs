using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MonolithicNetCore.Common
{
    public class ConfigurationUtility
    {
        public static IConfigurationRoot GetConfiguration()
        {
            string enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            string jsonFile = $"appsettings.{(string.IsNullOrWhiteSpace(enviroment) ? "Production" : enviroment)}.json";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(jsonFile, optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();
            return configuration;
        }

        public static ConnectionString GetConnectionStrings()
        {
            IConfigurationRoot configuration = GetConfiguration();

            ConnectionString connectionStrings = new ConnectionString();
            configuration.GetSection("ConnectionStrings").Bind(connectionStrings);

            return connectionStrings;
        }
    }
}
