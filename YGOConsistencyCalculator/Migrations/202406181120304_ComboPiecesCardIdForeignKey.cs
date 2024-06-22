namespace YGOConsistencyCalculator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ComboPiecesCardIdForeignKey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DeckCards", "Deck_DeckId", "dbo.Decks");
            DropForeignKey("dbo.DeckCards", "Card_Id", "dbo.Cards");
            DropIndex("dbo.DeckCards", new[] { "Deck_DeckId" });
            DropIndex("dbo.DeckCards", new[] { "Card_Id" });
            AddColumn("dbo.ComboPieces", "CardId", c => c.Int(nullable: false));
            CreateIndex("dbo.ComboPieces", "CardId");
            AddForeignKey("dbo.ComboPieces", "CardId", "dbo.Cards", "Id", cascadeDelete: true);
            DropColumn("dbo.ComboPieces", "CardNumber");
            DropTable("dbo.DeckCards");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.DeckCards",
                c => new
                    {
                        Deck_DeckId = c.Int(nullable: false),
                        Card_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Deck_DeckId, t.Card_Id });
            
            AddColumn("dbo.ComboPieces", "CardNumber", c => c.Int(nullable: false));
            DropForeignKey("dbo.ComboPieces", "CardId", "dbo.Cards");
            DropIndex("dbo.ComboPieces", new[] { "CardId" });
            DropColumn("dbo.ComboPieces", "CardId");
            CreateIndex("dbo.DeckCards", "Card_Id");
            CreateIndex("dbo.DeckCards", "Deck_DeckId");
            AddForeignKey("dbo.DeckCards", "Card_Id", "dbo.Cards", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DeckCards", "Deck_DeckId", "dbo.Decks", "DeckId", cascadeDelete: true);
        }
    }
}
