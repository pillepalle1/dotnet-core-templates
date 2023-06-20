namespace Customers.WebApi.Contracts;

public class CreateCustomerRequest
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("details")]
    public required string Details { get; set; }
}