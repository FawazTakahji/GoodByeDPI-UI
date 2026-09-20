namespace GoodByeDPI.Core.Dialogs;

public interface IToastHandle
{
    public void Update(string? title = null, string? message = null);

    public void Dismiss();
}