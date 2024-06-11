namespace Customers.WebApi.Mappers;

public static class CustomerMappingExtensions
{
    public static CreateCustomerCmd ToMediator(this CreateCustomerRequest request) => new()
    {
        Name = request.Name,
        Details = request.Details
    };

    public static UpdateCustomerCmd ToMediator(this UpdateCustomerRequest request, Customer customer) => new()
    {
        CustomerId = customer.Id,
        Name = request.Name ?? customer.Name,
        Details = request.Details ?? customer.Details
    };
    
    public static CustomerResponse ToContract(this Customer customer) => new()
    {
        Id = customer.Id,
        Name = customer.Name,
        Details = customer.Details
    };
}
