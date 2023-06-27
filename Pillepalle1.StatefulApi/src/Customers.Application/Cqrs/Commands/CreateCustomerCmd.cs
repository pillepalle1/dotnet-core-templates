namespace Customers.Application.Cqrs.Commands;

public class CreateCustomerCmd : ARequest<Customer>
{
    public required string Name { init; get; }
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

internal class CreateCustomerCmdHandler : ARequestHandler<CreateCustomerCmd, Customer>
{
    private readonly IDatabaseConnectionProvider _databaseConnectionProvider;

    public CreateCustomerCmdHandler(
        ILogger<CreateCustomerCmdHandler> logger,
        IEnumerable<IValidator<CreateCustomerCmd>> validators,
        IDatabaseConnectionProvider databaseConnectionProvider) 
        : base(logger, validators)
    {
        _databaseConnectionProvider = databaseConnectionProvider;
    }

    public override async Task<OneOf<Customer, Problem>> HandleImpl(CreateCustomerCmd request, CancellationToken cancellationToken)
    {
        var dbConnection = await _databaseConnectionProvider.ProvideAsync();

        // Find unused id
        Guid id;
        bool customerExists;
        do
        {
            id = Guid.NewGuid();
            customerExists = await dbConnection.ExistsCustomerEntryAsync(id);
            
        } while (customerExists);
        
        // Create entity
        var customer = new Customer()
        {
            Id = id,
            Name = request.Name,
            Details = request.Details
        };
             
        var customerCreated =  await dbConnection.CreateCustomerEntryAsync(customer);
        return customerCreated
            ? customer
            : Problem.SubsystemFailed($"Database failed to store {typeof(Customer)} with id {id.ToString()}");
    }
}