namespace Customers.Application.Cqrs.Customers.Commands;

public class UpdateCustomerCmd : ARequest<Customer>
{
    public required Guid CustomerId { init; get; }
    
    public required string Name { init; get; }
    public string NameNormalized => Name.NormalizeCustomerName();
    
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

internal class UpdateCustomerCmdHandler(
    ILogger<UpdateCustomerCmdHandler> logger,
    IEnumerable<IValidator<UpdateCustomerCmd>> validators,
    IDatabaseConnectionProvider dbConnectionProvider,
    ICustomerRepository customerRepository)
    : ARequestHandler<UpdateCustomerCmd, Customer>(logger, validators)
{
    public override async Task<OneOf<Customer, Problem>> HandleImpl(UpdateCustomerCmd cmd, CancellationToken cancellationToken)
    {
        var dbConnection = await dbConnectionProvider.ProvideAsync();
        
        // Make sure the customer exists
        var customerExists = await customerRepository.ReadByIdAsync(cmd.CustomerId, dbConnection);
        if (customerExists is null)
        {
            return Problem.EntityNotFound<Customer>(cmd.CustomerId.ToString());
        }
        
        // Update customer
        var customerUpdated = await customerRepository.UpdateAsync(cmd.MapToEntity(), dbConnection);
        if (customerUpdated is not null)
        {
            return customerUpdated;
        }

        var msg = $"Entity {typeof(Customer)} with key {cmd.CustomerId.ToString()} exists but was not updated"; 
        return Problem.SubsystemFailed(msg);
    }
}
