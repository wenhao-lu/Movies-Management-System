namespace Movies_Management_System.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dataset : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        ClientID = c.Int(nullable: false, identity: true),
                        ClientName = c.String(),
                        ClientLocation = c.String(),
                    })
                .PrimaryKey(t => t.ClientID);
            
            CreateTable(
                "dbo.Movies",
                c => new
                    {
                        MovieID = c.Int(nullable: false, identity: true),
                        MovieTitle = c.String(),
                        PubYear = c.Int(nullable: false),
                        Director = c.String(),
                        Ratings = c.Int(nullable: false),
                        GenreID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MovieID)
                .ForeignKey("dbo.Genres", t => t.GenreID, cascadeDelete: true)
                .Index(t => t.GenreID);
            
            CreateTable(
                "dbo.Genres",
                c => new
                    {
                        GenreID = c.Int(nullable: false, identity: true),
                        GenreName = c.String(),
                    })
                .PrimaryKey(t => t.GenreID);
            
            CreateTable(
                "dbo.MovieClients",
                c => new
                    {
                        Movie_MovieID = c.Int(nullable: false),
                        Client_ClientID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Movie_MovieID, t.Client_ClientID })
                .ForeignKey("dbo.Movies", t => t.Movie_MovieID, cascadeDelete: true)
                .ForeignKey("dbo.Clients", t => t.Client_ClientID, cascadeDelete: true)
                .Index(t => t.Movie_MovieID)
                .Index(t => t.Client_ClientID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Movies", "GenreID", "dbo.Genres");
            DropForeignKey("dbo.MovieClients", "Client_ClientID", "dbo.Clients");
            DropForeignKey("dbo.MovieClients", "Movie_MovieID", "dbo.Movies");
            DropIndex("dbo.MovieClients", new[] { "Client_ClientID" });
            DropIndex("dbo.MovieClients", new[] { "Movie_MovieID" });
            DropIndex("dbo.Movies", new[] { "GenreID" });
            DropTable("dbo.MovieClients");
            DropTable("dbo.Genres");
            DropTable("dbo.Movies");
            DropTable("dbo.Clients");
        }
    }
}
