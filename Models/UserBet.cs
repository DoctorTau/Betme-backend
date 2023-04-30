namespace BetMe.Models
{
    public class UserBet
    {
        public int Id { get; set; }
        public int BetId { get; set; }
        public int UserId { get; set; }
        public int OutcomeId { get; set; }
    }
}