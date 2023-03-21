namespace DSG.Data.Migrations
{
    using DSG.Model.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<DSG.Data.DsgDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }


        //  This method will be called after migrating to the latest version.

        //  You can use the DbSet<T>.AddOrUpdate() helper extension method
        //  to avoid creating duplicate seed data.

        protected override void Seed(DSG.Data.DsgDbContext context)
        {
            //CreateUser(context);
            //CreateProductCategorySample(context);
        }
        private void CreateUser(DsgDbContext context)
        {
            var manager = new UserManager<AppUser>(new UserStore<AppUser>(new DsgDbContext()));
            if (manager.Users.Count() == 0)
            {
                var roleManager = new RoleManager<AppRole>(new RoleStore<AppRole>(new DsgDbContext()));

                var user = new AppUser()
                {
                    UserName = "admin",
                    Email = "admin@dsg.com.vn",
                    EmailConfirmed = true,
                    BirthDay = DateTime.Now,
                    FullName = "Vo Ta Duc",
                    Avatar = "",
                    Gender = true,
                    Status = true
                };
                if (manager.Users.Count(x => x.UserName == "admin") == 0)
                {
                    manager.Create(user, "123456$");

                    if (!roleManager.Roles.Any())
                    {
                        roleManager.Create(new AppRole { Name = "Admin", Description = "Quản trị viên" });
                        roleManager.Create(new AppRole { Name = "Member", Description = "Người dùng" });
                    }

                    var adminUser = manager.FindByName("admin");

                    manager.AddToRoles(adminUser.Id, new string[] { "Admin", "Member" });
                }
            }
        }

        //    private void CreateProductCategorySample(DSG.Data.DsgDbContext context)
        //    {
        //        if (context.ProductCategories.Count() == 0)
        //        {
        //            List<ProductCategory> listProductCategory = new List<ProductCategory>()
        //        {
        //            new ProductCategory() { Name="Áo",Alias="ao",ParentID=1,Status=true },
        //             new ProductCategory() { Name="Quần",Alias="quan",ParentID=1,Status=true },
        //              new ProductCategory() { Name="Giày",Alias="giay",ParentID=1,Status=true },
        //               new ProductCategory() { Name="Dép",Alias="dep",ParentID=2,Status=true }
        //        };
        //            context.ProductCategories.AddRange(listProductCategory);
        //            context.SaveChanges();
        //        }

        //    }

    }
}

