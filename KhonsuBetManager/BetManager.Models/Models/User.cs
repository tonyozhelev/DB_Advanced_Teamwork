namespace BetManager.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        public User()
        {
            this.Bets = new HashSet<Bet>();
        }
        public int Id { get; set; }

        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        public decimal Balance { get; set; }
        public int IsAdmin { get; set; }
        public DateTime? LastLogin { get; set; }
        public virtual ICollection<Bet> Bets { get; set; }
    }
}
