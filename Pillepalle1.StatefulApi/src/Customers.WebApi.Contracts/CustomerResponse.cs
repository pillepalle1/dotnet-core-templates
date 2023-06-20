namespace Customers.WebApi.Contracts;

public class CustomerResponse
{
    [JsonPropertyName("id")]
    public required long Id { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("details")]
    public required string Details { get; set; }
}