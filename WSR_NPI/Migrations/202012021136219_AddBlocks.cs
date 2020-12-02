namespace WSR_NPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBlocks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Blocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Index = c.Int(nullable: false),
                        IndexUser = c.Int(nullable: false),
                        PreviousHash = c.String(),
                        TimeStamp = c.Long(nullable: false),
                        Data = c.String(),
                        Hash = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Blocks");
        }
    }
}
