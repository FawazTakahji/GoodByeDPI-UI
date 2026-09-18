using GoodByeDPI.Core.Navigation;

namespace GoodByeDPI.Core.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public NavigationService Navigation { get; }

    public MainViewModel(NavigationService navigation)
    {
        Navigation = navigation;
    }
}