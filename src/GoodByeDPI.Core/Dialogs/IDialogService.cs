namespace GoodByeDPI.Core.Dialogs;

public interface IDialogService
{
    public void Show(
        string? title,
        string? message,
        DialogButton buttons = DialogButton.OK,
        NotificationKind kind = NotificationKind.None,
        bool dismissible = true);

    public Task<DialogResult> ShowModal(
        string? title,
        string? message,
        DialogButton buttons = DialogButton.OK,
        NotificationKind kind = NotificationKind.None,
        bool dismissible = true);
}