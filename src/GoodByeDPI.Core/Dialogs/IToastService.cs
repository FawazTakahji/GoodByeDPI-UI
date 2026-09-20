namespace GoodByeDPI.Core.Dialogs;

public interface IToastService
{
    public void Show(
        string? title,
        string? message = null,
        NotificationKind kind = NotificationKind.Information,
        TimeSpan? duration = null,
        bool dismissibleByClick = true);

    public IToastHandle ShowAction(
        string? title,
        string? message = null,
        Action? onBodyClick = null,
        params ToastAction[] actions);

    public IToastHandle ShowLoading(string? title, string? message = null, TimeSpan? duration = null);
}