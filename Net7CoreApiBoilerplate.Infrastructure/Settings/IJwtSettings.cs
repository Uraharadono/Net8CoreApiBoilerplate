namespace Net7CoreApiBoilerplate.Infrastructure.Settings
{
    public interface IJwtSettings
    {
        public string Key { get; }
        public string Issuer { get; }
        public string Audience { get; }
        public double DurationInMinutes { get; }
        public int RememberMeDurationInHours { get; }
    }
}
