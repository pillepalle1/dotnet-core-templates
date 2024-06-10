namespace Customers.Application.Cqrs.Customers.Commands;

public class CreateCustomerCmd : ARequest<Customer>
{
    public required string Name { init; get; }
    public string NameNormalized => Name.NormalizeCustomerName();
    
    public required string Details { init; get; }
}

public class CreateCustomerCmdValidator : AbstractValidator<CreateCustomerCmd>
{
    public CreateCustomerCmdValidator()
    {
        RuleFor(x => x.Name).IsValidCustomerName();
        RuleFor(x => x.Details).AreValidCustomerDetails();
    }
}

internal class CreateCustomerCmdHandler(
    ILogger<CreateCustomerCmdHandler> logger,
    IEnumerable<IValidator<CreateCustomerCmd>> validators,
    IDatabaseConnectionProvider dbConnectionProvider,
    ICustomerRepository customerRepository) 
    : ARequestHandler<CreateCustomerCmd, Customer>(logger, validators)
{
    public override async Task<OneOf<Customer, Problem>> HandleImpl(CreateCustomerCmd cmd, CancellationToken cancellationToken)
    {
        var dbConnection = await dbConnectionProvider.ProvideAsync();
        
        var customerCreated = await customerRepository.CreateAsync(cmd.MapToEntity() , dbConnection);
        if (customerCreated is not null)
        {
            return customerCreated;
        }
        
        return Problem.SubsystemFailed($"Database failed to store {typeof(Customer)}");
    }
}