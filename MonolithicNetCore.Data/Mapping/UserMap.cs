using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MonolithicNetCore.Common;
using MonolithicNetCore.Entity;

namespace MonolithicNetCore.Data.Mapping
{
    public class UserMap : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> entity)
        {
            entity.HasKey(c => c.Id);
            entity.ToTable("users", ConfigAppSetting.DatabaseName);
        }
    }
}
