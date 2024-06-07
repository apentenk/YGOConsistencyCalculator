using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YGOConsistencyCalculator.Models
{
    public class Card
    {
        [Key]
        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string CardFrame { get; set; }
        public string CardType { get; set; }
        public int Level { get; set; }
        public int Scale { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public string Attribute { get; set; }
    }

    public class CardDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string CardFrame { get; set; }
        public string CardType { get; set; }
        public int Level { get; set; }
        public int Scale { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public string Attribute { get; set; }
    }
}