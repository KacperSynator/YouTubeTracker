namespace YouTubeTracker.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DBPlaylists",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false),
                })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.DBVideos",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DBPlaylistID = c.Int(nullable: false),
                        YTID = c.String(nullable: false),
                        Duration = c.Int(nullable: false),
                        Title = c.String(),
                        EmbedHTML = c.String(),
                        ThumbnailURL = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.DBPlaylists", t => t.DBPlaylistID, cascadeDelete: true)
                .Index(t => t.DBPlaylistID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DBVideos", "DBPlaylistID", "dbo.DBPlaylists");
            DropIndex("dbo.DBVideos", new[] { "DBPlaylistID" });
            DropTable("dbo.DBVideos");
            DropTable("dbo.DBPlaylists");
        }
    }
}
