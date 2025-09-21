using System;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using GoodByeDPI.Core.Services;
using MsBox.Avalonia;

namespace GoodByeDPI.UI.Services;

public class MessageBoxService(IClassicDesktopStyleApplicationLifetime lifetime) : IMessageBoxService
{
    public void Show(string title, string message, Button button = Button.Ok, Icon icon = Icon.None)
    {
        if (lifetime.MainWindow is null)
        {
            MessageBoxManager.GetMessageBoxStandard(title, message, ConvertButton(button), ConvertIcon(icon))
                .ShowWindowAsync();
        }
        else
        {
            MessageBoxManager.GetMessageBoxStandard(title, message, ConvertButton(button), ConvertIcon(icon))
                .ShowWindowDialogAsync(lifetime.MainWindow);
        }
    }

    public async Task<ButtonResult> ShowAsync(string title, string message, Button button = Button.Ok, Icon icon = Icon.None)
    {
        MsBox.Avalonia.Enums.ButtonResult result;
        if (lifetime.MainWindow is null)
        {
            result = await MessageBoxManager.GetMessageBoxStandard(
                    title,
                    message,
                    ConvertButton(button),
                    ConvertIcon(icon))
                .ShowWindowAsync();
        }
        else
        {
            result = await MessageBoxManager.GetMessageBoxStandard(
                    title,
                    message,
                    ConvertButton(button),
                    ConvertIcon(icon))
                .ShowWindowDialogAsync(lifetime.MainWindow);
        }

        return ConvertButtonResult(result);
    }

    private static ButtonResult ConvertButtonResult(MsBox.Avalonia.Enums.ButtonResult buttonResult)
    {
        return buttonResult switch
        {
            MsBox.Avalonia.Enums.ButtonResult.Ok => ButtonResult.Ok,
            MsBox.Avalonia.Enums.ButtonResult.Yes => ButtonResult.Yes,
            MsBox.Avalonia.Enums.ButtonResult.No => ButtonResult.No,
            MsBox.Avalonia.Enums.ButtonResult.Abort => ButtonResult.Abort,
            MsBox.Avalonia.Enums.ButtonResult.Cancel => ButtonResult.Cancel,
            MsBox.Avalonia.Enums.ButtonResult.None => ButtonResult.None,
            _ => throw new ArgumentOutOfRangeException(nameof(buttonResult), buttonResult, null)
        };
    }

    private static MsBox.Avalonia.Enums.ButtonEnum ConvertButton(Button button)
    {
        return button switch
        {
            Button.Ok => MsBox.Avalonia.Enums.ButtonEnum.Ok,
            Button.YesNo => MsBox.Avalonia.Enums.ButtonEnum.YesNo,
            Button.OkCancel => MsBox.Avalonia.Enums.ButtonEnum.OkCancel,
            Button.OkAbort => MsBox.Avalonia.Enums.ButtonEnum.OkAbort,
            Button.YesNoCancel => MsBox.Avalonia.Enums.ButtonEnum.YesNoCancel,
            Button.YesNoAbort => MsBox.Avalonia.Enums.ButtonEnum.YesNoAbort,
            _ => throw new ArgumentOutOfRangeException(nameof(button), button, "Enum value not mapped")
        };
    }

    private static MsBox.Avalonia.Enums.Icon ConvertIcon(Icon icon)
    {
        return icon switch
        {
            Icon.None => MsBox.Avalonia.Enums.Icon.None,
            Icon.Error => MsBox.Avalonia.Enums.Icon.Error,
            Icon.Info => MsBox.Avalonia.Enums.Icon.Info,
            Icon.Question => MsBox.Avalonia.Enums.Icon.Question,
            Icon.Success => MsBox.Avalonia.Enums.Icon.Success,
            Icon.Warning => MsBox.Avalonia.Enums.Icon.Warning,
            _ => throw new ArgumentOutOfRangeException(nameof(icon), icon, "Enum value not mapped")
        };
    }
}