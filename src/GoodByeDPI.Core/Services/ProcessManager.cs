using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GoodByeDPI.Core.Services;

public partial class ProcessManager : ObservableObject
{
    [ObservableProperty]
    private bool _isRunning;
    private Process? _process;

    public void Start(string path, string arguments)
    {
        if (_process is not null && !_process.HasExited)
        {
            return;
        }

        if (!File.Exists(path))
        {
            throw new FileNotFoundException();
        }

        foreach (Process process in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(path)))
        {
            if (process.MainModule != null && Path.GetFullPath(process.MainModule.FileName) != Path.GetFullPath(path))
            {
                continue;
            }

            process.Kill();
            break;
        }

        _process = new()
        {
            StartInfo = new()
            {
                FileName = path,
                WorkingDirectory = Path.GetDirectoryName(path),
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true
        };

        _process.Exited += OnProcessExited;
        _process.Start();
        IsRunning = true;
    }

    public void Stop()
    {
        _process?.Kill();
    }

    private void OnProcessExited(object? sender, EventArgs e)
    {
        if (_process is null)
        {
            return;
        }

        _process.Exited -= OnProcessExited;
        _process.Dispose();
        _process = null;
        IsRunning = false;
    }
}