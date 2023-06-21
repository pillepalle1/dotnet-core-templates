namespace Customers.Application.Cqrs.Queries;

public class RetrieveCustomerQuery : ARequestBase<OneOf<Customer, Problem>>
{
    public required Guid Id { init; get; }
}

public class RetrieveCustomerQueryValidator : AbstractValidator<RetrieveCustomerQuery>
{
    public RetrieveCustomerQueryValidator()
    {
        RuleFor(x => x.Id).IsValidId();
    }
}

internal class RetrieveCustomerQueryHandler : ARequestHandlerBase<RetrieveCustomerQuery, Customer>
{
    private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

    public RetrieveCustomerQueryHandler(
        ILogger<RetrieveCustomerQueryHandler> logger,
        IEnumerable<IValidator<RetrieveCustomerQuery>> validators,
        IDatabaseConnectionProvider databaseConnectionProvider)
        : base(logger, validators)
    {
        _databaseConnectionProvider = databaseConnectionProvider;
    }

    public override async Task<OneOf<Customer, Problem>> HandleImpl(RetrieveCustomerQuery request, CancellationToken cancellationToken)
    {
        var dbConnection = await _databaseConnectionProvider.ProvideAsync();

        var sql = @"SELECT * FROM customers WHERE Id=@Id LIMIT 1;";
        var customers = await dbConnection.QueryAsync<Customer>(sql, request);

        return customers.Any()
            ? customers.First()
            : Problem.EntityNotFound<Customer>(request.Id.ToString());
    }
}