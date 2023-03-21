namespace DSG.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AppRoles", "Description", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AppRoles", "Description", c => c.String());
        }
    }
}
