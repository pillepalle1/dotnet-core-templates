namespace Customers.Application.Cqrs.Customers.Queries;

public class CustomerQuery : ARequest<Customer>
{
    public required Guid CustomerId { init; get; }
}

public class CustomerQueryValidator : AbstractValidator<CustomerQuery>
{
    public CustomerQueryValidator()
    {
        RuleFor(x => x.CustomerId).IsValidId();
    }
}

internal class CustomerQueryHandler(
    ILogger<CustomerQueryHandler> logger,
    IEnumerable<IValidator<CustomerQuery>> validators,
    IDatabaseConnectionProvider dbConnectionProvider,
    ICustomerRepository customerRepository) 
    : ARequestHandler<CustomerQuery, Customer>(logger, validators)
{
    public override async Task<OneOf<Customer, Problem>> HandleImpl(CustomerQuery query, CancellationToken cancellationToken)
    {
        var dbConnection = await dbConnectionProvider.ProvideAsync();

        var customer = await customerRepository.ReadByIdAsync(query.CustomerId, dbConnection);
        if (customer is not null)
        {
            return customer;
        }
        
        return Problem.EntityNotFound<Customer>(query.CustomerId.ToString());
    }
}
