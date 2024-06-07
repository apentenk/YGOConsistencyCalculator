using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YGOConsistencyCalculator.Models
{
    public class Deck
    {
        [Key]
        public int DeckId { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        public string DeckName { get; set; }
        public string DeckList { get; set; }
        public decimal StarterChance { get; set; }
        public decimal OneCardChance { get; set; }
        public decimal TwoCardChance { get; set; }
        public decimal ExtenderChance { get; set;}
        public decimal BrickChance { get; set; }
        public decimal EngineReqChance { get; set;}
    }

    public class DeckDto
    {
        [Key]
        public int DeckId { get; set; }
        public string UserId { get; set; }
        public string DeckName { get; set; }
        public string DeckList { get; set; }
        public decimal StarterChance { get; set; }
        public decimal OneCardChance { get; set; }
        public decimal TwoCardChance { get; set; }
        public decimal ExtenderChance { get; set; }
        public decimal BrickChance { get; set; }
        public decimal EngineReqChance { get; set; }
    }
}