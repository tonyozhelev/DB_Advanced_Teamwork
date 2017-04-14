namespace BetManager.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Accounting
    {
        [Key]
        public int OrderId { get; set; }
        [Required]
        public string Transaction { get; set; }
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public decimal Ammount { get; set; }
        public string Notes { get; set; }
        [Required]
        public DateTime DateOfTransaction { get; set; }
    }
}




