namespace Application
{
    public class JwtSettings
    {
        public string Secret { get; set; }
        public int TokrnExpiryTimeInMinutes { get; set; }
        public int RefreshTokrnExpiryTimeInDays { get; set; }

    }
}
