using GoodByeDPI.Avalonia.Dialogs;
using GoodByeDPI.Core.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using SukiUI.Dialogs;

namespace GoodByeDPI.Avalonia.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUiServices(this IServiceCollection services)
    {
        services.AddSingleton<ISukiDialogManager, SukiDialogManager>()
            .AddSingleton<IDialogService, DialogService>();

        return services;
    }
}