namespace User.Infrastructure.ConfigurationOptions.App
{
    public sealed class AppOption
    {
        public const string OptionName = "AppConfigurations";
        public string ApplicationIdentifier { get; set; }
        public string ApplicationEnvironment { get; set; }
        public int HealthCheckTimeOutInSeconds { get; set; }
    }
}
