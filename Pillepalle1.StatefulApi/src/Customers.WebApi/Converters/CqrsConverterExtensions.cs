namespace Customers.WebApi.Converters;

public static class CqrsConverterExtensions
{
    public static CreateCustomerCmd ToCqrs(this CreateCustomerRequest request) => new()
    {
        Name = request.Name,
        Details = request.Details
    };

    public static UpdateCustomerCmd ToCqrs(this UpdateCustomerRequest request, Customer customer) => new()
    {
        Id = customer.Id,
        Name = request.Name ?? customer.Name,
        Details = request.Details ?? customer.Details
    };
}