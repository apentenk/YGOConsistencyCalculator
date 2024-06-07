namespace YGOConsistencyCalculator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedecktable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Decks", "DeckName", c => c.String());
            AddColumn("dbo.Decks", "DeckList", c => c.String());
            AddColumn("dbo.Decks", "StarterChance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Decks", "OneCardChance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Decks", "TwoCardChance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Decks", "ExtenderChance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Decks", "BrickChance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Decks", "EngineReqChance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Decks", "DeckPath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Decks", "DeckPath", c => c.String());
            DropColumn("dbo.Decks", "EngineReqChance");
            DropColumn("dbo.Decks", "BrickChance");
            DropColumn("dbo.Decks", "ExtenderChance");
            DropColumn("dbo.Decks", "TwoCardChance");
            DropColumn("dbo.Decks", "OneCardChance");
            DropColumn("dbo.Decks", "StarterChance");
            DropColumn("dbo.Decks", "DeckList");
            DropColumn("dbo.Decks", "DeckName");
        }
    }
}
