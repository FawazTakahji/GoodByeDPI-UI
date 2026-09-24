using System;
using Avalonia.Threading;
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
        Dispatch(() =>
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
        });
    }

    public IToastHandle ShowAction(
        string? title,
        string? message = null,
        Action? onBodyClick = null,
        params ToastAction[] actions)
    {
        IToastHandle handle = null!;
        Dispatch(() =>
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
            handle = new ToastHandle(toast, () => _manager.Dismiss(toast));
        });
        return handle;
    }

    public IToastHandle ShowLoading(string? title, string? message = null, TimeSpan? duration = null)
    {
        IToastHandle handle = null!;
        Dispatch(() =>
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
            handle = new ToastHandle(toast, () => _manager.Dismiss(toast));
        });
        return handle;
    }

    private static void Dispatch(Action action)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            action();
        }
        else
        {
            Dispatcher.UIThread.Invoke(action);
        }
    }
}