namespace BetManager.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        public decimal Balance { get; set; }
        public int IsAdmin { get; set; }
        public virtual ICollection<Bet> Bets { get; set; }
    }
}
