using NLog;
using Essentials.Redis.Sample;
using Essentials.Redis.Extensions;
using Essentials.Configuration.Helpers;
using Essentials.Configuration.Extensions;

var builder = WebApplication.CreateBuilder(args);
LogManager.Setup().LoadConfigurationFromFile(LoggingHelpers.GetNLogConfigPath(builder.Environment.EnvironmentName));

var logger = LogManager.GetCurrentClassLogger();

try
{
    var applicationName = EnvironmentHelpers.GetApplicationName();
    
    logger.Info("Сервис {@appName} запускается...", applicationName);
    
    var host = builder
        .ConfigureDefault(
            configureServicesAction: (context, services) =>
            {
                services
                    .AddHostedService<Service>()
                    .ConfigureRedis(context.Configuration);
            })
        .Build();
    
    logger.Info("Сервис {@appName} собран. Старт сервиса...", applicationName);
    
    await host.RunAsync();
}
catch (Exception ex)
{
    logger.Error(ex.Message);
    Thread.Sleep(1000);
}