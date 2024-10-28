using Newtonsoft.Json;

public partial class OrderCreateDto
    {
        [JsonProperty("orderCreate")]
        public required OrderCreate OrderCreate { get; set; }
    }

    public partial class OrderCreate
    {
        [JsonProperty("order")]
        public Order? Order { get; set; }

        [JsonProperty("errors")]
        public object? Errors { get; set; }
    }

    public partial class Order
    {
        [JsonProperty("id")]
        public required string Id { get; set; }

        [JsonProperty("legacyId")]
        public long LegacyId { get; set; }

        [JsonProperty("invoices")]
        public required Invoices Invoices { get; set; }
    }

    public partial class Invoices
    {
        [JsonProperty("nodes")]
        public required List<InvoiceNode> InvoiceNodes { get; set; }
    }

    public partial class InvoiceNode
    {
        [JsonProperty("id")]
        public required string Id { get; set; }

        [JsonProperty("legacyId")]
        public long LegacyId { get; set; }
    }