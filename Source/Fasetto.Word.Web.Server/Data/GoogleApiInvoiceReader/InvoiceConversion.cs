using System.Collections.Generic;
using System.Text.Json.Serialization;

public class InvoiceLineItem
{
    [JsonPropertyName("item")]
    public string Item { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public double Quantity { get; set; }

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
}

public class InvoiceExtractionResult
{
    [JsonPropertyName("items")]
    public List<InvoiceLineItem> Items { get; set; } = new();
}