using System.Threading.Tasks;
using GoodByeDPI.Core.Dialogs;

namespace GoodByeDPI.Avalonia.Dialogs;

public class QueuedDialog
{
    public string? Title { get; init; }
    public string? Message { get; init; }
    public DialogButton Buttons { get; init; }
    public NotificationKind Kind { get; init; }
    public bool Dismissible { get; init; }
    public TaskCompletionSource<DialogResult>? Completion { get; init; }
}