using GoodByeDPI.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace GoodByeDPI.Core.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<MainViewModel>();

        return services;
    }
}