namespace Customers.WebApi.Contracts;

public class UpdateCustomerRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("details")]
    public string? Details { get; set; }
}