using Serilog.Core;
using Serilog.Events;
using Serilog;
using User.Api.ConfigurationOptions.Elastic;
using User.Api.ConfigurationOptions.Logging;
using Destructurama;
using User.Infrastructure.ConfigurationOptions.App;

namespace User.Api
{
    public class Logging
    {
        public static ILogger GetLogger(IConfiguration configuration, IWebHostEnvironment environment)
        {
            var loggingOptions = configuration.GetSection("Logging").Get<LoggingOption>();
            var appConfigurations = configuration.GetSection("AppConfigurations").Get<AppOption>();
            var elasticUri = configuration.GetSection("Elasticsearch").Get<ElasticSearchOption>();
            var logIndexPattern = $"Ecoeden.User.API-{environment.EnvironmentName}";

            Enum.TryParse(loggingOptions.Console.LogLevel, false, out LogEventLevel minimumEventLevel);

            var loggerConfigurations = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(new LoggingLevelSwitch(minimumEventLevel))
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty(nameof(Environment.MachineName), Environment.MachineName)
                .Enrich.WithProperty(nameof(appConfigurations.ApplicationIdentifier), appConfigurations.ApplicationIdentifier)
                .Enrich.WithProperty(nameof(appConfigurations.ApplicationEnvironment), appConfigurations.ApplicationEnvironment);

            if (loggingOptions.Console.Enabled)
            {
                loggerConfigurations.WriteTo.Console(minimumEventLevel, loggingOptions.LogOutputTemplate);
            }
            if (loggingOptions.Elastic.Enabled)
            {
                loggerConfigurations.WriteTo.Elasticsearch(elasticUri.Uri, logIndexPattern);
            }

            return loggerConfigurations
                   .Destructure
                   .UsingAttributes()
                   .CreateLogger();
        }
    }
}
