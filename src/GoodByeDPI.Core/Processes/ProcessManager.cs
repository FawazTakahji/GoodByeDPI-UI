using Microsoft.Extensions.Logging;
using Microsoft.FSharp.Core;

namespace GoodByeDPI.Core.Processes;

public class ProcessManager
{
    private readonly ILogger<ProcessManager> _logger;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private RunningProcess? _currentProcess;
    private Task? _monitorTask;

    public bool IsRunning => _currentProcess is not null;
    public event EventHandler<bool>? StateChanged;

    public ProcessManager(ILogger<ProcessManager> logger)
    {
        _logger = logger;
    }

    public async Task Start(string exePath)
    {
        if (!File.Exists(exePath))
        {
            throw new FileNotFoundException($"GoodbyeDPI executable not found at '{exePath}'", exePath);
        }

        await _lock.WaitAsync();
        try
        {
            if (_currentProcess is not null)
            {
                _logger.LogWarning("GoodbyeDPI is already running");
                return;
            }

            Command command = new Command(exePath)
                .CurrentDir(Path.GetDirectoryName(exePath) ?? AppDomain.CurrentDomain.BaseDirectory)
                .CreateNoWindow();

            _logger.LogInformation("Starting GoodbyeDPI from '{ExePath}'", exePath);

            FSharpResult<RunningProcess, ProcessError> result = await command.StartAsync();
            if (!result.IsOk)
            {
                throw new InvalidOperationException($"Failed to launch GoodbyeDPI: {result.ErrorValue.Message}");
            }

            RunningProcess process = result.ResultValue;
            _currentProcess = process;
            _monitorTask = MonitorProcessAsync(process);
        }
        finally
        {
            _lock.Release();
        }

        StateChanged?.Invoke(this, true);
    }

    public async Task Stop()
    {
        RunningProcess? process;
        Task? task;

        await _lock.WaitAsync();
        try
        {
            if (_currentProcess is null)
            {
                return;
            }

            _logger.LogInformation("Stopping GoodbyeDPI...");

            process = _currentProcess;
            task = _monitorTask;

            _currentProcess = null;
            _monitorTask = null;

            try
            {
                process.Kill();
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Error killing GoodbyeDPI process");
            }
        }
        finally
        {
            _lock.Release();
        }

        if (task is not null)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error waiting for monitor task during stop");
            }
        }

        if (process is not null)
        {
            try
            {
                await ((IAsyncDisposable)process).DisposeAsync();
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Error disposing RunningProcess");
            }
        }

        _logger.LogInformation("GoodbyeDPI stopped");
        StateChanged?.Invoke(this, false);
    }

    private async Task MonitorProcessAsync(RunningProcess process)
    {
        try
        {
            Outcome outcome = await process.WaitAsync();
            _logger.LogInformation("GoodbyeDPI process terminated with outcome: {Outcome}", outcome);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while waiting for GoodbyeDPI process");
        }

        bool unexpectedExit = false;

        await _lock.WaitAsync(CancellationToken.None);
        try
        {
            if (ReferenceEquals(_currentProcess, process))
            {
                _currentProcess = null;
                _monitorTask = null;
                unexpectedExit = true;
            }
        }
        finally
        {
            _lock.Release();
        }

        if (unexpectedExit)
        {
            try
            {
                await ((IAsyncDisposable)process).DisposeAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while disposing process after unexpected exit");
            }

            StateChanged?.Invoke(this, false);
        }
    }
}