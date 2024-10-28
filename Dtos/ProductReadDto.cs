using Newtonsoft.Json;

public partial class ProductReadDto
    {
        [JsonProperty("advertsWhere")]
        public AdvertsWhere? AdvertsWhere { get; set; }
    }

    public partial class AdvertsWhere
    {
        [JsonProperty("totalCount")]
        public long TotalCount { get; set; }

        [JsonProperty("pageInfo")]
        public required PageInfo PageInfo { get; set; }

        [JsonProperty("nodes")]
        public List<ProductNode>? ProductNodes { get; set; }
    }

    public partial class ProductNode
    {
        [JsonProperty("id")]
        public required string Id { get; set; }

        [JsonProperty("title")]
        public required string Title { get; set; }

        [JsonProperty("legacyId")]
        public long LegacyId { get; set; }

        [JsonProperty("variants")]
        public Variants? Variants { get; set; }
    }

    public partial class Variants
    {
        [JsonProperty("nodes")]
        public List<VariantNode>? VariantNodes { get; set; }
    }

    public partial class VariantNode
    {
        [JsonProperty("id")]
        public required string Id { get; set; }

        [JsonProperty("countOnHand")]
        public long CountOnHand { get; set; }

        [JsonProperty("lowestPriceCents")]
        public long LowestPriceCents { get; set; }
    }