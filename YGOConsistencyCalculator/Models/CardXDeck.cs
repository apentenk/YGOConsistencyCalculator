using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YGOConsistencyCalculator.Models
{
    public class CardXDeck
    {
        [Key]
        public int CardxDeckId { get; set; }

        [ForeignKey("Card")]
        public int CardId { get; set; }
        public virtual Card Card { get; set; }

        [ForeignKey("Deck")]
        public int DeckId { get; set; }
        public virtual Deck Deck { get; set; }

        public int Copies { get; set; }

        public string ComboPiece { get; set; }
    }

    public class CardXDeckDto
    {
        public int CardxDeckId { get; set; }

        public int CardId { get; set; }

        public int DeckId { get; set; }

        public int Copies { get; set; }

        public string ComboPiece { get; set; }
    }
}