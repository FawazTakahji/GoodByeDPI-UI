using GoodByeDPI.Core.Github;
using GoodByeDPI.Core.Navigation;
using GoodByeDPI.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ZLogger;

namespace GoodByeDPI.Core.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<NavigationService>()
            .AddSingleton<MainViewModel>()
            .AddSingleton<SettingsViewModel>()
            .AddLogging(logging =>
            {
                logging.AddZLoggerFile(
                    Path.Combine(
                        Path.GetTempPath(),
                        $"GoodByeDPIUI_{DateTimeOffset.Now:yyyyMMdd_HHmmss}.log"),
                    o => o.UseJsonFormatter());
            });

        GithubApiClient.Register(services);

        return services;
    }
}