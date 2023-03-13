using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSG.Model.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DSG.Data
{
    public class DsgDbContext :IdentityDbContext<AppUser>
    {
        public DsgDbContext() : base("DsgConnection")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
        public DbSet<Product> Products { set; get; }
        public DbSet<ProductCategory> ProductCategories { set; get; }
        public DbSet<Error> Errors { set; get; }
        public DbSet<Function> Functions { set; get; }
        public DbSet<Permission> Permissions { set; get; }
        public DbSet<AppRole> AppRoles { set; get; }
        public DbSet<IdentityUserRole> UserRoles { set; get; }

        public static DsgDbContext Create()
        {
            return new DsgDbContext();
        }
        protected override void OnModelCreating(DbModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasKey<string>(r => r.Id).ToTable("AppRoles");
            builder.Entity<IdentityUserRole>().HasKey(i => new { i.UserId, i.RoleId }).ToTable("AppUserRoles");
            builder.Entity<IdentityUserLogin>().HasKey(i => i.UserId).ToTable("AppUserLogins");
            builder.Entity<IdentityUserClaim>().HasKey(i => i.UserId).ToTable("AppUserClaims");
        }
    }
}
