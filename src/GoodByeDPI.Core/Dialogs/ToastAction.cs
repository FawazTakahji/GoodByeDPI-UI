namespace GoodByeDPI.Core.Dialogs;

public record ToastAction(string Label, Action? OnClick = null, bool DismissOnClick = true);