namespace YGOConsistencyCalculator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class combopiecetable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CardXDecks", "CardId", "dbo.Cards");
            DropForeignKey("dbo.CardXDecks", "DeckId", "dbo.Decks");
            DropIndex("dbo.CardXDecks", new[] { "CardId" });
            DropIndex("dbo.CardXDecks", new[] { "DeckId" });
            CreateTable(
                "dbo.ComboPieces",
                c => new
                    {
                        ComboPieceId = c.Int(nullable: false, identity: true),
                        CardNumber = c.Int(nullable: false),
                        DeckId = c.Int(nullable: false),
                        Category = c.String(),
                    })
                .PrimaryKey(t => t.ComboPieceId)
                .ForeignKey("dbo.Decks", t => t.DeckId, cascadeDelete: true)
                .Index(t => t.DeckId);
            
            DropTable("dbo.CardXDecks");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CardXDecks",
                c => new
                    {
                        CardxDeckId = c.Int(nullable: false, identity: true),
                        CardId = c.Int(nullable: false),
                        DeckId = c.Int(nullable: false),
                        Copies = c.Int(nullable: false),
                        ComboPiece = c.String(),
                    })
                .PrimaryKey(t => t.CardxDeckId);
            
            DropForeignKey("dbo.ComboPieces", "DeckId", "dbo.Decks");
            DropIndex("dbo.ComboPieces", new[] { "DeckId" });
            DropTable("dbo.ComboPieces");
            CreateIndex("dbo.CardXDecks", "DeckId");
            CreateIndex("dbo.CardXDecks", "CardId");
            AddForeignKey("dbo.CardXDecks", "DeckId", "dbo.Decks", "DeckId", cascadeDelete: true);
            AddForeignKey("dbo.CardXDecks", "CardId", "dbo.Cards", "Id", cascadeDelete: true);
        }
    }
}
