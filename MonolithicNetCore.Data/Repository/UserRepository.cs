using System;
using System.Collections.Generic;
using System.Text;
using MonolithicNetCore.Data.Infrastructure;
using MonolithicNetCore.Entity;

namespace MonolithicNetCore.Data.Repository
{
    public interface IUserRepository : IDataRepository<Users>
    {

    }
    public class UserRepository : EntityFrameworkRepository<Users>, IUserRepository
    {
        public UserRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
