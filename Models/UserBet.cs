namespace BetMe.Models
{
    public class UserBet
    {
        public long Id { get; set; }
        public long BetId { get; set; }
        public long UserId { get; set; }
        public long OutcomeId { get; set; }
        public bool HasVoted { get; set; } = false;
    }
}