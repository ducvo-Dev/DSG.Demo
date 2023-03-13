using System;

namespace DSG.Data.Infrastructure
{
     public interface IDbFactory : IDisposable
    {
        DsgDbContext Init();
    }
}
