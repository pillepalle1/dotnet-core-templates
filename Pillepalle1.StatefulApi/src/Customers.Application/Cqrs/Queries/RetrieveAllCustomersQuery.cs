namespace Customers.Application.Cqrs.Queries;

public class RetrieveAllCustomersQuery : ARequestBase<OneOf<ImmutableList<Customer>, Problem>>
{

}

internal class RetrieveAllCustomersQueryHandler : ARequestHandlerBase<RetrieveAllCustomersQuery, ImmutableList<Customer>>
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

        var sql = @"SELECT * FROM customers;";
        var customers = await dbConnection.QueryAsync<Customer>(sql);
        
        return customers.ToImmutableList();
    }
}