using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

namespace GoodByeDPI.Core.Navigation;

public partial class NavigationService : ObservableObject
{
    private readonly IServiceProvider _services;
    private readonly Stack<INavigable> _backStack = new();

    [ObservableProperty]
    public partial INavigable? Current { get; private set; }

    public bool CanNavigateBack => _backStack.Count > 0;

    public NavigationService(IServiceProvider services)
    {
        _services = services;
    }

    public void NavigateTo<TNavigable>() where TNavigable : INavigable
    {
        SetCurrent(_services.GetRequiredService<TNavigable>());
    }

    public void NavigateTo(Type navigableType)
    {
        ArgumentNullException.ThrowIfNull(navigableType);

        if (!typeof(INavigable).IsAssignableFrom(navigableType))
        {
            throw new ArgumentException($"{navigableType} does not implement {nameof(INavigable)}.", nameof(navigableType));
        }

        SetCurrent((INavigable)_services.GetRequiredService(navigableType));
    }

    [RelayCommand(CanExecute = nameof(CanNavigateBack))]
    public void NavigateBack()
    {
        if (_backStack.Count < 1)
        {
            return;
        }

        Current = _backStack.Pop();
        NavigateBackCommand.NotifyCanExecuteChanged();
    }

    private void SetCurrent(INavigable navigable)
    {
        if (ReferenceEquals(navigable, Current))
        {
            return;
        }

        if (Current is not null)
        {
            _backStack.Push(Current);
            NavigateBackCommand.NotifyCanExecuteChanged();
        }

        Current = navigable;
    }
}