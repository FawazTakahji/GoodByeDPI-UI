using GoodByeDPI.Avalonia.Dialogs;
using GoodByeDPI.Avalonia.Theming;
using GoodByeDPI.Core.Dialogs;
using GoodByeDPI.Core.Theming;
using Microsoft.Extensions.DependencyInjection;
using SukiUI.Dialogs;
using SukiUI.Toasts;

namespace GoodByeDPI.Avalonia.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUiServices(this IServiceCollection services)
    {
        services.AddSingleton<ISukiDialogManager, SukiDialogManager>()
            .AddSingleton<IDialogService, DialogService>()
            .AddSingleton<ISukiToastManager, SukiToastManager>()
            .AddSingleton<IToastService, ToastService>()
            .AddSingleton<IThemeService, ThemeService>();

        return services;
    }
}