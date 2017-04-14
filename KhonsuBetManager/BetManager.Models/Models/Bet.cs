namespace BetManager.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Bet
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal Coef { get; set; }
        public decimal Ammount { get; set; }
        [Required]
        public char Win { get; set; }
        public virtual ICollection<Match> Matches { get; set; }
    }
}
