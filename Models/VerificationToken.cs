using NodaTime;

namespace pwr_msi.Models {
    public class VerificationToken {
        public int VerificationTokenId { get; set; }
        public string Token { get; set; }
        public bool IsUsed { get; set; }
        public int UserId { get; set; }
        public VerificationTokenType TokenType { get; set; }
        public Instant ValidUntil { get; set; }
        public User User { get; set; }

        public bool IsValid => !IsUsed && SystemClock.Instance.GetCurrentInstant() < ValidUntil;
    }
}
