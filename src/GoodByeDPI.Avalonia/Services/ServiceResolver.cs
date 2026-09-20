using System;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace GoodByeDPI.Avalonia.Services;

public class ServiceResolver : MarkupExtension
{
    public required Type Type { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Ioc.Default.GetRequiredService(Type);
    }
}