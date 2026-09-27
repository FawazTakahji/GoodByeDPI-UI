namespace GoodByeDPI.Core.Services;

public interface IWindowService
{
    public bool IsVisible { get; }

    public void Show();

    public void Hide();
}