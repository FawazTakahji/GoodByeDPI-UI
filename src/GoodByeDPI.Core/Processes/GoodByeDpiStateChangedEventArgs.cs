namespace GoodByeDPI.Core.Processes;

public class GoodByeDpiStateChangedEventArgs(bool isRunning, bool isBusy) : EventArgs
{
    public bool IsRunning { get; } = isRunning;
    public bool IsBusy { get; } = isBusy;
}
