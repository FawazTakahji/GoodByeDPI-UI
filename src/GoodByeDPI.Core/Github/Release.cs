using System.Text.Json.Serialization;

namespace GoodByeDPI.Core.Github;

public sealed record Release
{
    [JsonPropertyName("tag_name")]
    public string TagName { get; init; } = "";
    [JsonPropertyName("assets")]
    public Asset[] Assets { get; init; } = [];
}