using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using GoodByeDPI.Core.Github;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;

namespace GoodByeDPI.Core.Packages;

public class PackageManager
{
    private string PackagesPath { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "packages");
    private readonly ILogger<PackageManager> _logger;
    private readonly GithubApiClient _client;
    private readonly SemaphoreSlim _downloadLock = new(1, 1);

    public PackageManager(ILogger<PackageManager> logger, GithubApiClient client)
    {
        _logger = logger;
        _client = client;
    }

    public async Task<string> GetOrDownloadLatestAsync(CancellationToken ct = default)
    {
        Release release;
        try
        {
            release = await _client.GetLatestReleaseAsync("ValdikSS", "GoodbyeDPI", includePrereleases: true, ct);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Could not check GitHub for the latest release, falling back to local package");
            string? localExe = GetLatestLocalVersion();
            if (localExe is not null)
            {
                return localExe;
            }

            throw;
        }

        string exePath = Path.Combine(PackagesPath, release.TagName, "goodbyedpi.exe");

        await _downloadLock.WaitAsync(ct);

        try
        {
            if (File.Exists(exePath))
            {
                _logger.LogInformation("Package {Tag} already installed, skipping download", release.TagName);
            }
            else
            {
                await InstallAsync(release, ct);
            }

            DeleteOldVersions(release.TagName);
            return exePath;
        }
        finally
        {
            _downloadLock.Release();
        }
    }

    private async Task InstallAsync(Release release, CancellationToken ct)
    {
        Asset? zip = release.Assets.FirstOrDefault(a => a.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase));
        if (zip is null)
        {
            throw new InvalidOperationException($"Release {release.TagName} has no zip asset.");
        }

        _logger.LogInformation("Downloading {Url}", zip.BrowserDownloadUrl);
        Directory.CreateDirectory(PackagesPath);

        string tagDir = Path.Combine(PackagesPath, release.TagName);
        string tempZip = Path.Combine(PackagesPath, $".tmp-{release.TagName}.zip");
        string tempDir = Path.Combine(PackagesPath, $".tmp-{release.TagName}");
        string arch = RuntimeInformation.OSArchitecture == Architecture.X64 ? "x86_64" : "x86";

        try
        {
            await _client.DownloadAssetAsync(zip.BrowserDownloadUrl, tempZip, ct);

            Directory.CreateDirectory(tempDir);
            await ZipFile.ExtractToDirectoryAsync(tempZip, tempDir, ct);

            string? archDir = Directory
                .EnumerateDirectories(tempDir, "*", SearchOption.AllDirectories)
                .FirstOrDefault(d => string.Equals(Path.GetFileName(d), arch, StringComparison.OrdinalIgnoreCase)
                                  && File.Exists(Path.Combine(d, "goodbyedpi.exe")));
            if (archDir is null)
            {
                throw new InvalidOperationException($"Archive for {release.TagName} has no {arch} folder.");
            }

            if (Directory.Exists(tagDir))
            {
                Directory.Delete(tagDir, true);
            }
            Directory.Move(archDir, tagDir);

            _logger.LogInformation("Installed {Tag} to {Path}", release.TagName, tagDir);
        }
        finally
        {
            try
            {
                if (File.Exists(tempZip))
                {
                    File.Delete(tempZip);
                }

                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to clean up temporary files for {Tag}", release.TagName);
            }
        }
    }

    private string? GetLatestLocalVersion()
    {
        if (!Directory.Exists(PackagesPath))
        {
            return null;
        }

        NuGetVersion? latest = null;
        string? latestExe = null;
        foreach (string directory in Directory.EnumerateDirectories(PackagesPath))
        {
            string name = Path.GetFileName(directory);
            string exe = Path.Combine(directory, "goodbyedpi.exe");
            if (name.StartsWith(".tmp-", StringComparison.Ordinal) || !File.Exists(exe))
            {
                continue;
            }

            string tag = NormalizeTag(name);
            if (NuGetVersion.TryParse(tag, out NuGetVersion? version) && (latest is null || version > latest))
            {
                latest = version;
                latestExe = exe;
            }
        }

        return latestExe;
    }

    private static string NormalizeTag(string tag)
    {
        // e.g. "0.2.3rc3" -> "0.2.3-rc3", "0.2.0a" -> "0.2.0-a"
        Match match = Regex.Match(tag, @"^(?<ver>\d+\.\d+(?:\.\d+)?)(?<pre>[a-zA-Z].*)?$");
        if (match.Success)
        {
            string ver = match.Groups["ver"].Value;
            string pre = match.Groups["pre"].Value;
            return pre.Length > 0 ? $"{ver}-{pre}" : ver;
        }

        return tag;
    }

    private void DeleteOldVersions(string keepTag)
    {
        foreach (string directory in Directory.EnumerateDirectories(PackagesPath))
        {
            if (string.Equals(Path.GetFileName(directory), keepTag, StringComparison.Ordinal))
            {
                continue;
            }

            _logger.LogInformation("Removing old package {Directory}", directory);
            try
            {
                Directory.Delete(directory, true);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to remove old package {Directory}", directory);
            }
        }

        foreach (string zip in Directory.EnumerateFiles(PackagesPath, "*.zip"))
        {
            _logger.LogInformation("Removing leftover download {Zip}", zip);
            try
            {
                File.Delete(zip);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to remove leftover download {Zip}", zip);
            }
        }
    }
}