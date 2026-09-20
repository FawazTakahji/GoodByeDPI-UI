using Avalonia.Controls.Notifications;
using GoodByeDPI.Core.Dialogs;

namespace GoodByeDPI.Avalonia.Dialogs;

public static class NotificationTypeMapper
{
    public static NotificationType Map(NotificationKind kind)
    {
        return kind switch
        {
            NotificationKind.Information => NotificationType.Information,
            NotificationKind.Success => NotificationType.Success,
            NotificationKind.Warning => NotificationType.Warning,
            NotificationKind.Error => NotificationType.Error,
            _ => NotificationType.Information,
        };
    }
}