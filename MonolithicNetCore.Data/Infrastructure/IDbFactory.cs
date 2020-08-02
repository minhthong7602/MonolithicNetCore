using System;

namespace MonolithicNetCore.Data.Infrastructure
{
    public interface IDbFactory : IDisposable
    {
        BaseContext Init();

        string BackUpDatabase();
    }
}
