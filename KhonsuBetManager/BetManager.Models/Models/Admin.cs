namespace BetManager.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public int Owner { get; set; }
    }
}
