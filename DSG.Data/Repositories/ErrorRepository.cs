using DSG.Data.Infrastructure;
using DSG.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSG.Data.Repositories
{
    public interface IErrorRepository : IRepository<Error>
    {
    }

    public class ErrorRepository : RepositoryBase<Error>, IErrorRepository
    {
        public ErrorRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
