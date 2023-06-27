namespace Customers.Application.Cqrs.Queries;

public class RetrieveCustomerQuery : ARequest<Customer>
{
    public required Guid CustomerId { init; get; }
}

public class RetrieveCustomerQueryValidator : AbstractValidator<RetrieveCustomerQuery>
{
    public RetrieveCustomerQueryValidator()
    {
        RuleFor(x => x.CustomerId).IsValidId();
    }
}

internal class RetrieveCustomerQueryHandler : ARequestHandler<RetrieveCustomerQuery, Customer>
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

        // Make sure the customer exists
        var customerExists = await dbConnection.ExistsCustomerEntryAsync(request.CustomerId);
        if (!customerExists)
        {
            return Problem.EntityNotFound<Customer>(request.CustomerId.ToString());
        }
        
        // Retrieve the customer
        return await dbConnection.RetrieveCustomerEntryAsync(request.CustomerId);
    }
}