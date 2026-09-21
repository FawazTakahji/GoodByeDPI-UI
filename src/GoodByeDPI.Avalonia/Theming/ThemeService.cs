using Avalonia.Media;
using GoodByeDPI.Core.Theming;
using SukiUI;
using SukiUI.Enums;
using SukiUI.Models;

namespace GoodByeDPI.Avalonia.Theming;

public sealed class ThemeService : IThemeService
{
    private static readonly SukiColorTheme Amber = new("Amber", Color.Parse("#FFC107"), Color.Parse("#FFB300"));

    public void SetState(ThemeState state)
    {
        SukiColorTheme theme = state switch
        {
            ThemeState.Downloading => Amber,
            ThemeState.Running => SukiTheme.DefaultColorThemes[SukiColor.Green],
            _ => SukiTheme.DefaultColorThemes[SukiColor.Red],
        };

        SukiTheme.GetInstance().ChangeColorTheme(theme);
    }
}