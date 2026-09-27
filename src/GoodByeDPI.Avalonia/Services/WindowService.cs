using Avalonia.Controls;
using GoodByeDPI.Core.Services;

namespace GoodByeDPI.Avalonia.Services;

public class WindowService : IWindowService
{
    private static Window _window = null!;

    public static void Attach(Window window)
    {
        _window = window;
    }

    public bool IsVisible => _window.IsVisible;

    public void Show()
    {
        _window.Show();
        _window.Activate();
    }

    public void Hide()
    {
        _window.Hide();
    }
}