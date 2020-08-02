using Microsoft.EntityFrameworkCore;
using MonolithicNetCore.Common;
using MonolithicNetCore.Data.Mapping;
using MonolithicNetCore.Common;
using MonolithicNetCore.Data.Mapping;
using MonolithicNetCore.Entity;

namespace MonolithicNetCore.Data
{
    public class BaseContext : DbContext
    {
        protected string _connectionString;
        public virtual DbSet<Users> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                ConnectionString connections = ConfigurationUtility.GetConnectionStrings();
                _connectionString = connections.PrimaryDatabaseConnectionString;
            }

            optionsBuilder.UseMySQL(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserMap());
        }

        public BaseContext(DbContextOptions<BaseContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public BaseContext()
        {

        }
    }
}
