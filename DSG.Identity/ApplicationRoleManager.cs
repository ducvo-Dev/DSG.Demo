using DSG.Data;
using DSG.Model.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;


namespace DSG.Identity
{
    public class ApplicationRoleManager : RoleManager<AppRole>
    {
        public ApplicationRoleManager(IRoleStore<AppRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<AppRole>(context.Get<DsgDbContext>()));
        }
    }
}
