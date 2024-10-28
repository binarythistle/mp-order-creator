using Newtonsoft.Json;

public partial class PageInfo
{
    [JsonProperty("endCursor")]
    public required string EndCursor { get; set; }

    [JsonProperty("hasNextPage")]
    public bool HasNextPage { get; set; }
}