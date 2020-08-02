using Microsoft.AspNetCore.Hosting;
using MonolithicNetCore.Common;
using System;
using MySql.Data.MySqlClient;
using System.IO.Compression;
using System.IO;

namespace MonolithicNetCore.Data.Infrastructure
{
    public class DbFactory : IDbFactory
    {
        private BaseContext dbContext;
        private IHostingEnvironment hostingEnvironment;
        public DbFactory(BaseContext _dbContext)
        {
            this.dbContext = _dbContext;
        }

        public BaseContext Init()
        {
            return dbContext;
        }

        public string BackUpDatabase()
        {
            string pathDatabaseBackup = $"{hostingEnvironment.WebRootPath}{ConfigAppSetting.SqlLocationBackup}/{DateTime.Now.ToString("yyyyMMMMdd_HHmmss")}.sql";
            string pathDatabaseBackupZip = $"{hostingEnvironment.WebRootPath}{ConfigAppSetting.SqlLocationBackup}/{DateTime.Now.ToString("yyyyMMMMdd_HHmmss")}.zip";
            ConnectionString connections = ConfigurationUtility.GetConnectionStrings();
            using (MySqlConnection conn = new MySqlConnection(connections.PrimaryDatabaseConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cmd))
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        mb.ExportToFile(pathDatabaseBackup);
                        conn.Close();
                        using (ZipArchive archive = ZipFile.Open(pathDatabaseBackupZip, ZipArchiveMode.Create))
                        {
                            archive.CreateEntryFromFile(pathDatabaseBackup, Path.GetFileName(pathDatabaseBackup));
                            File.Delete(pathDatabaseBackup);
                        }
                    }
                }
            }
            return pathDatabaseBackupZip;
        }

        #region IDisposable Support

        private bool isDisposed;

        ~DbFactory()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!isDisposed && disposing)
            {
                DisposeCore();
            }
        }

        protected virtual void DisposeCore()
        {
        }

        #endregion IDisposable Support
    }
}
