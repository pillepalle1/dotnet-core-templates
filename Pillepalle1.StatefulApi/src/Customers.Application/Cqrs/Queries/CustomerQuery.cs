namespace Customers.Application.Cqrs.Queries;

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

internal class CustomerQueryHandler : ARequestHandler<CustomerQuery, Customer>
{
    private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

    public CustomerQueryHandler(
        ILogger<CustomerQueryHandler> logger,
        IEnumerable<IValidator<CustomerQuery>> validators,
        IDatabaseConnectionProvider databaseConnectionProvider)
        : base(logger, validators)
    {
        _databaseConnectionProvider = databaseConnectionProvider;
    }

    public override async Task<OneOf<Customer, Problem>> HandleImpl(CustomerQuery request, CancellationToken cancellationToken)
    {
        var dbConnection = await _databaseConnectionProvider.ProvideAsync();

        var customer = await dbConnection.RetrieveCustomerEntryAsync(request.CustomerId);
        return customer is not null
            ? customer
            : Problem.EntityNotFound<Customer>(request.CustomerId.ToString());
    }
}