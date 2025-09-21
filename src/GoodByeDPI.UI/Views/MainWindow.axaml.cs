using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace GoodByeDPI.UI.Views;

public partial class MainWindow : Window
{
    private bool ShouldArrange { get; set; }

    public MainWindow(bool shouldArrange = false)
    {
        InitializeComponent();
        ShouldArrange = shouldArrange;
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (e.CloseReason == WindowCloseReason.WindowClosing)
        {
            e.Cancel = true;
            Hide();
        }

        base.OnClosing(e);
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        if (!ShouldArrange || Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            return;
        }

        ShouldArrange = false;
        foreach (Window window in desktop.Windows)
        {
            if (window == this)
            {
                continue;
            }

            window.Activate();
        }
    }

#if DEBUG
    public MainWindow()
    {
        InitializeComponent();
    }
#endif
}