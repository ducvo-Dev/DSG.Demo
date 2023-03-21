namespace DSG.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_data04 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ApplicationRoleGroups", "GroupId", "dbo.ApplicationGroups");
            DropForeignKey("dbo.ApplicationRoleGroups", "RoleId", "dbo.AppRoles");
            DropForeignKey("dbo.ApplicationUserGroups", "GroupId", "dbo.ApplicationGroups");
            DropForeignKey("dbo.ApplicationUserGroups", "UserId", "dbo.AppUsers");
            DropIndex("dbo.ApplicationRoleGroups", new[] { "GroupId" });
            DropIndex("dbo.ApplicationRoleGroups", new[] { "RoleId" });
            DropIndex("dbo.ApplicationUserGroups", new[] { "UserId" });
            DropIndex("dbo.ApplicationUserGroups", new[] { "GroupId" });
            AddColumn("dbo.ApplicationRoleGroups", "ApplicationGroup_ID", c => c.Int());
            AddColumn("dbo.ApplicationRoleGroups", "AppRole_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.ApplicationUserGroups", "ApplicationGroup_ID", c => c.Int());
            AddColumn("dbo.ApplicationUserGroups", "AppUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.ApplicationRoleGroups", "ApplicationGroup_ID");
            CreateIndex("dbo.ApplicationRoleGroups", "AppRole_Id");
            CreateIndex("dbo.ApplicationUserGroups", "ApplicationGroup_ID");
            CreateIndex("dbo.ApplicationUserGroups", "AppUser_Id");
            AddForeignKey("dbo.ApplicationRoleGroups", "ApplicationGroup_ID", "dbo.ApplicationGroups", "ID");
            AddForeignKey("dbo.ApplicationRoleGroups", "AppRole_Id", "dbo.AppRoles", "Id");
            AddForeignKey("dbo.ApplicationUserGroups", "ApplicationGroup_ID", "dbo.ApplicationGroups", "ID");
            AddForeignKey("dbo.ApplicationUserGroups", "AppUser_Id", "dbo.AppUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUserGroups", "AppUser_Id", "dbo.AppUsers");
            DropForeignKey("dbo.ApplicationUserGroups", "ApplicationGroup_ID", "dbo.ApplicationGroups");
            DropForeignKey("dbo.ApplicationRoleGroups", "AppRole_Id", "dbo.AppRoles");
            DropForeignKey("dbo.ApplicationRoleGroups", "ApplicationGroup_ID", "dbo.ApplicationGroups");
            DropIndex("dbo.ApplicationUserGroups", new[] { "AppUser_Id" });
            DropIndex("dbo.ApplicationUserGroups", new[] { "ApplicationGroup_ID" });
            DropIndex("dbo.ApplicationRoleGroups", new[] { "AppRole_Id" });
            DropIndex("dbo.ApplicationRoleGroups", new[] { "ApplicationGroup_ID" });
            DropColumn("dbo.ApplicationUserGroups", "AppUser_Id");
            DropColumn("dbo.ApplicationUserGroups", "ApplicationGroup_ID");
            DropColumn("dbo.ApplicationRoleGroups", "AppRole_Id");
            DropColumn("dbo.ApplicationRoleGroups", "ApplicationGroup_ID");
            CreateIndex("dbo.ApplicationUserGroups", "GroupId");
            CreateIndex("dbo.ApplicationUserGroups", "UserId");
            CreateIndex("dbo.ApplicationRoleGroups", "RoleId");
            CreateIndex("dbo.ApplicationRoleGroups", "GroupId");
            AddForeignKey("dbo.ApplicationUserGroups", "UserId", "dbo.AppUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ApplicationUserGroups", "GroupId", "dbo.ApplicationGroups", "ID", cascadeDelete: true);
            AddForeignKey("dbo.ApplicationRoleGroups", "RoleId", "dbo.AppRoles", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ApplicationRoleGroups", "GroupId", "dbo.ApplicationGroups", "ID", cascadeDelete: true);
        }
    }
}
