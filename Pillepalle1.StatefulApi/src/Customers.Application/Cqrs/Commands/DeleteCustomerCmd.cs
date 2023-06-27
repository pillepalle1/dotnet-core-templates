namespace Customers.Application.Cqrs.Commands;

public class DeleteCustomerCmd : ARequest<Unit>
{
    public required Guid CustomerId { init; get; }
}

public class DeleteCustomerCmdValidator : AbstractValidator<DeleteCustomerCmd>
{
    public DeleteCustomerCmdValidator()
    { 
        RuleFor(x => x.CustomerId).IsValidId();
    }
}

internal class DeleteCustomerCmdHandler : ARequestHandler<DeleteCustomerCmd, Unit>
{
    private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

    public DeleteCustomerCmdHandler(
        ILogger<DeleteCustomerCmdHandler> logger,
        IEnumerable<IValidator<DeleteCustomerCmd>> validators,
        IDatabaseConnectionProvider databaseConnectionProvider)
        : base(logger, validators)
    {
        _databaseConnectionProvider = databaseConnectionProvider;
    }

    public override async Task<OneOf<Unit, Problem>> HandleImpl(DeleteCustomerCmd request, CancellationToken cancellationToken)
    {
        var dbConnection = await _databaseConnectionProvider.ProvideAsync();

        // Make sure the customer exists
        var customerExists = await dbConnection.ExistsCustomerEntryAsync(request.CustomerId);
        if (!customerExists)
        {
            return Problem.EntityNotFound<Customer>(request.CustomerId.ToString());
        }
        
        // Attempt to remove the customer from database
        var customerDeleted = await dbConnection.DeleteCustomerEntryAsync(request.CustomerId);
        return customerDeleted
            ? Unit.Value
            : Problem.SubsystemFailed($"Entity {typeof(Customer)} with key {request.CustomerId.ToString()} exists but still resides in database");
    }
}