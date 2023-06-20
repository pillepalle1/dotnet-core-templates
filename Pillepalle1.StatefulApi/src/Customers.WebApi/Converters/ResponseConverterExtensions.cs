namespace Customers.WebApi.Converters;

public static class ResponseConverterExtensions
{
    public static CustomerResponse ToResponse(this Customer customer) => new()
    {
        Id = customer.Id,
        Name = customer.Name,
        Details = customer.Details
    };
}