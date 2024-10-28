using Newtonsoft.Json;

public partial class SellerReadDto
{
    [JsonProperty("allSellers")]
    public Sellers? Sellers { get; set; }
}

public partial class Sellers
{
    [JsonProperty("pageInfo")]
    public required PageInfo PageInfo { get; set; }

    [JsonProperty("nodes")]
    public List<SellerNode>? SellerNodes { get; set; }
}

public partial class SellerNode
{
    [JsonProperty("id")]
    public required string Id { get; set; }

    [JsonProperty("legacyId")]
    public long LegacyId { get; set; }

    [JsonProperty("businessName")]
    public required string BusinessName { get; set; }

    [JsonProperty("online")]
    public bool Online { get; set; }
}