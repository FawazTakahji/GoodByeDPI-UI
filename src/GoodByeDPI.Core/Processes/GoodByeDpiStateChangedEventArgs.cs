namespace GoodByeDPI.Core.Processes;

public class GoodByeDpiStateChangedEventArgs(GoodByeDpiPhase phase, bool isBusy) : EventArgs
{
    public GoodByeDpiPhase Phase { get; } = phase;
    public bool IsBusy { get; } = isBusy;
    public bool IsRunning => Phase == GoodByeDpiPhase.Started;
}
