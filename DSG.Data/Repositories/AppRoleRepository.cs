using DSG.Data.Infrastructure;
using DSG.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSG.Data.Repositories
{
    public interface IApplicationRoleRepository : IRepository<AppRole>
    {
        IEnumerable<AppRole> GetListRoleByGroupId(int groupId);
       
    }
    public class ApplicationRoleRepository : RepositoryBase<AppRole>, IApplicationRoleRepository
    {
        public ApplicationRoleRepository(IDbFactory dbFactory) : base(dbFactory)
        {

        }
        public IEnumerable<AppRole> GetListRoleByGroupId(int groupId)
        {
            var query = from g in DbContext.AppRoles
                        join ug in DbContext.ApplicationRoleGroups
                        on g.Id equals ug.RoleId
                        where ug.GroupId == groupId
                        select g;
            return query;
        }
    }
}