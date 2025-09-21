using System.Diagnostics.CodeAnalysis;
using GoodByeDPI.Core.Services;
using GoodByeDPI.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace GoodByeDPI.Core;

public static class Singletons
{
    public static IServiceProvider? ServiceProvider { get; set; }

    [MemberNotNull(nameof(ServiceProvider))]
    public static void BuildProvider(IServiceCollection collection)
    {
        collection.AddSingleton<SettingsProvider>();
        collection.AddSingleton<ProcessManager>();
        collection.AddSingleton<MainWindowViewModel>();
        collection.AddSingleton<AppViewModel>();

        ServiceProvider = collection.BuildServiceProvider();
    }
}