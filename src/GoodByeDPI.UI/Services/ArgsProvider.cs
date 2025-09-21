using GoodByeDPI.Core.Services;

namespace GoodByeDPI.UI.Services;

public class ArgsProvider(string[]? args) : IArgsProvider
{
    public string[] Args { get; } = args ?? [];
}