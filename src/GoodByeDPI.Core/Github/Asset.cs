using System.Text.Json.Serialization;

namespace GoodByeDPI.Core.Github;

public sealed record Asset
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = "";
    [JsonPropertyName("browser_download_url")]
    public string BrowserDownloadUrl { get; init; } = "";
}