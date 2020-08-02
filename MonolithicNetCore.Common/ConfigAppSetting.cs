using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MonolithicNetCore.Common
{
    public class ConfigAppSetting
    {
        #region Fields

        private static IConfiguration _configuration;
        public static IConfiguration configuration
        {
            get
            {
                if (_configuration == null)
                {
                    Init();
                }
                return _configuration;
            }
        }

        #endregion Fields

        #region Ctr

        public static void Init()
        {
            string enviroment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json")
           .AddJsonFile($"appsettings.{ (string.IsNullOrWhiteSpace(enviroment) ? "Production" : enviroment)}.json");

            _configuration = builder.Build();
        }

        #endregion Ctr

        #region Properties
        public static string DatabaseName => configuration["ConnectionStrings.DatabaseName"] ?? throw new TypeInitializationException("ConfigAppSetting.DatabaseName", new Exception("DatabaseName String has to be init"));

        public static string DirectoryWebApp => configuration["DirectoryWebApp"] ?? throw new TypeInitializationException("ConfigAppSetting.DirectoryWebApp", new Exception("Connection String has to be init"));

        public static string FromEmail => configuration["FromEmail"] ?? throw new TypeInitializationException("ConfigAppSetting.FromEmail", new Exception("Connection String has to be init"));

        public static string EmailPass => configuration["EmailPass"] ?? throw new TypeInitializationException("ConfigAppSetting.EmailPass", new Exception("Connection String has to be init"));

        public static string AdminEmail => configuration["AdminEmail"] ?? throw new TypeInitializationException("ConfigAppSetting.AdminEmail", new Exception("Connection String has to be init"));

        public static List<string> ListEmail => configuration["ListEmail"] != null ? configuration["ListEmail"].Split(";").ToList() : new List<string>();

        public static List<string> ListBlockIP => configuration["ListIPBlock"] != null ? configuration["ListIPBlock"].Split(";").ToList() : new List<string>();

        public static string JwtSigningKey => configuration["JWT:SigningKey"] ?? throw new TypeInitializationException("ConfigAppSetting.JwtSigningKey", new Exception("JWT SigningKey has to be init"));

        public static string JwtIssuer => configuration["JWT:Issuer"] ?? throw new TypeInitializationException("ConfigAppSetting.JwtIssuer", new Exception("JWT Issuer has to be init"));

        public static string JwtAudience => configuration["JWT:Audience"] ?? throw new TypeInitializationException("ConfigAppSetting.Audience", new Exception("JWT Audience has to be init"));

        public static string SqlLocationBackup => configuration["SqlLocationBackup"] ?? throw new TypeInitializationException("ConfigAppSetting.SqlLocationBackup", new Exception("SqlLocationBackup Audience has to be init"));
        public static string DriveServiceCredentials => configuration["GoogleDriver:DriveServiceCredentials"] ?? throw new TypeInitializationException("ConfigAppSetting.DriveServiceCredentials", new Exception("DriveServiceCredentials Audience has to be init"));

        public static string DriverCredentialsPath => configuration["GoogleDriver:CredentialsPath"] ?? throw new TypeInitializationException("ConfigAppSetting.DriverCredentialsPath", new Exception("DriverCredentialsPath Audience has to be init"));
        public static string FolderIdBackupDriver => configuration["GoogleDriver:FolderId"] ?? throw new TypeInitializationException("ConfigAppSetting.FolderIdBackupDriver", new Exception("FolderIdBackupDriver Audience has to be init"));

        public static string JobBackupTimeDriver => configuration["GoogleDriver:JobBackupTime"] ?? throw new TypeInitializationException("ConfigAppSetting.FolderIdBackupDriver", new Exception("FolderIdBackupDriver Audience has to be init"));
        #endregion Properties
    }
}
