using Avalonia.Controls;
using SukiUI.Controls;

namespace GoodByeDPI.Avalonia;

public partial class MainWindow : SukiWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }
}