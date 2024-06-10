namespace Customers.Application.Cqrs.Customers.Queries;

public class AllCustomersQuery : ARequest<ImmutableList<Customer>>
{

}

internal class AllCustomersQueryHandler(
    ILogger<AllCustomersQueryHandler> logger,
    IEnumerable<IValidator<AllCustomersQuery>> validators,
    IDatabaseConnectionProvider dbConnectionProvider,
    ICustomerRepository customerRepository) 
    : ARequestHandler<AllCustomersQuery, ImmutableList<Customer>>(logger, validators)
{
    public override async Task<OneOf<ImmutableList<Customer>, Problem>> HandleImpl(
        AllCustomersQuery query, 
        CancellationToken cancellationToken)
    {
        var dbConnection = await dbConnectionProvider.ProvideAsync();

        var customers = await customerRepository.ReadAllAsync(dbConnection);
        return customers.ToImmutableList();
    }
}