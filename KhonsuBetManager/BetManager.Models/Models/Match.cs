namespace BetManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Match
    {
        public int Id { get; set; }
        [Required]
        public string Team1 { get; set; }
        [Required]
        public string Team2 { get; set; }
        public string League { get; set; }
        public decimal Coef1 { get; set; }
        public decimal CoefX { get; set; }
        public decimal Coef2 { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public DateTime End { get; set; }
        public int Result { get; set; }
        [Required]
        public string Score { get; set; }
        public virtual ICollection<Bet> Bets { get; set; }
    }
}
