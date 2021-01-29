namespace GeneralStoreAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addednullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Transactions", "CustomerId", "dbo.Customers");
            DropIndex("dbo.Transactions", new[] { "CustomerId" });
            AlterColumn("dbo.Transactions", "CustomerId", c => c.Int());
            CreateIndex("dbo.Transactions", "CustomerId");
            AddForeignKey("dbo.Transactions", "CustomerId", "dbo.Customers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "CustomerId", "dbo.Customers");
            DropIndex("dbo.Transactions", new[] { "CustomerId" });
            AlterColumn("dbo.Transactions", "CustomerId", c => c.Int(nullable: false));
            CreateIndex("dbo.Transactions", "CustomerId");
            AddForeignKey("dbo.Transactions", "CustomerId", "dbo.Customers", "Id", cascadeDelete: true);
        }
    }
}
