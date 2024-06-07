﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YGOConsistencyCalculator.Models
{
    public class ComboPiece
    {
        [Key]
        public int ComboPieceId { get; set; }

        public int CardNumber { get; set; }

        [ForeignKey("Deck")]
        public int DeckId { get; set; }
        public virtual Deck Deck { get; set; }

        public string Category { get; set; }
    }

    public class ComboPieceDto
    {
        [Key]
        public int ComboPieceId { get; set; }

        public int CardNumber { get; set; }

        [ForeignKey("Deck")]
        public int DeckId { get; set; }
        public virtual Deck Deck { get; set; }

        public string Category { get; set; }
    }
}