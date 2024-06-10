namespace Customers.Application.Cqrs.Customers.Commands;

public class DeleteCustomerCmd : ARequest<Customer>
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

internal class DeleteCustomerCmdHandler(
    ILogger<DeleteCustomerCmdHandler> logger,
    IEnumerable<IValidator<DeleteCustomerCmd>> validators,
    IDatabaseConnectionProvider dbConnectionProvider,
    ICustomerRepository customerRepository) 
    : ARequestHandler<DeleteCustomerCmd, Customer>(logger, validators)
{
    public override async Task<OneOf<Customer, Problem>> HandleImpl(DeleteCustomerCmd cmd, CancellationToken cancellationToken)
    {
        var dbConnection = await dbConnectionProvider.ProvideAsync();

        // Make sure the customer exists
        var customer = await customerRepository.ReadByIdAsync(cmd.CustomerId, dbConnection);
        if (customer is null)
        {
            return Problem.EntityNotFound<Customer>(cmd.CustomerId.ToString());
        }
        
        // Attempt to remove the customer from database
        var customerDeleted = await customerRepository.DeleteAsync(cmd.CustomerId, dbConnection);
        if (customerDeleted == 1)
        {
            return customer;
        }

        var msg = $"Entity {typeof(Customer)} with key {cmd.CustomerId.ToString()} exists but still resides in database";
        return Problem.SubsystemFailed(msg);
    }
}