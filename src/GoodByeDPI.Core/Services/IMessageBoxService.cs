namespace GoodByeDPI.Core.Services;

public interface IMessageBoxService
{
    public void Show(string title, string message, Button button = Button.Ok, Icon icon = Icon.None);

    public Task<ButtonResult> ShowAsync(string title, string message, Button button = Button.Ok, Icon icon = Icon.None);
}

public enum Button
{
    Ok,
    YesNo,
    OkCancel,
    OkAbort,
    YesNoCancel,
    YesNoAbort
}

public enum Icon
{
    None,
    Error,
    Info,
    Question,
    Success,
    Warning
}

public enum ButtonResult
{
    Ok,
    Yes,
    No,
    Abort,
    Cancel,
    None
}