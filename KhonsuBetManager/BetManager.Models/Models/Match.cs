namespace BetManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Match
    {
        public Match()
        {
            this.MatchesBets = new HashSet<MatchesBets>();
        }
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

        //1 for 1; 2 for 2; 3 for X;
        public int Result { get; set; }
        public string Score { get; set; }
        public virtual ICollection<MatchesBets> MatchesBets { get; set; }
    }
}
