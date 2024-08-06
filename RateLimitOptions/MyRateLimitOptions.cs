using System.Security.Principal;

namespace APICatalogo.RateLimitOptions
{
    public class MyRateLimitOptions
    {
        public const string MyRateLimit = "MyRateLimit";
        public int PermitLimit { get; set; } = 6;
        public int Window { get; set; } = 9;
        public int ReplenishmentPeriod { get; set; } = 1;
        public int QueueLimit { get; set; } = 2;
        public int SegmentsPerWindow { get; set; } = 4;
        public int TokenLimit { get; set; } = 8;
        public int TokenLimit2 { get; set; } = 12;
        public int TokensPerPeriod { get; set; } = 4;
        public bool AutoReplenishment { get; set; } = false;

    }
}
