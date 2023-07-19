namespace Customers.Application.Cqrs.Queries;

public class AllCustomersQuery : ARequest<ImmutableList<Customer>>
{

}

internal class AllCustomersQueryHandler : ARequestHandler<AllCustomersQuery, ImmutableList<Customer>>
{
    private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

    public AllCustomersQueryHandler(
        ILogger<AllCustomersQueryHandler> logger,
        IEnumerable<IValidator<AllCustomersQuery>> validators,
        IDatabaseConnectionProvider databaseConnectionProvider) 
        : base(logger, validators)
    {
        _databaseConnectionProvider = databaseConnectionProvider;
    }

    public override async Task<OneOf<ImmutableList<Customer>, Problem>> HandleImpl(AllCustomersQuery request, CancellationToken cancellationToken)
    {
        var dbConnection = await _databaseConnectionProvider.ProvideAsync();
        
        var customers = await dbConnection.RetrieveAllCustomerEntriesAsync();
        return customers.ToImmutableList();
    }
}