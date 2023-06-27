namespace Customers.Application.Cqrs.Commands;

public class UpdateCustomerCmd : ARequest<Customer>
{
    public required Guid CustomerId { init; get; }
    public required string Name { init; get; }
    public required string Details { init; get; }
}

public class UpdateCustomerCmdValidator : AbstractValidator<UpdateCustomerCmd>
{
    public UpdateCustomerCmdValidator()
    {
        RuleFor(x => x.CustomerId).IsValidId();
        RuleFor(x => x.Name).IsValidCustomerName();
        RuleFor(x => x.Details).AreValidCustomerDetails();
    }
}

internal class UpdateCustomerCmdHandler : ARequestHandler<UpdateCustomerCmd, Customer>
{
    private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

    public UpdateCustomerCmdHandler(
        ILogger<UpdateCustomerCmdHandler> logger,
        IEnumerable<IValidator<UpdateCustomerCmd>> validators,
        IDatabaseConnectionProvider databaseConnectionProvider)
        : base(logger, validators)
    {
        _databaseConnectionProvider = databaseConnectionProvider;
    }

    public override async Task<OneOf<Customer, Problem>> HandleImpl(UpdateCustomerCmd request, CancellationToken cancellationToken)
    {
        var dbConnection = await _databaseConnectionProvider.ProvideAsync();
        
        // Make sure the customer exists
        var customerExists = await dbConnection.ExistsCustomerEntryAsync(request.CustomerId);
        if (!customerExists)
        {
            return Problem.EntityNotFound<Customer>(request.CustomerId.ToString());
        }
        
        // Update customer
        var updatedCustomer = new Customer()
        {
            Id = request.CustomerId,
            Name = request.Name,
            Details = request.Details
        };

        var customerUpdated = await dbConnection.UpdateCustomerEntryAsync(updatedCustomer);
        return customerUpdated
            ? updatedCustomer
            : Problem.SubsystemFailed($"Entity {typeof(Customer)} with key {request.CustomerId.ToString()} exists but was not updated");
    }
}