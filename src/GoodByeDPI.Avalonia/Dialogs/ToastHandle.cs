using System;
using GoodByeDPI.Core.Dialogs;
using SukiUI.Toasts;

namespace GoodByeDPI.Avalonia.Dialogs;

public class ToastHandle : IToastHandle
{
    private readonly Action _dismiss;
    private readonly ISukiToast _toast;

    public ToastHandle(ISukiToast toast, Action dismiss)
    {
        _toast = toast;
        _dismiss = dismiss;
    }

    public void Update(string? title = null, string? message = null)
    {
        if (title is not null)
        {
            _toast.Title = title;
        }

        if (message is not null)
        {
            _toast.Content = message;
        }
    }

    public void Dismiss() => _dismiss();
}