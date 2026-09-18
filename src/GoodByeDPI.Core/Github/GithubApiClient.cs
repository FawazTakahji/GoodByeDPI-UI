using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace GoodByeDPI.Core.Github;

public class GithubApiClient
{
    private readonly HttpClient _http;

    public GithubApiClient(HttpClient http)
    {
        _http = http;
    }

    public static IHttpClientBuilder Register(IServiceCollection services, Action<HttpClient>? configureClient = null)
    {
        return services.AddHttpClient<GithubApiClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.github.com");
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            client.DefaultRequestHeaders.Add("User-Agent", "GoodByeDPI-UI");
            configureClient?.Invoke(client);
        });
    }

    public async Task<Release> GetLatestReleaseAsync(string owner, string repo, bool includePrereleases = true, CancellationToken ct = default)
    {
        string url = includePrereleases
            ? $"repos/{owner}/{repo}/releases?per_page=1"
            : $"repos/{owner}/{repo}/releases/latest";

        await using Stream response = await _http.GetStreamAsync(url, ct);

        if (includePrereleases)
        {
            List<Release>? releases = await JsonSerializer.DeserializeAsync<List<Release>>(response, cancellationToken: ct);
            if (releases is null || releases.Count < 1)
            {
                throw new InvalidOperationException("No releases found.");
            }

            return releases[0];
        }

        Release? release = await JsonSerializer.DeserializeAsync<Release>(response, cancellationToken: ct);
        return release ?? throw new InvalidOperationException("No release found.");
    }
}