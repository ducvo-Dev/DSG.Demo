namespace DSG.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_data03 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Functions", "ParentId", "dbo.Functions");
            DropForeignKey("dbo.Permissions", "RoleId", "dbo.AppRoles");
            DropForeignKey("dbo.Permissions", "FunctionId", "dbo.Functions");
            DropIndex("dbo.Functions", new[] { "ParentId" });
            DropIndex("dbo.Permissions", new[] { "RoleId" });
            DropIndex("dbo.Permissions", new[] { "FunctionId" });
            DropTable("dbo.Functions");
            DropTable("dbo.Permissions");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Permissions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        RoleId = c.String(maxLength: 128),
                        FunctionId = c.String(maxLength: 50, unicode: false),
                        CanCreate = c.Boolean(nullable: false),
                        CanRead = c.Boolean(nullable: false),
                        CanUpdate = c.Boolean(nullable: false),
                        CanDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Functions",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 50, unicode: false),
                        Name = c.String(nullable: false, maxLength: 50),
                        URL = c.String(nullable: false, maxLength: 256),
                        DisplayOrder = c.Int(nullable: false),
                        ParentId = c.String(maxLength: 50, unicode: false),
                        Status = c.Boolean(nullable: false),
                        IconCss = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateIndex("dbo.Permissions", "FunctionId");
            CreateIndex("dbo.Permissions", "RoleId");
            CreateIndex("dbo.Functions", "ParentId");
            AddForeignKey("dbo.Permissions", "FunctionId", "dbo.Functions", "ID");
            AddForeignKey("dbo.Permissions", "RoleId", "dbo.AppRoles", "Id");
            AddForeignKey("dbo.Functions", "ParentId", "dbo.Functions", "ID");
        }
    }
}
