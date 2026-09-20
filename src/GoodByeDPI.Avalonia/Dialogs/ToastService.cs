using System;
using GoodByeDPI.Core.Dialogs;
using SukiUI.Toasts;

namespace GoodByeDPI.Avalonia.Dialogs;

public class ToastService : IToastService
{
    private static readonly TimeSpan DefaultDuration = TimeSpan.FromSeconds(3);
    private readonly ISukiToastManager _manager;

    public ToastService(ISukiToastManager manager)
    {
        _manager = manager;
    }

    public void Show(
        string? title,
        string? message = null,
        NotificationKind kind = NotificationKind.Information,
        TimeSpan? duration = null,
        bool dismissibleByClick = true)
    {
        SukiToastBuilder builder = _manager.CreateToast()
            .WithTitle(title ?? string.Empty)
            .WithContent(message ?? string.Empty)
            .OfType(NotificationTypeMapper.Map(kind))
            .Dismiss().After(duration ?? DefaultDuration);

        if (dismissibleByClick)
        {
            builder = builder.Dismiss().ByClicking();
        }

        builder.Queue();
    }

    public IToastHandle ShowAction(
        string? title,
        string? message = null,
        Action? onBodyClick = null,
        params ToastAction[] actions)
    {
        SukiToastBuilder builder = _manager.CreateToast()
            .WithTitle(title ?? string.Empty)
            .WithContent(message ?? string.Empty);

        if (onBodyClick is not null)
        {
            builder = builder.OnClicked(_ => onBodyClick());
        }

        foreach (ToastAction action in actions)
        {
            builder = builder.WithActionButton(action.Label, _ => action.OnClick?.Invoke(), action.DismissOnClick);
        }

        ISukiToast toast = builder.Queue();
        return new ToastHandle(toast, () => _manager.Dismiss(toast));
    }

    public IToastHandle ShowLoading(string? title, string? message = null, TimeSpan? duration = null)
    {
        SukiToastBuilder builder = _manager.CreateToast()
            .WithTitle(title ?? string.Empty)
            .WithContent(message ?? string.Empty)
            .WithLoadingState(true);

        if (duration is not null)
        {
            builder = builder.Dismiss().After(duration.Value);
        }

        ISukiToast toast = builder.Queue();
        return new ToastHandle(toast, () => _manager.Dismiss(toast));
    }
}