namespace Claims.IntegrationTests.Infrastructure.Constants;

public static class TestConstants
{
    public static class Routes
    {
        public const string Claims = "/claims";
        public const string Covers = "/covers";
        public const string ComputePremium = "/formulas/cover/premium";
    }

    public static class Validation
    {
        public const decimal MaxClaimDamageCost = 100000m;
        public const int MaxCoverInsurancePeriodYears = 1;
    }
}
