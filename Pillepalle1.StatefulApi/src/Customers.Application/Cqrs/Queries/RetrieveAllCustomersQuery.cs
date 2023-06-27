namespace Customers.Application.Cqrs.Queries;

public class RetrieveAllCustomersQuery : ARequest<ImmutableList<Customer>>
{

}

internal class RetrieveAllCustomersQueryHandler : ARequestHandler<RetrieveAllCustomersQuery, ImmutableList<Customer>>
{
    private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

    public RetrieveAllCustomersQueryHandler(
        ILogger<RetrieveAllCustomersQueryHandler> logger,
        IEnumerable<IValidator<RetrieveAllCustomersQuery>> validators,
        IDatabaseConnectionProvider databaseConnectionProvider) 
        : base(logger, validators)
    {
        _databaseConnectionProvider = databaseConnectionProvider;
    }

    public override async Task<OneOf<ImmutableList<Customer>, Problem>> HandleImpl(RetrieveAllCustomersQuery request, CancellationToken cancellationToken)
    {
        var dbConnection = await _databaseConnectionProvider.ProvideAsync();
        
        var customers = await dbConnection.RetrieveAllCustomerEntriesAsync();
        return customers.ToImmutableList();
    }
}