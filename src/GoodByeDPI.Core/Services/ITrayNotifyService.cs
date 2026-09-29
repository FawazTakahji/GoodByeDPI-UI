using GoodByeDPI.Core.Dialogs;

namespace GoodByeDPI.Core.Services;

public interface ITrayNotifyService
{
    public void Show(string? title, string? message, NotificationKind kind = NotificationKind.None);
}