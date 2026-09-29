using GoodByeDPI.Avalonia.Tray;
using GoodByeDPI.Core.Dialogs;
using GoodByeDPI.Core.Services;
using H.NotifyIcon.Core;

namespace GoodByeDPI.Avalonia.Services;

public class TrayNotifyService : ITrayNotifyService
{
    public void Show(string? title, string? message, NotificationKind kind = NotificationKind.None)
    {
        TrayManager.ShowNotification(title ?? string.Empty, message ?? string.Empty, Map(kind));
    }

    private static NotificationIcon Map(NotificationKind kind) => kind switch
    {
        NotificationKind.Error => NotificationIcon.Error,
        NotificationKind.Warning => NotificationIcon.Warning,
        _ => NotificationIcon.Info,
    };
}