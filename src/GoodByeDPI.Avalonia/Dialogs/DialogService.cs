using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using GoodByeDPI.Core.Dialogs;
using SukiUI.Dialogs;

namespace GoodByeDPI.Avalonia.Dialogs;

public class DialogService : IDialogService
{
    private readonly ISukiDialogManager _manager;
    private readonly Queue<QueuedDialog> _queue = new();
    private bool _showing;

    public DialogService(ISukiDialogManager manager)
    {
        _manager = manager;
    }

    public void Show(
        string? title,
        string? message,
        DialogButton buttons = DialogButton.OK,
        NotificationKind kind = NotificationKind.None,
        bool dismissible = true)
    {
        _queue.Enqueue(new QueuedDialog
        {
            Title = title,
            Message = message,
            Buttons = buttons,
            Kind = kind,
            Dismissible = dismissible,
        });
        TryDequeue();
    }

    public Task<DialogResult> ShowModal(
        string? title,
        string? message,
        DialogButton buttons = DialogButton.OK,
        NotificationKind kind = NotificationKind.None,
        bool dismissible = true)
    {
        TaskCompletionSource<DialogResult> completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        _queue.Enqueue(new QueuedDialog
        {
            Title = title,
            Message = message,
            Buttons = buttons,
            Kind = kind,
            Dismissible = dismissible,
            Completion = completion,
        });
        TryDequeue();
        return completion.Task;
    }

    private void TryDequeue()
    {
        if (_showing || _queue.Count == 0)
        {
            return;
        }

        QueuedDialog dialog = _queue.Dequeue();

        SukiDialogBuilder builder = _manager.CreateDialog()
            .WithTitle(dialog.Title ?? string.Empty)
            .WithContent(dialog.Message ?? string.Empty)
            .OnDismissed(_ =>
            {
                dialog.Completion?.TrySetResult(DialogResult.None);
                Dispatcher.UIThread.Post(() =>
                {
                    _showing = false;
                    TryDequeue();
                });
            });

        if (dialog.Kind != NotificationKind.None)
        {
            builder = builder.OfType(MapKind(dialog.Kind));
        }

        if (dialog.Dismissible)
        {
            builder = builder.Dismiss().ByClickingBackground();
        }

        AddButtons(dialog, builder);

        if (!builder.TryShow())
        {
            dialog.Completion?.TrySetResult(DialogResult.None);
            return;
        }

        _showing = true;
    }

    private static void AddButtons(QueuedDialog dialog, SukiDialogBuilder builder)
    {
        foreach (var (label, result) in Buttons(dialog.Buttons))
        {
            builder = builder.WithActionButton(label, _ => dialog.Completion?.TrySetResult(result), dismissOnClick: true, "Flat");
        }
    }

    private static IEnumerable<(object Label, DialogResult Result)> Buttons(DialogButton buttons)
    {
        switch (buttons)
        {
            case DialogButton.OK:
                yield return ("OK", DialogResult.OK);
                break;
            case DialogButton.OKCancel:
                yield return ("OK", DialogResult.OK);
                yield return ("Cancel", DialogResult.Cancel);
                break;
            case DialogButton.YesNo:
                yield return ("Yes", DialogResult.Yes);
                yield return ("No", DialogResult.No);
                break;
            case DialogButton.YesNoCancel:
                yield return ("Yes", DialogResult.Yes);
                yield return ("No", DialogResult.No);
                yield return ("Cancel", DialogResult.Cancel);
                break;
        }
    }

    private static NotificationType MapKind(NotificationKind kind)
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