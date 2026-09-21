using Avalonia.Controls;
using Avalonia.Input;
using GoodByeDPI.Core.ViewModels;

namespace GoodByeDPI.Avalonia.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
    }

    private void OnPowerTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is HomeViewModel vm && vm.StartCommand.CanExecute(null))
        {
            vm.StartCommand.Execute(null);
        }
    }
}